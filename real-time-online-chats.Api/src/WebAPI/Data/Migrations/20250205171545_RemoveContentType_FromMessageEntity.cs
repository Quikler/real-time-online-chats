using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace real_time_online_chats.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContentType_FromMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
