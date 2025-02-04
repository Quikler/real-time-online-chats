using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace real_time_online_chats.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatuses_ToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CasualStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GamingStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoodStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkStatus",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActivityStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CasualStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GamingStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MoodStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkStatus",
                table: "AspNetUsers");
        }
    }
}
