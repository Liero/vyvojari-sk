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
            : base(options){
        }

        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
