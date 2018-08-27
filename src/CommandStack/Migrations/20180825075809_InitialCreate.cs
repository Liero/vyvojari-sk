using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DevPortal.CommandStack.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlogId = table.Column<Guid>(nullable: true),
                    EventType = table.Column<string>(nullable: false),
                    ForumThreadId = table.Column<Guid>(nullable: true),
                    NewsItemId = table.Column<Guid>(nullable: true),
                    SerializedEvent = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_BlogId",
                table: "Events",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventType",
                table: "Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ForumThreadId",
                table: "Events",
                column: "ForumThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_NewsItemId",
                table: "Events",
                column: "NewsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TimeStamp",
                table: "Events",
                column: "TimeStamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
