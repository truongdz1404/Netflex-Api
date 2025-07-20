using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Netflex;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Database.Repositories.Implements;
using Netflex.Exceptions.Handler;
using Netflex.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Ensure controllers are found
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero // Reduce clock skew to avoid token expiration issues
    };

    // Add events for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Netflex API", Version = "v1" });

    // Add JWT Auth - Fixed
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (without 'Bearer ' prefix)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDependencyInjection(builder.Configuration);

// Add Identity configuration if not already in AddDependencyInjection
// Uncomment if needed:
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// 
// builder.Services.AddIdentity<User, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();
// CRITICAL: Correct middleware order
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication(); // Must be after UseRouting and before UseAuthorization
app.UseAuthorization();

app.MapGet("/", () => "SpaceY API is running!");
app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");

// Debug: List all registered routes
if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/routes", (IServiceProvider serviceProvider) =>
    {
        var endpointDataSource = serviceProvider.GetService<EndpointDataSource>();
        var routes = endpointDataSource?.Endpoints.Select(e => new
        {
            DisplayName = e.DisplayName,
            RoutePattern = e.Metadata.GetMetadata<RouteAttribute>()?.Template
        }).ToList();

        return Results.Ok(routes);
    });
}

app.Run();