using AutoMapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SePrise.Tests.Integration.Config;

public class AutoMapperConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AutoMapperConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void AutoMapper_ConfigurationIsValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        // Act & Assert
        // Si hay algún mapeo faltando (Ej. no se registraron los perfiles de Application), esto lanzará AutoMapperConfigurationException
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
