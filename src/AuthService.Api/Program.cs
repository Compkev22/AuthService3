using AuthService.Api.Extensions;
using AuthService.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Extensiones personalizadas para registrar servicios de la aplicación
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Array de prueba para el endpoint de clima
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Endpoint de ejemplo (Minimal API)
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// INICIALIZACION DE LA BASE DE DATOS
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando migracion de la base de datos...");

        await context.Database.EnsureCreatedAsync();

        logger.LogInformation("Migracion completada exitosamente");
        await DataSeeder.SeedAsync(context);
        logger.LogInformation("Datos iniciales cargados exitosamente");

    }catch(Exception es)
    {
        logger.LogError(es, "Error al inicializar la base de datos");
        throw; //Detener la aplicacion si hay un error al inicializar la base de datos

    }
}

//----------------------------------------------------------

app.Run();

// Registro interno para el record de clima usado arriba
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}