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
        private readonly DevPortalDbContext _queryModelDb;

        public NewsItemDenormalizer(DevPortalDbContext queryModelDb) : base(queryModelDb)
        {
            this._queryModelDb = queryModelDb;
        }

        public Task Handle(NewsItemCreated message)
        {
            var newsItem = base.MapCreated(message);
            DenormalizedView.Add(newsItem);
            return Task.FromResult(message);
        }

        public async Task Handle(NewsItemEdited message)
        {
            NewsItem newsItem = await _queryModelDb.NewsItems
                .Include(i => i.Tags)
                .FirstAsync(i => i.Id == message.NewsItemId);

            base.MapEdited(message, newsItem);            
        }

        public async Task Handle(NewsItemPublished message)
        {
            NewsItem newsItem = await _queryModelDb.NewsItems.FindAsync(message.NewsItemId);
            newsItem.Published = message.TimeStamp;
            newsItem.IsPublished = true;
        }

        public async Task Handle(NewsItemCommented message)
        {
            NewsItem newsItem = await _queryModelDb.NewsItems.FindAsync(message.NewsItemId);

            newsItem.Comments.Add(new NewsItemComment
            {
                Id = message.CommentId,
                Content = message.Content,
                Created = message.TimeStamp,
                CreatedBy = message.UserName,
            });
            newsItem.CommentsCount++;
        }
    }
}
