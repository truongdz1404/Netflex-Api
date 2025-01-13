using Netflex.Persistance;
using Netflex.Services.Implements;

namespace Netflex;
public static class DependencyInjection
{
    public static IServiceCollection AddServices
        (this IServiceCollection services, IConfiguration configuration)
    {


        // var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<IEmailService, EmailService>();


        // services.AddDbContext<ApplicationDbContext>((sp, options) =>
        // {
        //     options.UseSqlServer(connectionString);
        // });
        // services.AddHttpClient();
        // services.AddScoped<ApplicationDbContext>();

        // services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
        //     .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));


        return services;
    }
}


