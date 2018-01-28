using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Handle(NewsItemPublished message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.NewsItems.AsNoTracking().First(i => i.Id == message.NewsItemId);                   
                SaveActivity<NewsItem>(message.NewsItemId, newsItem.Title, newsItem.CreatedBy, message, db);
            }
          
        }


        public void Handle(NewsItemCommented message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.NewsItems.AsNoTracking().First(i => i.Id == message.NewsItemId);
                SaveActivity<NewsItem>(message.NewsItemId, newsItem.Title, newsItem.CreatedBy, message, db);
            }
        }

        public void Handle(ForumThreadCreated message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                SaveActivity<ForumThread>(message.ForumThreadId, message.Title, message.AuthorUserName, message, db);
            }
        }

        public void Handle(ForumItemPosted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                //this introduces dependency on another view. Consider Refactoring using events
                var newsItem = db.ForumThreads.AsNoTracking().First(i => i.Id == message.ForumThreadId);
                SaveActivity<ForumThread>(message.ForumThreadId, newsItem.Title, newsItem.CreatedBy, message, db);
            }
        }

        public void Handle(BlogLinkCreated message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                SaveActivity<Blog>(message.BlogId, message.Title, message.UserName, message, db);
            }
        }


        private void SaveActivity<TContentType>(Guid contentId, string title, string userName, DomainEvent message, DevPortalDbContext db)
        {
            db.Activities.Add(new Activity
            {
                ActivityId = message.Id,
                ContentId = contentId,
                TimeStamp = message.TimeStamp,
                ContentTitle = title,
                Action = message.GetType().Name,
                UserName = userName,
            });
            db.SaveChanges();
        }
    }
}
