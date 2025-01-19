using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netflex.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "tblActors",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    About = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCountrys",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCountrys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblFilms",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Poster = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Trailer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionYear = table.Column<int>(type: "int", nullable: false),
                    HowLong = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblGenres",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblNotifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblRoles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblSeries",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Poster = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProductionYear = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmActors",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmActors", x => new { x.FilmId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblActors_ActorId",
                        column: x => x.ActorId,
                        principalSchema: "dbo",
                        principalTable: "tblActors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmCountrys",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmCountrys", x => new { x.FilmId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblFilmCountrys_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmCountrys_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmGenres",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmGenres", x => new { x.FilmId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_tblFilmGenres_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmGenres_tblGenres_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "tblGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRoleClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblRoleClaims_tblRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEpisodes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HowLong = table.Column<TimeSpan>(type: "time", nullable: false),
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEpisodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblEpisodes_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieActors",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieActors", x => new { x.SerieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblActors_ActorId",
                        column: x => x.ActorId,
                        principalSchema: "dbo",
                        principalTable: "tblActors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieCountrys",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieCountrys", x => new { x.SerieId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblSerieCountrys_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieCountrys_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieGenres",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieGenres", x => new { x.SerieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblGenres_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "tblGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblBlogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreaterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBlogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBlogs_tblUsers_CreaterId",
                        column: x => x.CreaterId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmFollows",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmFollows", x => new { x.FilmId, x.UserId });
                    table.ForeignKey(
                        name: "FK_tblFilmFollows_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmFollows_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblReviews",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreaterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblReviews_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblReviews_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblReviews_tblUsers_CreaterId",
                        column: x => x.CreaterId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieFollows",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SerieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieFollows", x => new { x.SerieId, x.UserId });
                    table.ForeignKey(
                        name: "FK_tblSerieFollows_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieFollows_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUserClaims_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserLogins",
                schema: "dbo",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tblUserLogins_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserNotifications",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserNotifications", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_tblUserNotifications_tblNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalSchema: "dbo",
                        principalTable: "tblNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUserNotifications_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserRoles",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_tblUserRoles_tblRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUserRoles_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserTokens",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_tblUserTokens_tblUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBlogs_CreaterId",
                schema: "dbo",
                table: "tblBlogs",
                column: "CreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_tblEpisodes_SerieId",
                schema: "dbo",
                table: "tblEpisodes",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmActors_ActorId",
                schema: "dbo",
                table: "tblFilmActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmCountrys_CountryId",
                schema: "dbo",
                table: "tblFilmCountrys",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmFollows_UserId",
                schema: "dbo",
                table: "tblFilmFollows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmGenres_GenreId",
                schema: "dbo",
                table: "tblFilmGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_CreaterId",
                schema: "dbo",
                table: "tblReviews",
                column: "CreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_FilmId",
                schema: "dbo",
                table: "tblReviews",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_SerieId",
                schema: "dbo",
                table: "tblReviews",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRoleClaims_RoleId",
                schema: "dbo",
                table: "tblRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "dbo",
                table: "tblRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieActors_ActorId",
                schema: "dbo",
                table: "tblSerieActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieCountrys_CountryId",
                schema: "dbo",
                table: "tblSerieCountrys",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieFollows_UserId",
                schema: "dbo",
                table: "tblSerieFollows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieGenres_GenreId",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserClaims_UserId",
                schema: "dbo",
                table: "tblUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserLogins_UserId",
                schema: "dbo",
                table: "tblUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserNotifications_NotificationId",
                schema: "dbo",
                table: "tblUserNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserRoles_RoleId",
                schema: "dbo",
                table: "tblUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "tblUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "tblUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBlogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblEpisodes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilmActors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilmCountrys",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilmFollows",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilmGenres",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblReviews",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblRoleClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblSerieActors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblSerieCountrys",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblSerieFollows",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblSerieGenres",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUserClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUserLogins",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUserNotifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUserTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblActors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblCountrys",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblGenres",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblSeries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblNotifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblUsers",
                schema: "dbo");
        }
    }
}
