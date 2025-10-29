using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // for MariaDbServerVersion
using OBLIG1.Data; 
var builder = WebApplication.CreateBuilder(args);

// Hent connstr fra appsettings.json (lokalt) eller miljøvariabel (docker-compose)
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connStr))
        throw new InvalidOperationException(
            "Connection string mangler. Sett ConnectionStrings__DefaultConnection i compose eller appsettings.json.");

    // Bruk fast versjon for EF design-time (slipper å koble til DB)
    var serverVersion = new MariaDbServerVersion(new Version(11, 4, 0));
    options.UseMySql(connStr, serverVersion);
});

var app = builder.Build();

// Kjør migrasjoner automatisk ved oppstart
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

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();