using Microsoft.Extensions.DependencyInjection;

namespace SePrise.Infrastructure.DependencyInjection;

/// <summary>
/// Extensiones de IServiceCollection para registrar los servicios de la capa Infrastructure.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de infraestructura: DbContext, repositorios y configuraciones.
    /// </summary>
    /// <param name="services">Colección de servicios del contenedor IoC.</param>
    /// <returns>La misma colección de servicios para encadenamiento fluido.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Registros de DI se agregarán en Microtarea 2.5
        return services;
    }
}
