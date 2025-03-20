using Netflex;
using Netflex.Exceptions.Handler;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Configs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDependencyInjection(builder.Configuration);
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

app.UseAuthentication();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
