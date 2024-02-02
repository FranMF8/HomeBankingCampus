using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HomeBankingContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<HomeBankingContext>();
        DBInitializer.Initialize(context);
    }
    catch (Exception ex) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos!");
    }
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();