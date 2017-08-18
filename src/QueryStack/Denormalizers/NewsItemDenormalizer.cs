using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Denormalizers
{
    public class NewsItemDenormalizer :
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

        public void Handle(NewsItemCreated message)
        {
            var newsItem = new NewsItem
            {
                Id = message.NewsItemId,
                Title = message.Title,
                Content = message.Content,
                Created = message.TimeStamp,
                CreatedBy = message.AuthorUserName,
                Tags = string.Join(",", message.Tags)
            };
            using(var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.NewsItems.Add(newsItem);
                db.SaveChanges();
            }
        }

        public void Handle(NewsItemEdited message)
        {
            using(var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems.Find(message.NewsItemId);
                newsItem.Title = message.Title;
                newsItem.Content = message.Content;
                newsItem.Tags = string.Join(",", message.Tags);
                newsItem.LastModified = message.TimeStamp;
                newsItem.LastModifiedBy = message.EditorUserName;
                db.SaveChanges();
            }
        }

        public void Handle(NewsItemPublished message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems.Find(message.NewsItemId);
                newsItem.Published = message.TimeStamp;
                newsItem.IsPublished = true;
                db.SaveChanges();
            }
        }

        public void Handle(NewsItemCommented message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                NewsItem newsItem = db.NewsItems.Find(message.NewsItemId);
                if (newsItem.Comments == null)
                {
                    newsItem.Comments = new List<NewsItemComment>();
                }
                newsItem.Comments.Add(new NewsItemComment
                {
                    Id = message.CommentId,
                    Content = message.Content,
                    Created = message.TimeStamp,
                    CreatedBy = message.UserName,
                });
                newsItem.CommentsCount++;
                db.SaveChanges();
            }
        }
    }
}
