using CobrancaPro.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Registre os serviços necessários
builder.Services
    .AddControllersWithViews()
    .AddDataAnnotationsLocalization();
    builder.Services.AddSession();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registre o serviço IBillingService
builder.Services.AddScoped<IBillingService, BillingService>();

builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection string usada: {connectionString}");

var app = builder.Build();

// Tratamento global de exceções para evitar que a aplicação feche inesperadamente
AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    var ex = eventArgs.ExceptionObject as Exception;
    Console.WriteLine($"[Global Exception] {ex}");
};

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Debug: Listar rotas
    Console.WriteLine("Rotas registradas:");
    foreach (var route in endpoints.DataSources.First().Endpoints.OfType<RouteEndpoint>())
    {
        Console.WriteLine(route.RoutePattern.RawText);
    }
});

app.Run();
