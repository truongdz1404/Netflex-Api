using Microsoft.AspNetCore.Identity;
using Netflex.Database;
using Netflex.Services.Implements;
using Microsoft.EntityFrameworkCore;
using Netflex.Database.Repositories.Implements;
using Netflex.Exceptions.Handler;
using Netflex.Models.Configs;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Netflex;
public static class DependencyInjection
{
    public static IServiceCollection AddServices
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
        services.AddScoped<IEmailSender, EmailService>();

        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        services.AddRazorPages();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.Configure<EmailConfig>(configuration.GetSection("EmailApiKey"));

        services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"];
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            options.CallbackPath = "/signin-google";
        })
        .AddFacebook(options =>
        {
            options.AppId = configuration["Authentication:Facebook:AppId"];
            options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
        });
        services.ConfigureApplicationCookie(options =>
  {
      options.SlidingExpiration = true;
      options.ExpireTimeSpan = TimeSpan.FromDays(7);
  });
        services.Configure<CookiePolicyOptions>(options =>
      {
          options.CheckConsentNeeded = context => false;
          options.MinimumSameSitePolicy = SameSiteMode.Lax;
      });
        return services;
    }
}


