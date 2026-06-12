using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SePrise.API.Models.Requests;
using SePrise.API.Models.Responses;
using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;
using SePrise.Infrastructure.Persistence;
using SePrise.Tests.Integration.Common;
using SePrise.Tests.Integration.Fixtures;
using SePrise.Tests.Integration.Helpers;
using Xunit;

namespace SePrise.Tests.Integration.Controllers;

[Collection("Sequential")]
public class TurnosControllerTests : ControllerTestBase
{
    private const string TurnosUrl = "/api/turnos";

    public TurnosControllerTests(SePriseWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<(Paciente paciente, Medico medico, Especialidad especialidad, Sala sala)> SeedEntitiesAsync()
    {
        using var db = GetDbContext();
        
        var randomSuffix = Guid.NewGuid().ToString().Substring(0, 4);
        var dniStr = new Random().Next(10000000, 99999999).ToString();
        var paciente = Paciente.Crear(Dni.Crear(dniStr), "Test", "Paciente", new DateTime(1990, 1, 1), 'M', "test@example.com");
        var medico = Medico.Crear($"MAT-{randomSuffix}", "Test", "Medico");
        var especialidad = Especialidad.Crear($"Especialidad-{randomSuffix}");
        var sala = Sala.Crear($"Sala-{randomSuffix}", TipoSala.Consultorio);

        db.Pacientes.Add(paciente);
        db.Medicos.Add(medico);
        db.Especialidades.Add(especialidad);
        db.Salas.Add(sala);
        
        await db.SaveChangesAsync();

        var medicoEspecialidad = MedicoEspecialidad.Crear(medico.IdMedico, especialidad.IdEspecialidad);
        db.Set<MedicoEspecialidad>().Add(medicoEspecialidad);
        await db.SaveChangesAsync();

        return (paciente, medico, especialidad, sala);
    }

    private CreateTurnoRequest ValidTurnoRequest(int idPaciente, int idMedico, int idEspecialidad, int idSala, DateTime? fechaInicio = null)
    {
        return new CreateTurnoRequest
        {
            IdPaciente = idPaciente,
            IdMedico = idMedico,
            IdEspecialidad = idEspecialidad,
            IdSala = idSala,
            FechaHoraInicio = fechaInicio ?? DateTime.UtcNow.AddDays(1).Date.AddHours(10), // Mañana a las 10:00
            DuracionMinutos = 30
        };
    }

    [Fact]
    public async Task CreateTurno_WithValidData_ReturnsCreated()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);

        // Act
        var response = await Client.PostJsonAsync(TurnosUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var turno = await response.ReadAsAsync<TurnoResponse>();
        turno.Should().NotBeNull();
        turno!.Id.Should().BeGreaterThan(0);
        turno.IdPaciente.Should().Be(request.IdPaciente);
        turno.IdMedico.Should().Be(request.IdMedico);
        turno.IdEspecialidad.Should().Be(request.IdEspecialidad);
        turno.IdSala.Should().Be(request.IdSala);
        turno.Estado.Should().Be("Reservado");
    }

    [Fact]
    public async Task CreateTurno_WithInvalidPaciente_ReturnsConflict()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(99999, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);

