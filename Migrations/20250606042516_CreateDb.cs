using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Netflex.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblActors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Photo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblActors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblAgeCategorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAgeCategorys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblCountrys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCountrys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblGenres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblFilms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Poster = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Path = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Trailer = table.Column<string>(type: "text", nullable: true),
                    ProductionYear = table.Column<int>(type: "integer", nullable: false),
                    AgeCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    HowLong = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFilms_tblAgeCategorys_AgeCategoryId",
                        column: x => x.AgeCategoryId,
                        principalTable: "tblAgeCategorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Poster = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductionYear = table.Column<int>(type: "integer", nullable: false),
                    AgeCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                        column: x => x.AgeCategoryId,
                        principalTable: "tblAgeCategorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblRoleClaims_tblRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblBlogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: false),
                    Thumbnail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaterId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBlogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBlogs_tblUsers_CreaterId",
                        column: x => x.CreaterId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblUserClaims_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tblUserLogins_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserNotifications",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    HaveRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserNotifications", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_tblUserNotifications_tblNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "tblNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUserNotifications_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_tblUserRoles_tblRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUserRoles_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_tblUserTokens_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmActors",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmActors", x => new { x.FilmId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblActors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "tblActors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmCountries",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmCountries", x => new { x.FilmId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmGenres",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmGenres", x => new { x.FilmId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_tblFilmGenres_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmGenres_tblGenres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "tblGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEpisodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Path = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    HowLong = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEpisodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblEpisodes_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFollows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerId = table.Column<string>(type: "text", nullable: false),
                    FilmId = table.Column<Guid>(type: "uuid", nullable: true),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: true),
                    FollowedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblFollows_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblFollows_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblFollows_tblUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreaterId = table.Column<string>(type: "text", nullable: false),
                    FilmId = table.Column<Guid>(type: "uuid", nullable: true),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblReviews_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblReviews_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblReviews_tblUsers_CreaterId",
                        column: x => x.CreaterId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieActors",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieActors", x => new { x.SerieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblActors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "tblActors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieCountries",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieCountries", x => new { x.SerieId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieGenres",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieGenres", x => new { x.SerieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblGenres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "tblGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBlogs_CreaterId",
                table: "tblBlogs",
                column: "CreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_tblEpisodes_SerieId",
                table: "tblEpisodes",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmActors_ActorId",
                table: "tblFilmActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmCountries_CountryId",
                table: "tblFilmCountries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmGenres_GenreId",
                table: "tblFilmGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilms_AgeCategoryId",
                table: "tblFilms",
                column: "AgeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_FilmId",
                table: "tblFollows",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_FollowerId",
                table: "tblFollows",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_SerieId",
                table: "tblFollows",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_CreaterId",
                table: "tblReviews",
                column: "CreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_FilmId",
                table: "tblReviews",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_SerieId",
                table: "tblReviews",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRoleClaims_RoleId",
                table: "tblRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "tblRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieActors_ActorId",
                table: "tblSerieActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieCountries_CountryId",
                table: "tblSerieCountries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieGenres_GenreId",
                table: "tblSerieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSeries_AgeCategoryId",
                table: "tblSeries",
                column: "AgeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserClaims_UserId",
                table: "tblUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserLogins_UserId",
                table: "tblUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserNotifications_NotificationId",
                table: "tblUserNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserRoles_RoleId",
                table: "tblUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "tblUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "tblUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBlogs");

            migrationBuilder.DropTable(
                name: "tblEpisodes");

            migrationBuilder.DropTable(
                name: "tblFilmActors");

            migrationBuilder.DropTable(
                name: "tblFilmCountries");

            migrationBuilder.DropTable(
                name: "tblFilmGenres");

            migrationBuilder.DropTable(
                name: "tblFollows");

            migrationBuilder.DropTable(
                name: "tblReviews");

            migrationBuilder.DropTable(
                name: "tblRoleClaims");

            migrationBuilder.DropTable(
                name: "tblSerieActors");

            migrationBuilder.DropTable(
                name: "tblSerieCountries");

            migrationBuilder.DropTable(
                name: "tblSerieGenres");

            migrationBuilder.DropTable(
                name: "tblUserClaims");

            migrationBuilder.DropTable(
                name: "tblUserLogins");

            migrationBuilder.DropTable(
                name: "tblUserNotifications");

            migrationBuilder.DropTable(
                name: "tblUserRoles");

            migrationBuilder.DropTable(
                name: "tblUserTokens");

            migrationBuilder.DropTable(
                name: "tblFilms");

            migrationBuilder.DropTable(
                name: "tblActors");

            migrationBuilder.DropTable(
                name: "tblCountrys");

            migrationBuilder.DropTable(
                name: "tblGenres");

            migrationBuilder.DropTable(
                name: "tblSeries");

            migrationBuilder.DropTable(
                name: "tblNotifications");

            migrationBuilder.DropTable(
                name: "tblRoles");

            migrationBuilder.DropTable(
                name: "tblUsers");

            migrationBuilder.DropTable(
                name: "tblAgeCategorys");
        }
    }
}
