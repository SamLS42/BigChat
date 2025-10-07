using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Messages",
                newName: "ThinkContent");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ThinkContent",
                table: "Messages",
                newName: "Text");
        }
    }
}
