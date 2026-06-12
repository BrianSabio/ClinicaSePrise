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
using SePrise.Domain.Aggregates;
using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;
using SePrise.Infrastructure.Persistence;
using SePrise.Tests.Integration.Common;
using SePrise.Tests.Integration.Fixtures;
using SePrise.Tests.Integration.Helpers;
using Xunit;

namespace SePrise.Tests.Integration.Controllers;

[Collection("Sequential")]
public class AtencionsControllerTests : ControllerTestBase
{
    private const string AtencionsUrl = "/api/atencions";

    public AtencionsControllerTests(SePriseWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Helper Methods

    /// <summary>
    /// Crea un setup completo: Paciente, Medico, Especialidad, Sala, Relación N:N, Turno.
    /// Retorna el turno listo para acreditar.
    /// </summary>
    private async Task<(int pacienteId, int medicoId, int especialidadId, int salaId, int turnoId)> SetupTurnoReadyForAccreditAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();

        var randomSuffix = Guid.NewGuid().ToString().Substring(0, 4);
        var dniStr = new Random().Next(10000000, 99999999).ToString();

        var paciente = Paciente.Crear(Dni.Crear(dniStr), "Test", "Paciente", new DateTime(1990, 1, 1), 'M');
        var medico = Medico.Crear($"MAT-{randomSuffix}", "Test", "Medico");
        var especialidad = Especialidad.Crear($"Especialidad-{randomSuffix}");
        var sala = Sala.Crear($"Sala-{randomSuffix}", TipoSala.Consultorio);

        dbContext.Pacientes.Add(paciente);
        dbContext.Medicos.Add(medico);
        dbContext.Especialidades.Add(especialidad);
        dbContext.Salas.Add(sala);
        await dbContext.SaveChangesAsync();

        var medicoEspec = MedicoEspecialidad.Crear(medico.IdMedico, especialidad.IdEspecialidad);
        dbContext.Set<MedicoEspecialidad>().Add(medicoEspec);
        await dbContext.SaveChangesAsync();

        var turno = TurnoAggregate.Crear(
            paciente.IdPaciente,
            medico.IdMedico,
            especialidad.IdEspecialidad,
            sala.IdSala,
            DateTime.UtcNow.AddDays(1).Date.AddHours(10), // Mañana a las 10
            30
        );
        dbContext.Turnos.Add(turno);
        await dbContext.SaveChangesAsync();

        return (paciente.IdPaciente, medico.IdMedico, especialidad.IdEspecialidad, sala.IdSala, turno.IdTurno);
    }

    #endregion

    #region Acreditación Tests

    [Fact]
    public async Task AcreditarPaciente_CreatesAtencionAndConfirmsTurno()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();

        var request = new AcreditarPacienteRequest
        {
            IdTurno = turnoId,
            ModalidadPago = "ObraSocial"
        };

        // Act
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var atencion = await response.ReadAsAsync<AtencionResponse>();
        atencion.Should().NotBeNull();
        atencion!.Estado.Should().Be("Acreditada");
        atencion.IdTurno.Should().Be(turnoId);

