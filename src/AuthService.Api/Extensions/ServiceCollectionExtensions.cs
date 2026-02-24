using AuthService.Application.Interfaces;
using AuthService.Application.Services;

using AuthService.Domain.Entities;
using AuthService.Domain.Constants;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;

namespace AuthService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration configuration)
    {
        //Inicializando la conexion a la base de datos
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        //Inicializando el Servicio de Email
        services.AddScoped<IEmailService, EmailService>();

        // Repositories
        services.AddHealthChecks();

        return services;
    }
}

