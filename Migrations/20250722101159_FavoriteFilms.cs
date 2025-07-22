using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteFilms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblFavoriteFilms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFavoriteFilms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFavoriteFilms_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFavoriteFilms_tblSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFavoriteFilms_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblFavoriteFilms_FilmId",
                table: "tblFavoriteFilms",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFavoriteFilms_SeriesId",
                table: "tblFavoriteFilms",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFavoriteFilms_UserId",
                table: "tblFavoriteFilms",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblFavoriteFilms");
        }
    }
}
