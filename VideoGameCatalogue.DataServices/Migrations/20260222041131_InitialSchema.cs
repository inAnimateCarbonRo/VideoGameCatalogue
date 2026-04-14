using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameCatalogue.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genre",
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
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserScore = table.Column<int>(type: "int", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOnDts = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGame", x => x.Id);
                    table.CheckConstraint("CK_VideoGame_UserScore_Range", "[UserScore] >= 0 AND [UserScore] <= 100");
                });

            migrationBuilder.CreateTable(
                name: "VideoGameGenre",
                columns: table => new
                {
                    VideoGameId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGameGenre", x => new { x.VideoGameId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_VideoGameGenre_Genre_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoGameGenre_VideoGame_VideoGameId",
                        column: x => x.VideoGameId,
                        principalTable: "VideoGame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genre_Name",
                table: "Genre",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoGameGenre_GenreId",
                table: "VideoGameGenre",
                column: "GenreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGameGenre");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "VideoGame");
        }
    }
}
