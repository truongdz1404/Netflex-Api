using Netflex;
using Netflex.Exceptions.Handler;

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"<PATH_TO_CREDENTIALS_FILE");

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
