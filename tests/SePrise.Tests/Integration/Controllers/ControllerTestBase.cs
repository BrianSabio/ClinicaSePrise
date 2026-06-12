namespace SePrise.Tests.Integration.Controllers;

using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SePrise.Infrastructure.Persistence;
using SePrise.Tests.Integration.Fixtures;
using Xunit;

/// <summary>
/// Clase base para las pruebas de integración de controladores.
/// Comparte el factory (y por ende, la base de datos in-memory) en todos los tests de la clase.
/// </summary>
public abstract class ControllerTestBase : IClassFixture<SePriseWebApplicationFactory>
{
    protected readonly SePriseWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ControllerTestBase(SePriseWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Crea un nuevo scope de DI y obtiene una instancia fresca del DbContext.
    /// Importante: El llamador debe desechar el DbContext (y opcionalmente el scope),
    /// o bien usarlo temporalmente para setup o asserts.
    /// </summary>
    protected SePriseDbContext GetDbContext()
    {
        // No almacenamos el DbContext como campo porque EF no es thread-safe
        // y porque debemos obtener uno fresco en cada request de setup/assert.
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
    }
}

