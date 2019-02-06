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
        private readonly DevPortalDbContext _queryModelDb;

        public ForumThreadDenormalizer(DevPortalDbContext queryModelDb) : base(queryModelDb)
        {
            _queryModelDb = queryModelDb;
        }

        #region ForumThread

        public Task Handle(ForumThreadCreated message)
        {
            //generic content mapping
            ForumThread entity = base.MapCreated(message);

            //specific for ForumThread
            entity.LastPosted = message.TimeStamp;
            entity.LastPostedBy = message.AuthorUserName;

            DenormalizedView.Add(entity);

            return Task.CompletedTask;
        }

        public Task Handle(ForumThreadEdited message)
        {
            ForumThread entity = DenormalizedView
                .Include(f => f.Tags)
                .FirstOrDefault(f => f.Id == message.ForumThreadId);
            MapEdited(message, entity);

            return Task.CompletedTask;
        }

        public Task Handle(ForumThreadDeleted message)
        {
            var entity = new ForumThread { Id = message.Id };
            DenormalizedView.Attach(entity);
            DenormalizedView.Remove(entity);

            return Task.CompletedTask;
        }

        #endregion ForumThread

        #region ForumItems

        public Task Handle(ForumItemPosted message)
        {
            ForumThread forumThread = DenormalizedView.Find(message.ForumThreadId);
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

            return Task.CompletedTask;
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
            ForumPost post = await _queryModelDb.ForumPosts
                .Include(p => p.Thread)
                .FirstAsync(p => p.Id == message.ForumItemId);

            post.Thread.PostsCount++;
            post.Content = message.Content;
            post.LastModified = message.TimeStamp;
            post.LastModifiedBy = message.EditorUserName;
        }

        public Task Handle(ForumItemDeleted message)
        {
            var entity = new ForumPost { Id = message.ForumItemId };
            _queryModelDb.ForumPosts.Attach(entity);
            _queryModelDb.RemoveRange(entity);

            return Task.CompletedTask;
        }

        #endregion ForumItems

    }
}
