using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Reactor.Data.Migrations
{
    public partial class AddUserSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotifyWhenUserAcceptFriendRequest = table.Column<bool>(nullable: false),
                    NotifyWhenUserCommentOnPost = table.Column<bool>(nullable: false),
                    NotifyWhenUserFollow = table.Column<bool>(nullable: false),
                    NotifyWhenUserLikePost = table.Column<bool>(nullable: false),
                    NotifyWhenUserRejectFriendRequest = table.Column<bool>(nullable: false),
                    NotifyWhenUserSendFriendRequest = table.Column<bool>(nullable: false),
                    NotifyWhenUserUnFollow = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSetting_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSetting_UserId",
                table: "UserSetting",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSetting");
        }
    }
}
