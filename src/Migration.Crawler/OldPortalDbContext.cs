using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Migration.Crawler
{
    public class OldPortalDbContext: DbContext
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NewsItem> News { get; set; }
        public DbSet<ForumCategory> Categories { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public DbSet<Blog> Blogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=DevPortal_Old;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NewsItem>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<Comment>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<ForumCategory>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<ForumThread>().Property(p => p.Id).ValueGeneratedNever();
            //modelBuilder.Entity<Answer>().Property(p => p.Id).ValueGeneratedNever();
        }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Author { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set;  }

        public string Url { get; set; }
        public int Views { get; set; }
    }

    public class NewsItem
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }
        public string Text { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }

    public class Answer
    {
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public int Id { get; set; }
        public int IdWithinPost { get; set; }

    }


    public class ForumThread
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public List<Answer> Answers { get; set; }
        public int ForumCategoryId { get; set; }
    }

    public class ForumCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}
