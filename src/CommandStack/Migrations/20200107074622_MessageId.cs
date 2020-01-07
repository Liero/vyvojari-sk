using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevPortal.CommandStack.Migrations
{
    public partial class MessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_MessageId",
                table: "Events",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_MessageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Events");
        }
    }
}
