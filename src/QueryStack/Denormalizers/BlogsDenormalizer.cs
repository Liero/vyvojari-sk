using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack.Denormalizers
{
    public class BlogDenormalizer :
        IHandleMessages<BlogLinkCreated>,
        IHandleMessages<BlogDeleted>
    {
        private readonly DbSet<Blog> _denormalizedView;

        public BlogDenormalizer(DevPortalDbContext queryModelDb)
        {
            this._denormalizedView = queryModelDb.Blogs;
        }

        public Task Handle(BlogLinkCreated message)
        {
            var entity = new Blog
            {
                Id = message.BlogId,
                Title = message.Title,
                ExternalUrl = message.Url,
                Description = message.Description,
                Created = message.TimeStamp,
                CreatedBy = message.UserName,
            };
            _denormalizedView.Add(entity);

            return Task.CompletedTask;
        }

        //public async Task Handle(BlogEdited message)
        //{
        //    using (var db = new DevPortalDbContext(_dbContextOptions))
        //    {
        //        Blog entity = _db.Blogs.Find(message.BlogId);
        //        entity.Title = message.Title;
        //        entity.Content = message.Content;
        //        entity.Tags = string.Join(",", message.Tags);
        //        entity.LastModified = message.TimeStamp;
        //        entity.LastModifiedBy = message.EditorUserName;
        //        await _db.SaveChangesAsync();
        //    }
        //}

        public Task Handle(BlogDeleted message)
        {
            var entity = new Blog { Id = message.BlogId };
            _denormalizedView.Attach(entity);
            _denormalizedView.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
