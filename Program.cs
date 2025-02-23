using Netflex;
using Netflex.Exceptions.Handler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Netflex.Models;
using Netflex.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped(typeof(ApplicationDbContext));

builder.Services.AddControllersWithViews();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

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
