using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev_V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                column: "AgeCategoryId",
                principalSchema: "dbo",
                principalTable: "tblAgeCategorys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries");

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
    }
}
