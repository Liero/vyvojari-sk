using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevPortal.QueryStack.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(nullable: false),
                    ContentId = table.Column<Guid>(nullable: false),
                    Fragment = table.Column<Guid>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    ContentTitle = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    ExternalUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    ExternalUrl = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ThreadId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    LastPosted = table.Column<DateTime>(nullable: true),
                    LastPostedBy = table.Column<string>(nullable: true),
                    PostsCount = table.Column<int>(nullable: true),
                    ParticipantsCsv = table.Column<string>(nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: true),
                    CommentsCount = table.Column<int>(nullable: true),
                    NewsItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBase_ContentBase_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "ContentBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentBase_ContentBase_NewsItemId",
                        column: x => x.NewsItemId,
                        principalTable: "ContentBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    ContentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.ContentId, x.Name });
                    table.ForeignKey(
                        name: "FK_Tags_ContentBase_ContentId",
                        column: x => x.ContentId,
                        principalTable: "ContentBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentBase_ThreadId",
                table: "ContentBase",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBase_NewsItemId",
                table: "ContentBase",
                column: "NewsItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ContentBase");
        }
    }
}
