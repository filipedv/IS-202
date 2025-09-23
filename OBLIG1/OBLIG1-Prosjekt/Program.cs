using OBLIG1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPointRepository, InMemoryPointRepository>(); // <-- legg til denne

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    // In production, we redirect HTTP -> HTTPS.
    app.UseHttpsRedirection();
}

// In Development (e.g., inside Docker with HTTP on :8080) we do NOT force HTTPS.
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Points}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();