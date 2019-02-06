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
        private readonly DevPortalDbContext _db;

        public ActivityDenormalizer(DevPortalDbContext queryDb)
        {
            _db = queryDb;
        }

        public async Task Handle(NewsItemPublished message)
        {
                var newsItem = await _db.NewsItems.AsNoTracking().FirstAsync(i => i.Id == message.NewsItemId);                   
                AddActivity<NewsItem>(message.NewsItemId, newsItem.Title, newsItem.CreatedBy, message);
        }

        public async Task Handle(NewsItemCommented message)
        {
                //this introduces dependency on another view. Consider Refactoring using events
            var newsItem = await _db.NewsItems.AsNoTracking().FirstAsync(i => i.Id == message.NewsItemId);

            AddActivity<NewsItem>(message.NewsItemId, newsItem.Title, message.UserName, message,
                    fragment: message.CommentId);
        }

        public Task Handle(ForumThreadCreated message)
        {
            AddActivity<ForumThread>(message.ForumThreadId, message.Title, message.AuthorUserName, message);
            return Task.CompletedTask;
        }

        public async Task Handle(ForumItemPosted message)
        {
            var thread = await _db.ForumThreads.AsNoTracking().FirstAsync(i => i.Id == message.ForumThreadId);
            AddActivity<ForumThread>(message.ForumThreadId, thread.Title, message.AuthorUserName, message, 
                fragment: message.ForumItemId);
        }

        public Task Handle(BlogLinkCreated message)
        {
            AddActivity<Blog>(message.BlogId, message.Title, message.UserName, message, message.Url);
            return Task.CompletedTask;
        }

        private void AddActivity<TContentType>(
            Guid contentId, 
            string title, 
            string userName, 
            DomainEvent message, 
            string externalUrl = null,
            Guid? fragment = null)
        {
            _db.Activities.Add(new Activity
            {
                ActivityId = message.Id,
                ContentId = contentId,
                Fragment = fragment,
                ContentType = typeof(TContentType).Name,
                TimeStamp = message.TimeStamp,
                ContentTitle = title,
                Action = message.GetType().Name,
                UserName = userName,
                ExternalUrl = externalUrl,
            });
        }
    }
}
