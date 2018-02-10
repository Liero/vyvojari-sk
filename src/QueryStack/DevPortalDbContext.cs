using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack
{
    public class DevPortalDbContext : DbContext
    {
        public DevPortalDbContext(DbContextOptions<DevPortalDbContext> options)
            : base(options)
        {
        }

        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<ContentBase> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GenericContent>(entity =>
            {
                entity.HasMany(e => e.Tags)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NewsItem>(entity =>
            {
                entity.HasMany(e => e.Comments)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ForumThread>(entity =>
            {
                entity.HasMany(e => e.Posts)
                    .WithOne(e => e.Thread)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.ContentId,
                    e.Name,
                });
            });
        }
    }
}
