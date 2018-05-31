using Microsoft.EntityFrameworkCore.Migrations;

namespace Reactor.Data.Migrations
{
    public partial class AddNewPropertiesToUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "User",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lives",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkAt",
                table: "User",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "User");

            migrationBuilder.DropColumn(
                name: "From",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Lives",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WorkAt",
                table: "User");
        }
    }
}
