using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class altComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_tblFilms_FilmId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_tblSeries_SeriesId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_tblUsers_UserId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "tblComments");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "tblComments",
                newName: "IX_tblComments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_SeriesId",
                table: "tblComments",
                newName: "IX_tblComments_SeriesId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_FilmId",
                table: "tblComments",
                newName: "IX_tblComments_FilmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblComments",
                table: "tblComments",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_tblComments_tblUsers_UserId",
                table: "tblComments",
                column: "UserId",
                principalTable: "tblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropForeignKey(
                name: "FK_tblComments_tblUsers_UserId",
                table: "tblComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblComments",
                table: "tblComments");

            migrationBuilder.RenameTable(
                name: "tblComments",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_tblComments_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_tblComments_SeriesId",
                table: "Comments",
                newName: "IX_Comments_SeriesId");

            migrationBuilder.RenameIndex(
                name: "IX_tblComments_FilmId",
                table: "Comments",
                newName: "IX_Comments_FilmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_tblFilms_FilmId",
                table: "Comments",
                column: "FilmId",
                principalTable: "tblFilms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_tblSeries_SeriesId",
                table: "Comments",
                column: "SeriesId",
                principalTable: "tblSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_tblUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "tblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
