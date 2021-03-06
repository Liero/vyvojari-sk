﻿using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack
{
    public class DevPortalDbContext : DbContext
    {
        public DevPortalDbContext(DbContextOptions<DevPortalDbContext> options)
            : base(options)
        {
        }

        public DbSet<DenormalizerState> Denormalizers { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<ContentBase> Contents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddDateTimeUtcConversion();

            modelBuilder.Entity<DenormalizerState>(entity =>
            {
                entity.HasKey(e => e.Key);
                entity.Property(e => e.Key).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<ContentBase>(entity => {
                entity.ToTable("ContentBase");
            });

            modelBuilder.Entity<GenericContent>(entity =>
            {
                entity.HasMany(e => e.Tags)
                    .WithOne()
                    .HasForeignKey(e => e.ContentId);
            });

            modelBuilder.Entity<ChildContent>(entity =>
            {
                entity.HasOne(e => e.Root)
                .WithMany()
                .HasForeignKey(e => e.RootId);
            });

            modelBuilder.Entity<NewsItem>(entity =>
            {
                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.Root)
                    .HasForeignKey(e => e.RootId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.CommentsCount).HasColumnName("ChildrenCount");
            });

            modelBuilder.Entity<ForumThread>(entity =>
            {
                entity.HasMany(e => e.Posts)
                    .WithOne(e => e.Root)
                    .HasForeignKey(e => e.RootId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.PostsCount).HasColumnName("ChildrenCount");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.ContentId,
                    e.Name,
                });
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.Property(e => e.ContentType).IsRequired();
                entity.Property(e => e.Action).IsRequired();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.RecipientUserName).IsRequired();
                entity.Property(e => e.SenderUserName).IsRequired();
                // entity.HasIndex(e => new { e.RecipientUserName, e.SenderUserName })
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => new { e.UserName1, e.UserName2 });
                entity.Property(e => e.LastPostedBy).IsRequired();
            });
        }   
    }
}
