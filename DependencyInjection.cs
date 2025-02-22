using Microsoft.AspNetCore.Identity;
using Netflex.Database;
using Netflex.Services.Implements;
using Microsoft.EntityFrameworkCore;
using Netflex.Database.Repositories.Implements;

namespace Netflex;
public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
        });
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase =
                options.Password.RequireLowercase =
                    options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider;
        }
        ).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddHttpClient();
        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IStorageService>(s => new StorageService());

        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));


        return services;
    }
}