﻿// <auto-generated />
using System;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevPortal.QueryStack.Migrations
{
    [DbContext(typeof(DevPortalDbContext))]
    partial class DevPortalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DevPortal.QueryStack.Model.Activity", b =>
                {
                    b.Property<Guid>("ActivityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action")
                        .IsRequired();

                    b.Property<Guid>("ContentId");

                    b.Property<string>("ContentTitle");

                    b.Property<string>("ContentType")
                        .IsRequired();

                    b.Property<string>("ExternalUrl");

                    b.Property<Guid?>("Fragment");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<string>("UserName");

                    b.HasKey("ActivityId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.Blog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("ExternalUrl");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.ContentBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("LastModifiedBy");

                    b.HasKey("Id");

                    b.ToTable("ContentBase");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ContentBase");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.Tag", b =>
                {
                    b.Property<Guid>("ContentId");

                    b.Property<string>("Name");

                    b.HasKey("ContentId", "Name");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.ChildContent", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.ContentBase");

                    b.ToTable("ChildContent");

                    b.HasDiscriminator().HasValue("ChildContent");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.GenericContent", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.ContentBase");

                    b.Property<string>("Title");

                    b.ToTable("GenericContent");

                    b.HasDiscriminator().HasValue("GenericContent");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.ForumPost", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.ChildContent");

                    b.Property<Guid?>("RootId");

                    b.HasIndex("RootId");

                    b.ToTable("ForumPost");

                    b.HasDiscriminator().HasValue("ForumPost");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.NewsItemComment", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.ChildContent");

                    b.Property<Guid?>("RootId")
                        .HasColumnName("NewsItemComment_RootId");

                    b.HasIndex("RootId");

                    b.ToTable("NewsItemComment");

                    b.HasDiscriminator().HasValue("NewsItemComment");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.ForumThread", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.GenericContent");

                    b.Property<DateTime>("LastPosted");

                    b.Property<string>("LastPostedBy");

                    b.Property<string>("ParticipantsCsv");

                    b.Property<int>("PostsCount");

                    b.ToTable("ForumThread");

                    b.HasDiscriminator().HasValue("ForumThread");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.NewsItem", b =>
                {
                    b.HasBaseType("DevPortal.QueryStack.Model.GenericContent");

                    b.Property<int>("CommentsCount");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("Published");

                    b.ToTable("NewsItem");

                    b.HasDiscriminator().HasValue("NewsItem");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.Tag", b =>
                {
                    b.HasOne("DevPortal.QueryStack.Model.GenericContent")
                        .WithMany("Tags")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.ForumPost", b =>
                {
                    b.HasOne("DevPortal.QueryStack.Model.ForumThread", "Root")
                        .WithMany("Posts")
                        .HasForeignKey("RootId");
                });

            modelBuilder.Entity("DevPortal.QueryStack.Model.NewsItemComment", b =>
                {
                    b.HasOne("DevPortal.QueryStack.Model.NewsItem", "Root")
                        .WithMany("Comments")
                        .HasForeignKey("RootId");
                });
#pragma warning restore 612, 618
        }
    }
}
