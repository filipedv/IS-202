using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;

var builder = WebApplication.CreateBuilder(args);

// ----- Connection string -----
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(connStr))
{
    throw new InvalidOperationException(
        "Connection string mangler. Sett ConnectionStrings__DefaultConnection i compose eller appsettings.json.");
}

// ----- DbContext (MariaDB + retry) -----
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var serverVersion = new MariaDbServerVersion(new Version(11, 4, 0));
    options.UseMySql(connStr, serverVersion, mySql =>
    {
        mySql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });
});

builder.Services.AddScoped<IObstacleService, ObstacleService>();

// ----- Identity (ApplicationUser + roller) -----
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // Her kan du legge til strengere passordregler osv. om ønskelig
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders(); // <-- VIKTIG: denne gjør at "Default"-token provider finnes

// Registrer cookie-skjemaene Identity bruker (Identity.Application osv.)
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme       = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----- Migrasjoner + seeding ved oppstart -----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();          // kjør migrasjoner

    // Seed roller + brukere (pilot1, pilot2, registerforer, admin)
    await SeedData.InitializeAsync(services);
}

// ----- Middleware-pipeline -----
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    // valgfritt i dev:
    // app.UseDeveloperExceptionPage();
}

app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();   // må komme før UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
