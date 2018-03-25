using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack.Denormalizers
{
    public class ForumThreadDenormalizer : ContentDenormalizerBase<ForumThread>,
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

        public async Task Handle(ForumThreadCreated message)
        {
            //generic content mapping
            ForumThread entity = base.MapCreated(message);

            //specific for ForumThread
            entity.LastPosted = message.TimeStamp;
            entity.LastPostedBy = message.AuthorUserName;
         
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.ForumThreads.Add(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(ForumThreadEdited message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                ForumThread entity = db.ForumThreads
                    .Include(f => f.Tags)
                    .FirstOrDefault(f => f.Id == message.ForumThreadId);
                MapEdited(message, entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(ForumThreadDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new ForumThread { Id = message.Id };
                db.ForumThreads.Attach(entity);
                db.ForumThreads.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        #endregion ForumThread

        #region ForumItems

        public async Task Handle(ForumItemPosted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                ForumThread forumThread = db.ForumThreads.Find(message.ForumThreadId);
                forumThread.LastPosted = message.TimeStamp;
                forumThread.LastPostedBy = message.AuthorUserName;

                SetParticipantsCsv(message, forumThread);

                forumThread.Posts.Add(new ForumPost
                {
                    Id = message.ForumItemId,
                    Content = message.Content,
                    Created = message.TimeStamp,
                    CreatedBy = message.AuthorUserName,
                });
                forumThread.PostsCount++;
                await db.SaveChangesAsync();
            }
        }

        private static void SetParticipantsCsv(ForumItemPosted message, ForumThread forumThread)
        {
            var participants = forumThread.ParticipantsCsv.Split(',').ToList();
            participants.Remove(message.AuthorUserName);
            participants.Add(message.AuthorUserName);
            forumThread.ParticipantsCsv = string.Join(",", participants);
        }

        public async Task Handle(ForumItemEdited message)
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
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(ForumItemDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new ForumThread { Id = message.Id };
                db.ForumThreads.Attach(entity);
                db.ForumThreads.RemoveRange(entity);
                await db.SaveChangesAsync();
            }
        }

        #endregion ForumItems

    }
}
