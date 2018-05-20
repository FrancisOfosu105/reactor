using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Reactor.Data.Migrations
{
    public partial class RemoveMessagePropertyFromNotificationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Notification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Notification",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
