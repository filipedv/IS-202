using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // MariaDbServerVersion
using OBLIG1.Data;

var builder = WebApplication.CreateBuilder(args);

// Hent connection string (appsettings eller miljøvariabel i Docker)
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(connStr))
    throw new InvalidOperationException(
        "Connection string mangler. Sett ConnectionStrings__DefaultConnection i compose eller appsettings.json.");

// DbContext (MariaDB) + retry
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

// Identity med roller
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>() // VIKTIG for Roller
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // hvis du evt. bruker Identity UI senere

var app = builder.Build();

// --- Kjør migrasjoner og seed roller ved oppstart ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1) Migrer DB
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // 2) Seed roller
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "Pilot", "Registerforer" }) // "Registerforer" = ASCII-variant av "Registerfører"
    {
        if (!await roleMgr.RoleExistsAsync(roleName))
            await roleMgr.CreateAsync(new IdentityRole(roleName));
    }

    // (Valgfritt) Seed test-brukere:
    // var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
    // var u = await userMgr.FindByEmailAsync("pilot@example.com")
    //         ?? new IdentityUser { UserName = "pilot@example.com", Email = "pilot@example.com" };
    // if (u.Id == null) { await userMgr.CreateAsync(u, "Passw0rd!"); await userMgr.AddToRoleAsync(u, "Pilot"); }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// statiske filer (fra .NET 9-templates)
app.MapStaticAssets();

app.UseRouting();

// Authn/Authz må ligge mellom UseRouting og Map*
app.UseAuthentication();
app.UseAuthorization();

// MVC route
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Index}/{id?}")
    .WithStaticAssets();

// (hvis du bruker Identity UI/Razor Pages)
app.MapRazorPages();

app.Run();
