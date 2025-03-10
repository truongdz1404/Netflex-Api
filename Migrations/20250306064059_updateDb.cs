using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class updateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries");

            migrationBuilder.AlterColumn<string>(
                name: "Poster",
                schema: "dbo",
                table: "tblSeries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "About",
                schema: "dbo",
                table: "tblSeries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GenreId1",
                schema: "dbo",
                table: "tblSerieGenres",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GenreId2",
                schema: "dbo",
                table: "tblSerieGenres",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieGenres",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId1",
                schema: "dbo",
                table: "tblSerieCountries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieCountries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActorId1",
                schema: "dbo",
                table: "tblSerieActors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieActors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmGenres",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId1",
                schema: "dbo",
                table: "tblFilmCountries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmCountries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActorId1",
                schema: "dbo",
                table: "tblFilmActors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmActors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieGenres_GenreId1",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieGenres_GenreId2",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId2");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieGenres_SerieId1",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "SerieId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieCountries_CountryId1",
                schema: "dbo",
                table: "tblSerieCountries",
                column: "CountryId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieCountries_SerieId1",
                schema: "dbo",
                table: "tblSerieCountries",
                column: "SerieId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieActors_ActorId1",
                schema: "dbo",
                table: "tblSerieActors",
                column: "ActorId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieActors_SerieId1",
                schema: "dbo",
                table: "tblSerieActors",
                column: "SerieId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmGenres_FilmId1",
                schema: "dbo",
                table: "tblFilmGenres",
                column: "FilmId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmCountries_CountryId1",
                schema: "dbo",
                table: "tblFilmCountries",
                column: "CountryId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmCountries_FilmId1",
                schema: "dbo",
                table: "tblFilmCountries",
                column: "FilmId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmActors_ActorId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "ActorId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmActors_FilmId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "FilmId1");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFilmActors_tblActors_ActorId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "ActorId1",
                principalSchema: "dbo",
                principalTable: "tblActors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFilmActors_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "FilmId1",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFilmCountries_tblCountrys_CountryId1",
                schema: "dbo",
                table: "tblFilmCountries",
                column: "CountryId1",
                principalSchema: "dbo",
                principalTable: "tblCountrys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFilmCountries_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmCountries",
                column: "FilmId1",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblFilmGenres_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmGenres",
                column: "FilmId1",
                principalSchema: "dbo",
                principalTable: "tblFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieActors_tblActors_ActorId1",
                schema: "dbo",
                table: "tblSerieActors",
                column: "ActorId1",
                principalSchema: "dbo",
                principalTable: "tblActors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieActors_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieActors",
                column: "SerieId1",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieCountries_tblCountrys_CountryId1",
                schema: "dbo",
                table: "tblSerieCountries",
                column: "CountryId1",
                principalSchema: "dbo",
                principalTable: "tblCountrys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieCountries_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieCountries",
                column: "SerieId1",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieGenres_tblGenres_GenreId1",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId1",
                principalSchema: "dbo",
                principalTable: "tblGenres",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieGenres_tblGenres_GenreId2",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId2",
                principalSchema: "dbo",
                principalTable: "tblGenres",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSerieGenres_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "SerieId1",
                principalSchema: "dbo",
                principalTable: "tblSeries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                column: "AgeCategoryId",
                principalSchema: "dbo",
                principalTable: "tblAgeCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblFilmActors_tblActors_ActorId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFilmActors_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFilmCountries_tblCountrys_CountryId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFilmCountries_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_tblFilmGenres_tblFilms_FilmId1",
                schema: "dbo",
                table: "tblFilmGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieActors_tblActors_ActorId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieActors_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieCountries_tblCountrys_CountryId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieCountries_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieGenres_tblGenres_GenreId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieGenres_tblGenres_GenreId2",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSerieGenres_tblSeries_SerieId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieGenres_GenreId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieGenres_GenreId2",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieGenres_SerieId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieCountries_CountryId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieCountries_SerieId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieActors_ActorId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropIndex(
                name: "IX_tblSerieActors_SerieId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropIndex(
                name: "IX_tblFilmGenres_FilmId1",
                schema: "dbo",
                table: "tblFilmGenres");

            migrationBuilder.DropIndex(
                name: "IX_tblFilmCountries_CountryId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropIndex(
                name: "IX_tblFilmCountries_FilmId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropIndex(
                name: "IX_tblFilmActors_ActorId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.DropIndex(
                name: "IX_tblFilmActors_FilmId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.DropColumn(
                name: "GenreId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropColumn(
                name: "GenreId2",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropColumn(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieGenres");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropColumn(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieCountries");

            migrationBuilder.DropColumn(
                name: "ActorId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropColumn(
                name: "SerieId1",
                schema: "dbo",
                table: "tblSerieActors");

            migrationBuilder.DropColumn(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmGenres");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropColumn(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmCountries");

            migrationBuilder.DropColumn(
                name: "ActorId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.DropColumn(
                name: "FilmId1",
                schema: "dbo",
                table: "tblFilmActors");

            migrationBuilder.AlterColumn<string>(
                name: "Poster",
                schema: "dbo",
                table: "tblSeries",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid>(
                name: "AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "About",
                schema: "dbo",
                table: "tblSeries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                column: "AgeCategoryId",
                principalSchema: "dbo",
                principalTable: "tblAgeCategorys",
                principalColumn: "Id");
        }
    }
}
