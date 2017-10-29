using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack.Denormalizers
{
    public class ForumThreadDenormalizer :
        IHandleMessages<ForumThreadCreated>,
        IHandleMessages<ForumThreadEdited>,
        IHandleMessages<ForumThreadDeleted>,

        IHandleMessages<ForumItemPosted>,
        IHandleMessages<ForumItemEdited>,
        IHandleMessages<ForumItemDeleted>
    {
        private readonly DbContextOptions<DevPortalDbContext> _dbContextOptions;

        public ForumThreadDenormalizer(DbContextOptions<DevPortalDbContext> dbContextOptions)
        {
            this._dbContextOptions = dbContextOptions;
        }

        #region ForumThread

        public void Handle(ForumThreadCreated message)
        {
            var entity = new ForumThread
            {
                Id = message.ForumThreadId,
                Title = message.Title,
                Content = message.Content,
                Created = message.TimeStamp,
                CreatedBy = message.AuthorUserName,
                Tags = string.Join(",", message.Tags)
            };
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.ForumThreads.Add(entity);
                db.SaveChanges();
            }
        }

        public void Handle(ForumThreadEdited message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                ForumThread entity = db.ForumThreads.Find(message.ForumThreadId);
                entity.Title = message.Title;
                entity.Content = message.Content;
                entity.Tags = string.Join(",", message.Tags);
                entity.LastModified = message.TimeStamp;
                entity.LastModifiedBy = message.EditorUserName;
                db.SaveChanges();
            }
        }

        public void Handle(ForumThreadDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new ForumThread { Id = message.Id };
                db.ForumThreads.Attach(entity);
                db.ForumThreads.Remove(entity);
                db.SaveChanges();
            }
        }

        #endregion ForumThread

        #region ForumItems

        public void Handle(ForumItemPosted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                ForumThread forumThread = db.ForumThreads.Find(message.ForumThreadId);
                Trace.WriteLine("Posted ForumItem with Id " + message.ForumItemId);
                forumThread.Posts.Add(new ForumPost
                {
                    Id = message.ForumItemId,
                    Content = message.Content,
                    Created = message.TimeStamp,
                    CreatedBy = message.AuthorUserName,
                });
                forumThread.PostsCount++;
                db.SaveChanges();
            }
        }

        public void Handle(ForumItemEdited message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                ForumPost post = db.ForumPosts
                    .Include(p => p.Thread)
                    .First(p => p.Id == message.ForumItemId);

                post.Thread.PostsCount++;
                post.Content = message.Content;
                post.LastModified = message.TimeStamp;
                post.LastModifiedBy = message.EditorUserName;
                db.SaveChanges();
            }
        }

        public void Handle(ForumItemDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new ForumThread { Id = message.Id };
                db.ForumThreads.Attach(entity);
                db.ForumThreads.RemoveRange(entity);
                db.SaveChanges();
            }
        }

        #endregion ForumItems

    }
}
