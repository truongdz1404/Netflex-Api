using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class nullable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFavoriteFilms_tblFilms_FilmId",
                table: "tblFavoriteFilms");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFavoriteFilms_tblSeries_SeriesId",
                table: "tblFavoriteFilms");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeriesId",
                table: "tblFavoriteFilms",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                table: "tblFavoriteFilms",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFavoriteFilms_tblFilms_FilmId",
                table: "tblFavoriteFilms",
                column: "FilmId",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFavoriteFilms_tblSeries_SeriesId",
                table: "tblFavoriteFilms",
                column: "SeriesId",
                principalTable: "tblSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFavoriteFilms_tblFilms_FilmId",
                table: "tblFavoriteFilms");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFavoriteFilms_tblSeries_SeriesId",
                table: "tblFavoriteFilms");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeriesId",
                table: "tblFavoriteFilms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                table: "tblFavoriteFilms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tblFavoriteFilms_tblFilms_FilmId",
                table: "tblFavoriteFilms",
                column: "FilmId",
                principalTable: "tblFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblFavoriteFilms_tblSeries_SeriesId",
                table: "tblFavoriteFilms",
                column: "SeriesId",
                principalTable: "tblSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
