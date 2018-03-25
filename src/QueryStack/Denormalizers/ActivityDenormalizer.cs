using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack.Denormalizers
{
    public class ActivityDenormalizer :
        IHandleMessages<NewsItemPublished>,
        IHandleMessages<NewsItemCommented>,
        IHandleMessages<ForumThreadCreated>,
        IHandleMessages<ForumItemPosted>,
        IHandleMessages<BlogLinkCreated>
    {
        private readonly DbContextOptions<DevPortalDbContext> _dbContextOptions;

        public ActivityDenormalizer(DbContextOptions<DevPortalDbContext> dbContextOptions)
        {
            this._dbContextOptions = dbContextOptions;
        }

        public async Task Handle(NewsItemPublished message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.NewsItems.AsNoTracking().First(i => i.Id == message.NewsItemId);                   
                await SaveActivity<NewsItem>(message.NewsItemId, newsItem.Title, newsItem.CreatedBy, message, db);
            }
          
        }


        public async Task Handle(NewsItemCommented message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.NewsItems.AsNoTracking().First(i => i.Id == message.NewsItemId);
                await SaveActivity<NewsItem>(message.NewsItemId, newsItem.Title, newsItem.CreatedBy, message, db);
            }
        }

        public async Task Handle(ForumThreadCreated message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                await SaveActivity<ForumThread>(message.ForumThreadId, message.Title, message.AuthorUserName, message, db);
            }
        }

        public async Task Handle(ForumItemPosted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.ForumThreads.AsNoTracking().First(i => i.Id == message.ForumThreadId);
                await SaveActivity<ForumThread>(message.ForumThreadId, newsItem.Title, message.AuthorUserName, message, db);
            }
        }

        public async Task Handle(BlogLinkCreated message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                await SaveActivity<Blog>(message.BlogId, message.Title, message.UserName, message, db, message.Url);
            }
        }


        private Task SaveActivity<TContentType>(Guid contentId, string title, string userName, DomainEvent message, DevPortalDbContext db, string externalUrl = null)
        {
            db.Activities.Add(new Activity
            {
                ActivityId = message.Id,
                ContentId = contentId,
                ContentType = typeof(TContentType).Name,
                TimeStamp = message.TimeStamp,
                ContentTitle = title,
                Action = message.GetType().Name,
                UserName = userName,
                ExternalUrl = externalUrl,
            });
            return db.SaveChangesAsync();
        }
    }
}