        // Validar cascada: Turno debe estar Confirmado
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        var turno = await dbContext.Turnos.FindAsync(turnoId);
        turno.Should().NotBeNull();
        turno!.Estado.Should().Be(EstadoTurno.Confirmado);
    }

    [Fact]
    public async Task AcreditarPaciente_WithNonExistentTurno_ReturnsConflict()
    {
        // Arrange
        var request = new AcreditarPacienteRequest
        {
            IdTurno = 99999,
            ModalidadPago = "ObraSocial"
        };

        // Act
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", request);

        // Assert
        // Turno no encontrado arroja TurnoException que es un Conflict (409)
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AcreditarPaciente_WithTurnoAlreadyConfirmed_ReturnsConflict()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        
        var request = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", request);

        // Act - Intentar acreditar de nuevo
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region Demanda Espontánea Tests

    [Fact]
    public async Task CrearDemandaEspontanea_WithoutTurno_ReturnsCreated()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();

        var dniStr = new Random().Next(10000000, 99999999).ToString();
        var randomSuffix = Guid.NewGuid().ToString().Substring(0, 4);

        var paciente = Paciente.Crear(Dni.Crear(dniStr), "Test", "Demanda", new DateTime(1990, 1, 1), 'M');
        var medico = Medico.Crear($"MAT-D{randomSuffix}", "Medico", "Demanda");
        var especialidad = Especialidad.Crear($"Especialidad-D{randomSuffix}");
        
        dbContext.Pacientes.Add(paciente);
        dbContext.Medicos.Add(medico);
        dbContext.Especialidades.Add(especialidad);
        await dbContext.SaveChangesAsync();

        var medicoEspec = MedicoEspecialidad.Crear(medico.IdMedico, especialidad.IdEspecialidad);
        dbContext.Set<MedicoEspecialidad>().Add(medicoEspec);
        await dbContext.SaveChangesAsync();

        var request = new CrearDemandaEspontaneaRequest
        {
            IdPaciente = paciente.IdPaciente,
            IdMedico = medico.IdMedico,
            ModalidadPago = "Particular"
        };

        // Act
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/demanda-espontanea", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var atencion = await response.ReadAsAsync<AtencionResponse>();
        atencion.Should().NotBeNull();
        atencion!.IdTurno.Should().BeNull();
        atencion.Estado.Should().Be("Acreditada");
    }

    [Fact]
    public async Task CrearDemandaEspontanea_WithNonExistentPaciente_ReturnsNotFound()
    {
        // Arrange
        var request = new CrearDemandaEspontaneaRequest
        {
            IdPaciente = 99999,
            IdMedico = 1,
            ModalidadPago = "Particular"
        };

        // Act
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/demanda-espontanea", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CrearDemandaEspontanea_WithNonExistentMedico_ReturnsNotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        var dniStr = new Random().Next(10000000, 99999999).ToString();
        var paciente = Paciente.Crear(Dni.Crear(dniStr), "Test", "Demanda2", new DateTime(1990, 1, 1), 'M');
        dbContext.Pacientes.Add(paciente);
        await dbContext.SaveChangesAsync();

        var request = new CrearDemandaEspontaneaRequest
        {
            IdPaciente = paciente.IdPaciente,
            IdMedico = 99999,
            ModalidadPago = "Particular"
        };

        // Act
        var response = await Client.PostJsonAsync($"{AtencionsUrl}/demanda-espontanea", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Estado Changes Tests

    [Fact]
    public async Task IniciarAtencion_FromAcreditada_ReturnsOk()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };

        // Act
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updated = await response.ReadAsAsync<AtencionResponse>();
        updated.Should().NotBeNull();
        updated!.Estado.Should().Be("EnProgreso");
        updated.FechaHoraInicio.Should().NotBeNull();
    }

    [Fact]
    public async Task IniciarAtencion_FromNonAcreditada_ReturnsConflict()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        // Act - Intentar iniciar de nuevo
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task FinalizarAtencion_FromEnProgreso_UpdatesAtencionAndTurno()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        var finalizarRequest = new FinalizarAtencionRequest 
        { 
            IdAtencion = idAtencion,
            Notas = "Todo bien"
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/finalizar", finalizarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var finalizada = await response.ReadAsAsync<AtencionResponse>();
        finalizada.Should().NotBeNull();
        finalizada!.Estado.Should().Be("Finalizada");
        finalizada.Notas.Should().Be("Todo bien");
        finalizada.FechaHoraFin.Should().NotBeNull();

        // Validar cascada en el Turno
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        var turno = await dbContext.Turnos.FindAsync(turnoId);
        turno.Should().NotBeNull();
        turno!.Estado.Should().Be(EstadoTurno.Atendido);
    }

    [Fact]
    public async Task FinalizarAtencion_FromNonEnProgreso_ReturnsConflict()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        // Está en Acreditada, no EnProgreso
        var finalizarRequest = new FinalizarAtencionRequest { IdAtencion = idAtencion, Notas = "Error" };

        // Act
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/finalizar", finalizarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task FinalizarAtencion_WithoutTurno_FinalizesOnly()
    {
        // Arrange - Demanda espontánea
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        var randomSuffix = Guid.NewGuid().ToString().Substring(0, 4);
        var dniStr = new Random().Next(10000000, 99999999).ToString();
        var paciente = Paciente.Crear(Dni.Crear(dniStr), "Test", "Demanda", new DateTime(1990, 1, 1), 'M');
        var medico = Medico.Crear($"MAT-D2{randomSuffix}", "Medico", "Demanda");
        var especialidad = Especialidad.Crear($"Especialidad-D2{randomSuffix}");
        dbContext.Pacientes.Add(paciente);
        dbContext.Medicos.Add(medico);
        dbContext.Especialidades.Add(especialidad);
        await dbContext.SaveChangesAsync();
        var medicoEspec = MedicoEspecialidad.Crear(medico.IdMedico, especialidad.IdEspecialidad);
        dbContext.Set<MedicoEspecialidad>().Add(medicoEspec);
        await dbContext.SaveChangesAsync();

        var request = new CrearDemandaEspontaneaRequest
        {
            IdPaciente = paciente.IdPaciente,
            IdMedico = medico.IdMedico,
            ModalidadPago = "Particular"
        };
        var createResponse = await Client.PostJsonAsync($"{AtencionsUrl}/demanda-espontanea", request);
        var atencion = await createResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        // Iniciar
        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        // Act - Finalizar
        var finalizarRequest = new FinalizarAtencionRequest { IdAtencion = idAtencion, Notas = "Demanda espontánea completada" };
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/finalizar", finalizarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var finalizada = await response.ReadAsAsync<AtencionResponse>();
        finalizada!.Estado.Should().Be("Finalizada");
        finalizada.IdTurno.Should().BeNull();
    }

    #endregion

    #region Cancelación Tests

    [Fact]
    public async Task CancelarAtencion_FromAcreditada_ReturnsOk()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var cancelarRequest = new CancelarAtencionRequest { IdAtencion = idAtencion };

        // Act
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/cancelar", cancelarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var cancelada = await response.ReadAsAsync<AtencionResponse>();
        cancelada.Should().NotBeNull();
        cancelada!.Estado.Should().Be("Cancelada");

        // Validar cascada: Turno también debe estar Cancelado
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        var turno = await dbContext.Turnos.FindAsync(turnoId);
        turno.Should().NotBeNull();
        turno!.Estado.Should().Be(EstadoTurno.Cancelado);
    }

    [Fact]
    public async Task CancelarAtencion_FromEnProgreso_ReturnsOk()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        var cancelarRequest = new CancelarAtencionRequest { IdAtencion = idAtencion };

        // Act
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/cancelar", cancelarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cancelada = await response.ReadAsAsync<AtencionResponse>();
        cancelada!.Estado.Should().Be("Cancelada");
    }

    [Fact]
    public async Task CancelarAtencion_FromTerminalState_ReturnsConflict()
    {
        // Arrange
        var (_, _, _, _, turnoId) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest = new AcreditarPacienteRequest { IdTurno = turnoId, ModalidadPago = "ObraSocial" };
        var acreditResponse = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest);
        var atencion = await acreditResponse.ReadAsAsync<AtencionResponse>();
        var idAtencion = atencion!.Id;

        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = idAtencion };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/iniciar", iniciarRequest);

        var finalizarRequest = new FinalizarAtencionRequest { IdAtencion = idAtencion, Notas = "Ok" };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/finalizar", finalizarRequest);

        // Act - Intentar cancelar desde Finalizada
        var cancelarRequest = new CancelarAtencionRequest { IdAtencion = idAtencion };
        var response = await Client.PatchAsJsonAsync($"{AtencionsUrl}/{idAtencion}/cancelar", cancelarRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region Filtrado Tests

    [Fact]
    public async Task GetAtenciones_FilteredByEstado_ReturnsFiltered()
    {
        // Arrange
        var (_, _, _, _, turnoId1) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest1 = new AcreditarPacienteRequest { IdTurno = turnoId1, ModalidadPago = "ObraSocial" };
        await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest1);

        var (_, _, _, _, turnoId2) = await SetupTurnoReadyForAccreditAsync();
        var acreditRequest2 = new AcreditarPacienteRequest { IdTurno = turnoId2, ModalidadPago = "ObraSocial" };
        var acreditResponse2 = await Client.PostJsonAsync($"{AtencionsUrl}/acreditar", acreditRequest2);
        var atencion2 = await acreditResponse2.ReadAsAsync<AtencionResponse>();
        var iniciarRequest = new IniciarAtencionRequest { IdAtencion = atencion2!.Id };
        await Client.PatchAsJsonAsync($"{AtencionsUrl}/{atencion2.Id}/iniciar", iniciarRequest);

        // Act
        var response = await Client.GetAsync($"{AtencionsUrl}?estado=EnProgreso");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var atenciones = await response.ReadAsAsync<List<AtencionResponse>>();
        atenciones.Should().NotBeNull();
        atenciones!.Should().NotBeEmpty();
        atenciones.Should().AllSatisfy(a => a.Estado.Should().Be("EnProgreso"));
    }

    #endregion
}
