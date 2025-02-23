using Netflex;
using Netflex.Exceptions.Handler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Netflex.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NetflexContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped(typeof(NetflexContext));

builder.Services.AddControllersWithViews();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
