using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Models; // Add MvcMovies.Data later as well
using MvcMovie.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found.");

builder.Services.AddDbContext<MvcMovieContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MvcMovieContext>(); // Register Identity services in Program.cs — builder.Services.AddDefaultIdentity<IdentityUser>(...) and wire it to your DbContext

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // Add authentication middleware to the request pipeline before authorization middleware.
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages(); // Map Razor Pages for Identity UI, which includes the login page and other account management pages.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}") // Default routing format
    .WithStaticAssets();


app.Run();
