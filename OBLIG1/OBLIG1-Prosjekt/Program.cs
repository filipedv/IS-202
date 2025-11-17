using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using OBLIG1.Data;

var builder = WebApplication.CreateBuilder(args);

// ----- Connection string -----
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(connStr))
    throw new InvalidOperationException(
        "Connection string mangler. Sett ConnectionStrings__DefaultConnection i compose eller appsettings.json.");

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

// ----- Enkel autentisering med cookies + roller via claims -----
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";          // hvor man sendes ved behov for login
        options.AccessDeniedPath = "/Auth/AccessDenied";
        // evt. flere cookie-innstillinger her
    });

builder.Services.AddAuthorization();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----- Kjør migrasjoner ved oppstart -----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.MapStaticAssets();

app.UseRouting();
app.UseAuthentication();   // MÅ være før UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
