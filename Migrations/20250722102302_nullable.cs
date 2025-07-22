using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblComments_tblFilms_FilmId",
                table: "tblComments");

            migrationBuilder.DropForeignKey(
                name: "FK_tblComments_tblSeries_SeriesId",
                table: "tblComments");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeriesId",
                table: "tblComments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                table: "tblComments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_tblComments_tblFilms_FilmId",
                table: "tblComments",
                column: "FilmId",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblComments_tblSeries_SeriesId",
                table: "tblComments",
                column: "SeriesId",
                principalTable: "tblSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblComments_tblFilms_FilmId",
                table: "tblComments");

            migrationBuilder.DropForeignKey(
                name: "FK_tblComments_tblSeries_SeriesId",
                table: "tblComments");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeriesId",
                table: "tblComments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                table: "tblComments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tblComments_tblFilms_FilmId",
                table: "tblComments",
                column: "FilmId",
                principalTable: "tblFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblComments_tblSeries_SeriesId",
                table: "tblComments",
                column: "SeriesId",
                principalTable: "tblSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
