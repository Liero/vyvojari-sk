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
        private readonly DbContextOptions<DevPortalDbContext> _dbContextOptions;

        public BlogDenormalizer(DbContextOptions<DevPortalDbContext> dbContextOptions)
        {
            this._dbContextOptions = dbContextOptions;
        }

        public async Task Handle(BlogLinkCreated message)
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
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.Blogs.Add(entity);
                await db.SaveChangesAsync();
            }
        }

        //public async Task Handle(BlogEdited message)
        //{
        //    using (var db = new DevPortalDbContext(_dbContextOptions))
        //    {
        //        Blog entity = db.Blogs.Find(message.BlogId);
        //        entity.Title = message.Title;
        //        entity.Content = message.Content;
        //        entity.Tags = string.Join(",", message.Tags);
        //        entity.LastModified = message.TimeStamp;
        //        entity.LastModifiedBy = message.EditorUserName;
        //        await db.SaveChangesAsync();
        //    }
        //}

        public async Task Handle(BlogDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new Blog { Id = message.BlogId };
                db.Blogs.Attach(entity);
                db.Blogs.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

    }
}
