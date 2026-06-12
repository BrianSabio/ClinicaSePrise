using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using SePrise.WinForms.Models; // WinForms DTOs

namespace SePrise.Tests.Integration.Contracts;

public class ContractIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ContractIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        // Exactamente la misma configuración que en WinForms
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
    }

    [Fact]
    public async Task GetPacientes_ReturnsJson_ThatMatchesWinFormsDTOExactly()
    {
        // Act
        var response = await _client.GetAsync("/api/pacientes");

        // Assert
        response.EnsureSuccessStatusCode();

        // Validar que el string retornado no lanza excepción de mapeo al pasarlo por las reglas estrictas de WinForms
        // Si la API cambia un nombre o agrega una propiedad nueva no documentada en el Front, esto fallará.
        IEnumerable<PacienteDTO>? dto = null;
        try
        {
            dto = await response.Content.ReadFromJsonAsync<IEnumerable<PacienteDTO>>(_jsonOptions);
        }
        catch (JsonException ex)
        {
            Assert.Fail($"Desincronización de Contrato: {ex.Message}");
        }
        
        dto.Should().NotBeNull();
        if (dto != null && dto.Any())
        {
            dto.First().IdPaciente.Should().BeGreaterThan(0, "porque el ID del JSON debe haberse mapeado correctamente a IdPaciente");
        }
    }
}
