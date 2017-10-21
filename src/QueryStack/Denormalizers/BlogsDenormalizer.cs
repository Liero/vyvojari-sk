using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void Handle(BlogLinkCreated message)
        {
            var entity = new Blog
            {
                Id = message.BlogId,
                Title = message.Title,
                Description = message.Description,
                Created = message.TimeStamp,
                CreatedBy = message.UserName,
            };
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                db.Blogs.Add(entity);
                db.SaveChanges();
            }
        }

        //public void Handle(BlogEdited message)
        //{
        //    using (var db = new DevPortalDbContext(_dbContextOptions))
        //    {
        //        Blog entity = db.Blogs.Find(message.BlogId);
        //        entity.Title = message.Title;
        //        entity.Content = message.Content;
        //        entity.Tags = string.Join(",", message.Tags);
        //        entity.LastModified = message.TimeStamp;
        //        entity.LastModifiedBy = message.EditorUserName;
        //        db.SaveChanges();
        //    }
        //}

        public void Handle(BlogDeleted message)
        {
            using (var db = new DevPortalDbContext(_dbContextOptions))
            {
                var entity = new Blog { Id = message.Id };
                db.Blogs.Attach(entity);
                db.Blogs.Remove(entity);
                db.SaveChanges();
            }
        }

    }
}
