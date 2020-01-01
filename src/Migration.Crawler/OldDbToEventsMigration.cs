using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migration.Crawler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Migration.Crawler
{
    public class OldDbToEventsMigration
    {
        OldPortalDbContext _oldDb;
        IServiceProvider provider;
        int lastId = 0;
        Html2Markdown.Converter html2Markdown = new Html2Markdown.Converter();

        public OldDbToEventsMigration()
        {
            _oldDb = new OldPortalDbContext();
            var allEvents = Assembly.GetAssembly(typeof(NewsItemCreated))
                .GetExportedTypes()
                .Where(e => typeof(DomainEvent).IsAssignableFrom(e)); ;

            ServiceCollection services = new ServiceCollection();
            services.AddTransient<IEventStore, SqlEventStore>(sp => ActivatorUtilities.CreateInstance<SqlEventStore>(sp, new object[] { allEvents.ToArray() }));
            services.AddSingleton<IEventDispatcher, FakeEventDispatcher>();
            services.AddDbContextPool<EventsDbContext>(option =>
            {
                option.UseSqlServer("Server=localhost;Database=DevPortal_EventStore;Trusted_Connection=True");
            });
            provider = services.BuildServiceProvider();

        }

        private string toMarkDown(string content)
        {
            try
            {
                return html2Markdown.Convert(content);
            }
            catch (Exception)
            {
                return content;
            }
        }

        internal void MigrateNewsItems()
        {
            lastId = 0;
            IEnumerable<NewsItem> nextPage;
            do
            {
                nextPage = GetNextPageNewsItems();
                int count = 0;
                var sw = new Stopwatch();
                sw.Restart();

                foreach (var entity in nextPage)
                {
                    var eventStore = provider.GetRequiredService<IEventStore>();
                    using (eventStore as IDisposable)
                    {
                        var newsItemId = Guid.NewGuid();
                        eventStore.Save(new NewsItemCreated
                        {
                            AuthorUserName = entity.Author,
                            Content = toMarkDown(entity.Text),
                            Title = entity.Title,
                            NewsItemId = newsItemId,
                            TimeStamp = entity.Published,
                            Url = entity.Url,
                            Tags = new[] { "old-portal" },
                            OriginalId = entity.Id
                        });
                        eventStore.Save(new NewsItemPublished
                        {
                            TimeStamp = entity.Published,
                            NewsItemId = newsItemId
                        });
                        foreach (var item in entity.Comments)
                        {
                            eventStore.Save(new NewsItemCommented
                            {
                                CommentId = Guid.NewGuid(),
                                UserName = item.Author,
                                Content = item.Text,
                                TimeStamp = item.Date,
                                NewsItemId = newsItemId
                            });
                        }
                        lastId = entity.Id;
                        count += entity.Comments.Count + 2;
                    }
                }
                if (sw.ElapsedMilliseconds > 0)
                {
                    Console.WriteLine($"Generated {count} news events in {sw.ElapsedMilliseconds / 1000d} sec = {count * 1000 / sw.ElapsedMilliseconds} evt/s");
                }
            } while (nextPage.Any());
        }

        internal void MigrateForumThreads()
        {
            lastId = 0;
            IEnumerable<ForumThread> nextPage;
            do
            {
                nextPage = GetNextPageForumThreads();
                int count = 0;
                var sw = new Stopwatch();
                sw.Restart();

                var categories = _oldDb.Categories.ToDictionary(c => c.Id, c => c.Title.ToLower().Replace(' ', '_'));

                foreach (var entity in nextPage)

                {
                    var eventStore = provider.GetRequiredService<IEventStore>();
                    using (eventStore as IDisposable)
                    {
                        var forumThreadId = Guid.NewGuid();
                        eventStore.Save(new ForumThreadCreated
                        {
                            AuthorUserName = entity.Author,
                            Content = toMarkDown(entity.Text),
                            Title = entity.Title,
                            ForumThreadId = forumThreadId,
                            TimeStamp = entity.Date,
                            Tags = new[] { "old-portal-" + categories.GetValueOrDefault(entity.ForumCategoryId) },
                            OriginalId = entity.Id
                        });
                        foreach (var answer in entity.Answers)
                        {
                            eventStore.Save(new ForumItemPosted
                            {
                                Id = Guid.NewGuid(),
                                AuthorUserName = answer.Author,
                                Content = toMarkDown(answer.Text),
                                TimeStamp = answer.Date,
                                ForumThreadId = forumThreadId
                            });
                        }
                        lastId = entity.Id;
                        count += entity.Answers.Count + 1;
                    }
                }
                if (sw.ElapsedMilliseconds > 0)
                {
                    Console.WriteLine($"Generated {count} forum events in {sw.ElapsedMilliseconds / 1000d} sec = {count * 1000 / sw.ElapsedMilliseconds} evt/s");
                }

            } while (nextPage.Any());
        }

        internal void MigrateBlogs()
        {
            lastId = 0;
            IEnumerable<Blog> nextPage;
            do
            {
                nextPage = GetNextPageBlog();
                int count = 0;
                var sw = new Stopwatch();
                sw.Restart();

                foreach (var entity in nextPage)
                {
                    var eventStore = provider.GetRequiredService<IEventStore>();
                    using (eventStore as IDisposable)
                    {
                        eventStore.Save(new BlogLinkCreated
                        {
                            UserName = entity.Author,
                            Title = entity.Title,
                            BlogId = Guid.NewGuid(),
                            TimeStamp = entity.Date,
                            Url = entity.Url,
                            Description = "Migrated from old portal",
                        });
                        lastId = entity.Id;

                        count += 1;
                    }
                }
                if (sw.ElapsedMilliseconds > 0)
                {
                    Console.WriteLine($"Generated {count} blog events in {sw.ElapsedMilliseconds / 1000d} sec = {count * 1000 / sw.ElapsedMilliseconds} evt/s");
                }

            } while (nextPage.Any());
        }


        IEnumerable<NewsItem> GetNextPageNewsItems()
        {
            using (var db = provider.GetRequiredService<EventsDbContext>())
            {
                if (lastId == 0)
                {
                    var evt = db.Events.OrderByDescending(e => e.EventNumber).Where(e => e.EventType == "NewsItemCreated").FirstOrDefault();
                    if (evt != null)
                    {
                        lastId = JsonConvert.DeserializeObject<JObject>(evt.SerializedEvent).Property("OriginalId").Value.Value<int>();
                    }
                }
            }
            return _oldDb.News.Where(n => n.Id > lastId)
                .Include(n => n.Comments)
                .OrderBy(n => n.Id)
                .Take(100)
                .ToArray();
        }

        IEnumerable<ForumThread> GetNextPageForumThreads()
        {
            using (var db = provider.GetRequiredService<EventsDbContext>())
            {
                if (lastId == 0)
                {
                    var evt = db.Events.OrderByDescending(e => e.EventNumber).Where(e => e.EventType == "ForumThreadCreated").FirstOrDefault();
                    if (evt != null)
                    {
                        lastId = JsonConvert.DeserializeObject<JObject>(evt.SerializedEvent).Property("OriginalId").Value.Value<int>();
                    }
                }
            }
            return _oldDb.ForumThreads.Where(n => n.Id > lastId)
                .Include(n => n.Answers)
                .OrderBy(n => n.Id)
                .Take(100)
                .ToArray(); 
        }

        IEnumerable<Blog> GetNextPageBlog()
        {
            using (var db = provider.GetRequiredService<EventsDbContext>())
            {
            }
            return _oldDb.Blogs.Where(n => n.Id > lastId)
                .OrderBy(n => n.Id)
                .Take(100)
                .ToArray();
        }
    }
}
