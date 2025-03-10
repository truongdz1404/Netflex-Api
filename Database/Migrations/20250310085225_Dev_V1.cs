using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Netflex.Database.Migrations
{
    /// <inheritdoc />
    public partial class Dev_V1 : Migration
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
                schema: "dbo",
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
                schema: "dbo",
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
                schema: "dbo",
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
                schema: "dbo",
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
                schema: "dbo",
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
                schema: "dbo",
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
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Poster = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                        principalSchema: "dbo",
                        principalTable: "tblAgeCategorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblSeries",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Poster = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductionYear = table.Column<int>(type: "integer", nullable: false),
                    AgeCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblSeries_tblAgeCategorys_AgeCategoryId",
                        column: x => x.AgeCategoryId,
                        principalSchema: "dbo",
                        principalTable: "tblAgeCategorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRoleClaims",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "tblRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblBlogs",
                schema: "dbo",
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFilmActors",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    FilmId1 = table.Column<Guid>(type: "uuid", nullable: true)
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
                        name: "FK_tblFilmActors_tblActors_ActorId1",
                        column: x => x.ActorId1,
                        principalSchema: "dbo",
                        principalTable: "tblActors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmActors_tblFilms_FilmId1",
                        column: x => x.FilmId1,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblFilmCountries",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    FilmId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFilmCountries", x => new { x.FilmId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblCountrys_CountryId1",
                        column: x => x.CountryId1,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFilmCountries_tblFilms_FilmId1",
                        column: x => x.FilmId1,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblFilmGenres",
                schema: "dbo",
                columns: table => new
                {
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilmId1 = table.Column<Guid>(type: "uuid", nullable: true)
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
                        name: "FK_tblFilmGenres_tblFilms_FilmId1",
                        column: x => x.FilmId1,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblFilmGenres_tblGenres_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "tblGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEpisodes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HowLong = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "tblFollows",
                schema: "dbo",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "text", nullable: false),
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFollows", x => new { x.FollowerId, x.FilmId, x.SerieId });
                    table.ForeignKey(
                        name: "FK_tblFollows_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFollows_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFollows_tblUsers_FollowerId",
                        column: x => x.FollowerId,
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
                    CreaterId = table.Column<string>(type: "text", nullable: false),
                    FilmId = table.Column<Guid>(type: "uuid", nullable: false),
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblReviews", x => new { x.FilmId, x.CreaterId, x.SerieId });
                    table.ForeignKey(
                        name: "FK_tblReviews_tblFilms_FilmId",
                        column: x => x.FilmId,
                        principalSchema: "dbo",
                        principalTable: "tblFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblReviews_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblReviews_tblUsers_CreaterId",
                        column: x => x.CreaterId,
                        principalSchema: "dbo",
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblSerieActors",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    SerieId1 = table.Column<Guid>(type: "uuid", nullable: true)
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
                        name: "FK_tblSerieActors_tblActors_ActorId1",
                        column: x => x.ActorId1,
                        principalSchema: "dbo",
                        principalTable: "tblActors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieActors_tblSeries_SerieId1",
                        column: x => x.SerieId1,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblSerieCountries",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    SerieId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSerieCountries", x => new { x.SerieId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblCountrys_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblCountrys_CountryId1",
                        column: x => x.CountryId1,
                        principalSchema: "dbo",
                        principalTable: "tblCountrys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieCountries_tblSeries_SerieId1",
                        column: x => x.SerieId1,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tblSerieGenres",
                schema: "dbo",
                columns: table => new
                {
                    SerieId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    GenreId2 = table.Column<Guid>(type: "uuid", nullable: true),
                    SerieId1 = table.Column<Guid>(type: "uuid", nullable: true)
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
                        name: "FK_tblSerieGenres_tblGenres_GenreId1",
                        column: x => x.GenreId1,
                        principalSchema: "dbo",
                        principalTable: "tblGenres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblGenres_GenreId2",
                        column: x => x.GenreId2,
                        principalSchema: "dbo",
                        principalTable: "tblGenres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblSeries_SerieId",
                        column: x => x.SerieId,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSerieGenres_tblSeries_SerieId1",
                        column: x => x.SerieId1,
                        principalSchema: "dbo",
                        principalTable: "tblSeries",
                        principalColumn: "Id");
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
                name: "IX_tblFilmActors_ActorId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "ActorId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmActors_FilmId1",
                schema: "dbo",
                table: "tblFilmActors",
                column: "FilmId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmCountries_CountryId",
                schema: "dbo",
                table: "tblFilmCountries",
                column: "CountryId");

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
                name: "IX_tblFilmGenres_FilmId1",
                schema: "dbo",
                table: "tblFilmGenres",
                column: "FilmId1");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilmGenres_GenreId",
                schema: "dbo",
                table: "tblFilmGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFilms_AgeCategoryId",
                schema: "dbo",
                table: "tblFilms",
                column: "AgeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_FilmId",
                schema: "dbo",
                table: "tblFollows",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_tblFollows_SerieId",
                schema: "dbo",
                table: "tblFollows",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReviews_CreaterId",
                schema: "dbo",
                table: "tblReviews",
                column: "CreaterId");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblSerieActors_ActorId",
                schema: "dbo",
                table: "tblSerieActors",
                column: "ActorId");

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
                name: "IX_tblSerieCountries_CountryId",
                schema: "dbo",
                table: "tblSerieCountries",
                column: "CountryId");

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
                name: "IX_tblSerieGenres_GenreId",
                schema: "dbo",
                table: "tblSerieGenres",
                column: "GenreId");

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
                name: "IX_tblSeries_AgeCategoryId",
                schema: "dbo",
                table: "tblSeries",
                column: "AgeCategoryId");

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
                unique: true);
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
                name: "tblFilmCountries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFilmGenres",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblFollows",
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
                name: "tblSerieCountries",
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

            migrationBuilder.DropTable(
                name: "tblAgeCategorys",
                schema: "dbo");
        }
    }
}
