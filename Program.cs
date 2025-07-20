using Netflex;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Database.Repositories.Implements;
using Netflex.Exceptions.Handler;
using Netflex.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionManager>();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapGet("/", () => "SpaceY API is running!");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 
app.MapHub<NotificationHub>("/notification-hub");

app.Run();
