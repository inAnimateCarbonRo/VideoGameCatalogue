using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameCatalogue.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformsCompaniesCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CoverImageBytes",
                table: "VideoGame",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageContentType",
                table: "VideoGame",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeveloperId",
                table: "VideoGame",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "VideoGame",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOnDts = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOnDts = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platform", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoGamePlatform",
                columns: table => new
                {
                    VideoGameId = table.Column<int>(type: "int", nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGamePlatform", x => new { x.VideoGameId, x.PlatformId });
                    table.ForeignKey(
                        name: "FK_VideoGamePlatform_Platform_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platform",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoGamePlatform_VideoGame_VideoGameId",
                        column: x => x.VideoGameId,
                        principalTable: "VideoGame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoGame_DeveloperId",
                table: "VideoGame",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGame_PublisherId",
                table: "VideoGame",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platform_Name",
                table: "Platform",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoGamePlatform_PlatformId",
                table: "VideoGamePlatform",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoGame_Company_DeveloperId",
                table: "VideoGame",
                column: "DeveloperId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoGame_Company_PublisherId",
                table: "VideoGame",
                column: "PublisherId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoGame_Company_DeveloperId",
                table: "VideoGame");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoGame_Company_PublisherId",
                table: "VideoGame");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "VideoGamePlatform");

            migrationBuilder.DropTable(
                name: "Platform");

            migrationBuilder.DropIndex(
                name: "IX_VideoGame_DeveloperId",
                table: "VideoGame");

            migrationBuilder.DropIndex(
                name: "IX_VideoGame_PublisherId",
                table: "VideoGame");

            migrationBuilder.DropColumn(
                name: "CoverImageBytes",
                table: "VideoGame");

            migrationBuilder.DropColumn(
                name: "CoverImageContentType",
                table: "VideoGame");

            migrationBuilder.DropColumn(
                name: "DeveloperId",
                table: "VideoGame");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "VideoGame");
        }
    }
}
