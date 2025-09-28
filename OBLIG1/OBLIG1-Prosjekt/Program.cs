//Starter asp.net core-appen
var builder = WebApplication.CreateBuilder(args);

//Registrerer MVC(controllers og razor-views
builder.Services.AddControllersWithViews();

//Bygger web-appen
var app = builder.Build();

//Konfigurerer http
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); //sender feil til /home/error
    app.UseHsts(); //hsts header for å sikre https forbindelser
    app.UseHttpsRedirection(); //redirecter http til https i produksjonen
}

//Aktiverer routing-system
app.UseRouting();

//Aktiverer en "server" for statiske filer
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); //Statiske filer kan brukes sammen med routene / sørger for at- 
                        // CSS-filene kan leveres når de brukes sammen med controller sider

//Starter webserveren
app.Run();