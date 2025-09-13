using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraNgheCore;
using TraNgheCore.Models;
using DotNetEnv;

// Load .env variables BEFORE accessing configuration
Env.Load();

var builder = WebApplication.CreateBuilder(args);

var testKey = builder.Configuration["ReCaptcha:SiteKey"];
Console.WriteLine("ReCaptcha SiteKey loaded? " + (string.IsNullOrEmpty(testKey) ? "NO" : "YES"));

// Bind to strongly typed settings
builder.Services.Configure<ReCaptchaSettings>(builder.Configuration.GetSection("ReCaptcha"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Configure Cookie Expiration
//sets the cookie expiration time to 15 minutes and enables sliding expiration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;   //refresh timeout on activity
});

var app = builder.Build();

//�	Ensures the database is created and up-to-date with your EF Core models.
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
