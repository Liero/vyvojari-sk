using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Migration.Crawler
{


    class Program
    {
        static HttpClient client;
        static HtmlWeb web = new HtmlWeb();

        static void Main(string[] args)
        {
            //Main().GetAwaiter().GetResult();
            //using (var db = new OldPortalDbContext())
            //{
            //    //db.Database.EnsureDeleted();
            //    db.Database.EnsureCreated();
            //    foreach (var item in CrawlNews(1, 271))
            //    {
            //        if (db.News.Any(n => n.Id == item.Id)) continue;
            //        db.News.Add(item);
            //        db.SaveChanges();
            //    }
            //}

            //foreach (var categ in CrawlForumCategories())
            //{

            //    using (var db = new OldPortalDbContext())
            //    {
            //        //db.Database.EnsureDeleted();
            //        db.Database.EnsureCreated();
            //        if (!db.Categories.Any(c => c.Id == categ.Id))
            //        {
            //            db.Categories.Add(categ);
            //            db.SaveChanges();
            //        }

            //        foreach (var thread in CrawlForumThreads(categ))
            //        {
            //            if (Utils.Spammers.Contains(thread.Author))
            //            {
            //                Console.WriteLine("Skipping spammer " + thread.Author);
            //                continue;
            //            }
            //            CrawlSingleForumThread(thread);
            //            if (!db.ForumThreads.Any(t => t.Id == thread.Id))
            //            {
            //                db.ForumThreads.Add(thread);
            //                db.SaveChanges();
            //                db.Entry(thread).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            //            }
            //        }
            //    }

            //}

            using (var db = new OldPortalDbContext())
            {
                foreach (var blog in CrawlBlogs())
                {
                    db.Blogs.Add(blog);
                    db.SaveChanges();
                }
            }
        }

        static IEnumerable<Blog> CrawlBlogs()
        {
            string nextUrl =  "/Blogs.aspx";
            while (nextUrl != null)
            {
                Console.Write("Querying " + nextUrl);
                var doc = web.Load("http://vyvojari.sk" + nextUrl);
                Console.WriteLine(" - OK");

                foreach (var article in doc.QuerySelectorAll(".blog_article"))
                {
                    var blog = new Blog
                    {
                        Title = article.QuerySelector("h3").InnerText.Trim('\n', '\r', '\t'),
                        Url = article.QuerySelector("h3 a").GetAttributeValue("href", ""),
                        Author = article.QuerySelector(".author").InnerText.Split(" od ")[1],
                        Date = DateTime.Parse(article.QuerySelector(".date").InnerText, CultureInfo.GetCultureInfo("sk-sk")),
                        Views = int.Parse(article.QuerySelector(".views").InnerText.Split(" views")[0])
                    };
                    yield return blog;
                }

                nextUrl = doc.QuerySelectorAll("#ctl00_MainContent_Pager1_PagingLabel a")
                    .FirstOrDefault(a => a.InnerHtml == " ďalej")
                    ?.GetAttributeValue("href", "");
            }

        }

        static void CrawlSingleForumThread(ForumThread post)
        {
            if (post.Title.ToLower().Contains("buy ") || post.Author == "sdfgdfgsdfdf02" || post.Author == "rsorderforgame") return;
            Console.Write("Querying Post " + post.Id);
            var doc = web.Load("http://vyvojari.sk" + post.Url);
            Console.WriteLine(" - OK");
            var items = doc.QuerySelectorAll("article.topic_item").Select(topic => new
            {
                Id = int.Parse(topic.QuerySelector("h3 a[name]").GetAttributeValue("name", "")),
                Author = topic.QuerySelector(".info a").InnerText,
                Date = DateTime.Parse(topic.QuerySelector(".info .date").InnerText, CultureInfo.GetCultureInfo("sk-sk")),
                Text = string.Join(Environment.NewLine,
                        topic.QuerySelector(".text")
                            .ChildNodes
                            .Where(tag => !tag.HasClass("ForumAdminButtons")
                                    && tag.QuerySelector(".ForumSignature") == null
                                    && !tag.Descendants().Any(t => t.Id.Contains("LabelSignature")))
                            .Select(n => n.OuterHtml))
                        .Trim('\r', '\n', '\t')
            }).ToList();
            post.Text = items[0].Text;
            post.Date = items[0].Date;
            post.Answers = new List<Answer>();

            foreach (var item in items.Skip(1))
            {
                post.Answers.Add(new Answer
                {
                    IdWithinPost = item.Id,
                    Author = item.Author,
                    Date = item.Date,
                    Text = item.Text
                });
            }
        }

        static IEnumerable<ForumThread> CrawlForumThreads(ForumCategory categ)
        {
            int page = 0;
            string nextPage = categ.Url;
            while (nextPage != null)
            {
                page++;

                Console.Write("Querying Category " + categ.Title + " Page " + page);
                var doc = web.Load("http://vyvojari.sk" + nextPage);
                Console.WriteLine(" - OK");

                var rows = doc.QuerySelectorAll(".ForumTable tr").Skip(1);
                foreach (var row in rows)
                {
                    var link = row.QuerySelector(".ForumTopicListLeft a");
                    yield return new ForumThread
                    {
                        Id = int.Parse(link.GetAttributeValue("href", "").Split('-', '.').Reverse().ElementAt(1)),
                        ForumCategoryId = categ.Id,
                        Title = link.InnerText,
                        Url = link.GetAttributeValue("href", ""),
                        Author = row.QuerySelector(".ForumTopicList a").InnerText
                    };
                }
                nextPage = doc.QuerySelectorAll("#ctl00_MainContent_Topics1_PagingLabel a")
                    .FirstOrDefault(a => a.InnerHtml == ">")
                    ?.GetAttributeValue("href", "");
            }
        }


        static IEnumerable<ForumCategory> CrawlForumCategories()
        {
            Console.WriteLine("Querying Categories");

            var doc = web.Load("http://vyvojari.sk/Forum/Default.aspx");
            var categories = doc.QuerySelectorAll(".ForumTopicListLeft").Select(td => new ForumCategory
            {
                Id = int.Parse(td.QuerySelector("a").GetAttributeValue("href", "").Split('-', '.').Reverse().ElementAt(1)),
                Title = td.QuerySelector("a").InnerText,
                Url = td.QuerySelector("a").GetAttributeValue("href", ""),
                Description = td.QuerySelector("span").InnerText
            });
            return categories;
        }





        static IEnumerable<NewsItem> CrawlNews(int startPage, int endPage)
        {

            var articles = Enumerable.Range(startPage, endPage).SelectMany(page =>
            {
                Console.Write("Querying News Page " + page);
                var doc = web.Load("http://vyvojari.sk/News.aspx?page=" + page);
                Console.WriteLine(" - Ok");
                var articleNodes = doc.QuerySelectorAll("article.news");
                var articles_ = articleNodes.Select(article => new NewsItem
                {
                    Url = article.QuerySelector("a:not(.comment)").Attributes["href"].Value,
                    Title = article.QuerySelector("h3").InnerText,
                }).ToList();
                return articles_;
            });


            int i = 0;
            foreach (var article in articles)
            {
                Console.Write("Querying NewsItem " + ++i);
                var doc = web.Load("http://vyvojari.sk" + article.Url);
                Console.Write(" - Ok");

                var articleNode = doc.QuerySelector("section.content article");
                article.Id = int.Parse(article.Url.Split('-', '.').Reverse().ElementAt(1));
                article.Title = articleNode.QuerySelector("h1").InnerText;
                article.Published = DateTime.Parse(articleNode.QuerySelector(".date").InnerText, CultureInfo.GetCultureInfo("sk-sk"));
                article.Author = articleNode.QuerySelector(".ListArticleInfo a").InnerText;
                article.Views = int.Parse(articleNode.QuerySelector(".views").InnerText.Replace(" views", ""));
                article.Text = articleNode.QuerySelector("section.ListArticleText").InnerHtml.Trim('\r', '\n', '\t');
                article.Comments = doc.QuerySelectorAll("#comments section.comment").Select(commentNode => new Comment
                {
                    Id = int.Parse(commentNode.QuerySelector("a[name]").Attributes["name"].Value.Trim('#')),
                    Text = commentNode.QuerySelector(".text").InnerHtml.Trim('\r', '\n', '\t'),
                    Author = commentNode.QuerySelector("a:not([name])").InnerHtml,
                    Date = DateTime.Parse(commentNode.QuerySelector(".date").InnerHtml, CultureInfo.GetCultureInfo("sk-sk"))
                }).ToList();
                Console.WriteLine(" - Parsing Ok");
                yield return article;
            }
        }



        //static async Task Main()
        //{
        //    client = new HttpClient()
        //    {
        //        BaseAddress = new Uri("http://vyvojari.sk/")
        //    };
        //    Console.WriteLine("Hitting " + client.BaseAddress.AbsoluteUri);

        //    var response = await client.GetStringAsync(");
        //}
    }

}
