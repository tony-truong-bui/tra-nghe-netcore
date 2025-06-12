using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraNgheCore;
using TraNgheCore.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

//•	Ensures the database is created and up-to-date with your EF Core models.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // This will create the database and apply any migrations
}

//Call role and admin seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAndAdminAsync(services);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


await app.RunAsync();