        // Act
        var response = await Client.PostJsonAsync(TurnosUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict); // PacienteException is mapped to 409 Conflict
    }

    [Fact]
    public async Task GetTurno_WithValidId_ReturnsOk()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        var createResponse = await Client.PostJsonAsync(TurnosUrl, request);
        var createdTurno = await createResponse.ReadAsAsync<TurnoResponse>();
        var id = createdTurno!.Id;

        // Act
        var getResponse = await Client.GetAsync($"{TurnosUrl}/{id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var turno = await getResponse.ReadAsAsync<TurnoResponse>();
        turno.Should().NotBeNull();
        turno!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetTurnos_WithPacienteFilter_ReturnsOk()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        await Client.PostJsonAsync(TurnosUrl, request);

        // Act
        var getResponse = await Client.GetAsync($"{TurnosUrl}?idPaciente={entities.paciente.IdPaciente}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var turnos = await getResponse.ReadAsAsync<IEnumerable<TurnoResponse>>();
        turnos.Should().NotBeNull();
        turnos!.Should().ContainSingle();
        turnos!.First().IdPaciente.Should().Be(entities.paciente.IdPaciente);
    }

    [Fact]
    public async Task ConfirmarTurno_WithValidId_ReturnsOk()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        var createResponse = await Client.PostJsonAsync(TurnosUrl, request);
        var createdTurno = await createResponse.ReadAsAsync<TurnoResponse>();
        var id = createdTurno!.Id;

        var confirmRequest = new ConfirmTurnoRequest { IdTurno = id };

        // Act
        var patchResponse = await Client.PatchAsJsonAsync($"{TurnosUrl}/{id}/confirmar", confirmRequest);

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var turno = await patchResponse.ReadAsAsync<TurnoResponse>();
        turno.Should().NotBeNull();
        turno!.Estado.Should().Be("Confirmado");
    }

    [Fact]
    public async Task ConfirmarTurno_WithAlreadyConfirmedTurno_ReturnsConflict()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        var createResponse = await Client.PostJsonAsync(TurnosUrl, request);
        var createdTurno = await createResponse.ReadAsAsync<TurnoResponse>();
        var id = createdTurno!.Id;

        var confirmRequest = new ConfirmTurnoRequest { IdTurno = id };
        await Client.PatchAsJsonAsync($"{TurnosUrl}/{id}/confirmar", confirmRequest);

        // Act
        var patchResponse2 = await Client.PatchAsJsonAsync($"{TurnosUrl}/{id}/confirmar", confirmRequest);

        // Assert
        patchResponse2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CancelarTurno_WithValidId_ReturnsOk()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        var createResponse = await Client.PostJsonAsync(TurnosUrl, request);
        var createdTurno = await createResponse.ReadAsAsync<TurnoResponse>();
        var id = createdTurno!.Id;

        var cancelRequest = new CancelTurnoRequest { IdTurno = id };

        // Act
        var patchResponse = await Client.PatchAsJsonAsync($"{TurnosUrl}/{id}/cancelar", cancelRequest);

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verificar estado
        var getResponse = await Client.GetAsync($"{TurnosUrl}/{id}");
        var turno = await getResponse.ReadAsAsync<TurnoResponse>();
        turno!.Estado.Should().Be("Cancelado");
    }

    [Fact]
    public async Task ReprogramarTurno_WithValidData_ReturnsOk()
    {
        // Arrange
        var entities = await SeedEntitiesAsync();
        var request = ValidTurnoRequest(entities.paciente.IdPaciente, entities.medico.IdMedico, entities.especialidad.IdEspecialidad, entities.sala.IdSala);
        
        var createResponse = await Client.PostJsonAsync(TurnosUrl, request);
        var createdTurno = await createResponse.ReadAsAsync<TurnoResponse>();
        var id = createdTurno!.Id;

        var nuevaFecha = DateTime.UtcNow.AddDays(2).Date.AddHours(14);
        var rescheduleRequest = new RescheduleTurnoRequest 
        { 
            IdTurno = id,
            FechaHoraInicio = nuevaFecha,
            DuracionMinutos = 30
        };

        // Act
        var patchResponse = await Client.PatchAsJsonAsync($"{TurnosUrl}/{id}/reprogramar", rescheduleRequest);

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var turnoReprogramado = await patchResponse.ReadAsAsync<TurnoResponse>();
        turnoReprogramado.Should().NotBeNull();
        turnoReprogramado!.Id.Should().NotBe(id); // Debería ser un NUEVO turno
        turnoReprogramado.FechaHoraInicio.Should().Be(nuevaFecha);
        turnoReprogramado.Estado.Should().Be("Reservado");

        // Verificar que el viejo se canceló/reprogramó
        var getOldResponse = await Client.GetAsync($"{TurnosUrl}/{id}");
        var oldTurno = await getOldResponse.ReadAsAsync<TurnoResponse>();
        oldTurno!.Estado.Should().Be("Reprogramado");
    }
}

