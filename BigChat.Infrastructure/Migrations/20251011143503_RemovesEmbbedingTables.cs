using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovesEmbbedingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileEmbeddings");

            migrationBuilder.DropTable(
                name: "UserFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "DateTime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "DateTime2", nullable: true),
                    Name = table.Column<string>(type: "NVarChar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    Contents = table.Column<string>(type: "TEXT", nullable: false),
                    ContentsEmbedding = table.Column<string>(type: "Blob", maxLength: 384, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DateTime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileEmbeddings_UserFiles_UserFileId",
                        column: x => x.UserFileId,
                        principalTable: "UserFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileEmbeddings_UserFileId",
                table: "FileEmbeddings",
                column: "UserFileId");
        }
    }
}
