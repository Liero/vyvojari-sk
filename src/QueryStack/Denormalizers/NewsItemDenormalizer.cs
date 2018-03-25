using DevPortal.CommandStack.Events;
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
    public class NewsItemDenormalizer : ContentDenormalizerBase<NewsItem>,
        IHandleMessages<NewsItemCreated>,
        IHandleMessages<NewsItemEdited>,
        IHandleMessages<NewsItemPublished>,
        IHandleMessages<NewsItemCommented>
    {
        private readonly DbContextOptions<DevPortalDbContext> _dbContextOptions;

        public NewsItemDenormalizer(DbContextOptions<DevPortalDbContext> dbContextOptions)
        {
            this._dbContextOptions = dbContextOptions;
        }

        public async Task Handle(NewsItemCreated message)
        {
            var newsItem = base.MapCreated(message);
            using(var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.NewsItems.Add(newsItem);
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(NewsItemEdited message)
        {
            using(var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems
                    .Include(i => i.Tags)
                    .First(i => i.Id == message.NewsItemId);

                base.MapEdited(message, newsItem);
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(NewsItemPublished message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems.Find(message.NewsItemId);
                newsItem.Published = message.TimeStamp;
                newsItem.IsPublished = true;
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(NewsItemCommented message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems.Find(message.NewsItemId);

                newsItem.Comments.Add(new NewsItemComment
                {
                    Id = message.CommentId,
                    Content = message.Content,
                    Created = message.TimeStamp,
                    CreatedBy = message.UserName,
                });
                newsItem.CommentsCount++;
                await db.SaveChangesAsync();
            }
        }
    }
}
