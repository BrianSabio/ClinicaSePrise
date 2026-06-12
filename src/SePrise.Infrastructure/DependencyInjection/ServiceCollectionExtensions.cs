namespace SePrise.Infrastructure.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SePrise.Domain.Repositories;
using SePrise.Infrastructure.Persistence;
using SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Extensiones para registrar servicios de infraestructura en el contenedor de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de infraestructura (DbContext, Repositorios).
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <returns>IServiceCollection para encadenamiento.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Validar parámetros
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Registrar DbContext con SQL Server
        // La connection string viene de appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada.");

        services.AddDbContext<SePriseDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        // Registrar todos los repositorios (Scoped = nueva instancia por request HTTP)
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<ISalaRepository, SalaRepository>();
        services.AddScoped<IMedicoEspecialidadRepository, MedicoEspecialidadRepository>();
        services.AddScoped<ITurnoRepository, TurnoRepository>();
        services.AddScoped<IAtencionRepository, AtencionRepository>();

        // Registrar AutoMapper
        services.AddAutoMapper(cfg => {
            cfg.AddMaps(typeof(SePrise.Application.Mappers.PacienteProfile).Assembly);
        });

        return services;
    }
}
