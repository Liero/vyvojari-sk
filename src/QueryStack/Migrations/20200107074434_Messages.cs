using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevPortal.QueryStack.Migrations
{
    public partial class Messages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    UserName1 = table.Column<string>(nullable: false),
                    UserName2 = table.Column<string>(nullable: false),
                    LastContent = table.Column<string>(nullable: true),
                    LastPostedBy = table.Column<string>(nullable: false),
                    LastPosted = table.Column<DateTime>(nullable: false),
                    UnreadMessages = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => new { x.UserName1, x.UserName2 });
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RecipientUserName = table.Column<string>(nullable: false),
                    SenderUserName = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
