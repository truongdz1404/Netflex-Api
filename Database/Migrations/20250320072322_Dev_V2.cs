using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFollows_tblFilms_FilmId",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFollows_tblSeries_SerieId",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblFollows",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.AlterColumn<Guid>(
                name: "SerieId",
                schema: "dbo",
                table: "tblFollows",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                schema: "dbo",
                table: "tblFollows",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "dbo",
                table: "tblFollows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblFollows",
                schema: "dbo",
                table: "tblFollows",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_FollowerId",
                schema: "dbo",
                table: "tblFollows",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFollows_tblFilms_FilmId",
                schema: "dbo",
                table: "tblFollows",
                column: "FilmId",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFollows_tblSeries_SerieId",
                schema: "dbo",
                table: "tblFollows",
                column: "SerieId",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFollows_tblFilms_FilmId",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFollows_tblSeries_SerieId",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblFollows",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropIndex(
                name: "IX_tblFollows_FollowerId",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "dbo",
                table: "tblFollows");

            migrationBuilder.AlterColumn<Guid>(
                name: "SerieId",
                schema: "dbo",
                table: "tblFollows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FilmId",
                schema: "dbo",
                table: "tblFollows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblFollows",
                schema: "dbo",
                table: "tblFollows",
                columns: new[] { "FollowerId", "FilmId", "SerieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_tblFollows_tblFilms_FilmId",
                schema: "dbo",
                table: "tblFollows",
                column: "FilmId",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblFollows_tblSeries_SerieId",
                schema: "dbo",
                table: "tblFollows",
                column: "SerieId",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
