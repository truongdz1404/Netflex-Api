using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev_V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblReviews_tblFilms_FilmId",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_tblReviews_tblSeries_SerieId",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblReviews",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "SerieId",
                schema: "dbo",
                table: "tblReviews",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                schema: "dbo",
                table: "tblReviews",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "dbo",
                table: "tblReviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblReviews",
                schema: "dbo",
                table: "tblReviews",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_FilmId",
                schema: "dbo",
                table: "tblReviews",
                column: "FilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblReviews_tblFilms_FilmId",
                schema: "dbo",
                table: "tblReviews",
                column: "FilmId",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblReviews_tblSeries_SerieId",
                schema: "dbo",
                table: "tblReviews",
                column: "SerieId",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblReviews_tblFilms_FilmId",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_tblReviews_tblSeries_SerieId",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblReviews",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropIndex(
                name: "IX_tblReviews_FilmId",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "dbo",
                table: "tblReviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "SerieId",
                schema: "dbo",
                table: "tblReviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                schema: "dbo",
                table: "tblReviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblReviews",
                schema: "dbo",
                table: "tblReviews",
                columns: new[] { "FilmId", "CreaterId", "SerieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_tblReviews_tblFilms_FilmId",
                schema: "dbo",
                table: "tblReviews",
                column: "FilmId",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblReviews_tblSeries_SerieId",
                schema: "dbo",
                table: "tblReviews",
                column: "SerieId",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
