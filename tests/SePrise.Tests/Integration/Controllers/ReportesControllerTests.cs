namespace SePrise.Tests.Integration.Controllers;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SePrise.API.Models.Responses;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;
using SePrise.Infrastructure.Persistence;
using SePrise.Tests.Integration.Common;
using SePrise.Tests.Integration.Fixtures;
using SePrise.Tests.Integration.Helpers;
using Xunit;

[Collection("Sequential")]
public class ReportesControllerTests : ControllerTestBase
{
    public ReportesControllerTests(SePriseWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Helper Methods

    private async Task<(int medicoId1, int medicoId2)> SetupFinalizadasAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();

        var m1Matricula = $"M1-{Guid.NewGuid().ToString().Substring(0, 4)}";
        var m2Matricula = $"M2-{Guid.NewGuid().ToString().Substring(0, 4)}";

        var medico1 = Medico.Crear(m1Matricula, "Carlos", "García");
        var medico2 = Medico.Crear(m2Matricula, "Ana", "López");
        dbContext.Medicos.Add(medico1);
        dbContext.Medicos.Add(medico2);
        await dbContext.SaveChangesAsync();

        var randomSuffix = Guid.NewGuid().ToString().Substring(0, 4);
        var especialidad = Especialidad.Crear($"Clínica Médica-{randomSuffix}");
        var sala = Sala.Crear($"S-{randomSuffix}", TipoSala.Consultorio);
        dbContext.Especialidades.Add(especialidad);
        dbContext.Salas.Add(sala);
        await dbContext.SaveChangesAsync();

        dbContext.Set<MedicoEspecialidad>().Add(MedicoEspecialidad.Crear(medico1.IdMedico, especialidad.IdEspecialidad));
        dbContext.Set<MedicoEspecialidad>().Add(MedicoEspecialidad.Crear(medico2.IdMedico, especialidad.IdEspecialidad));
        await dbContext.SaveChangesAsync();

        var p1Dni = new Random().Next(10000000, 99999999).ToString();
        var p2Dni = new Random().Next(10000000, 99999999).ToString();
        var p3Dni = new Random().Next(10000000, 99999999).ToString();

        var paciente1 = Paciente.Crear(Dni.Crear(p1Dni), "Juan", "P", new DateTime(1990, 1, 1), 'M');
        var paciente2 = Paciente.Crear(Dni.Crear(p2Dni), "María", "P", new DateTime(1990, 1, 1), 'F');
        var paciente3 = Paciente.Crear(Dni.Crear(p3Dni), "Pedro", "P", new DateTime(1990, 1, 1), 'M');
        dbContext.Pacientes.Add(paciente1);
        dbContext.Pacientes.Add(paciente2);
        dbContext.Pacientes.Add(paciente3);
        await dbContext.SaveChangesAsync();

        for (int i = 0; i < 3; i++)
        {
            var turno = TurnoAggregate.Crear(
                paciente1.IdPaciente,
                medico1.IdMedico,
                especialidad.IdEspecialidad,
                sala.IdSala,
                DateTime.UtcNow.AddDays(5),
                30
            );
            typeof(TurnoAggregate).GetProperty("FechaHoraInicio")?.SetValue(turno, new DateTime(2010, 6, 10 + i, 9, 0, 0, DateTimeKind.Utc));
            dbContext.Turnos.Add(turno);
            await dbContext.SaveChangesAsync();

            var atencion = AtencionAggregate.CrearDesdeConfirmacion(turno.IdTurno, paciente1.IdPaciente, medico1.IdMedico, ModalidadPago.ObraSocial);
            var baseDate = new DateTime(2010, 6, 10 + i, 9, 0, 0, DateTimeKind.Utc);
            typeof(AtencionAggregate).GetProperty("FechaHoraAcreditacion")?.SetValue(atencion, baseDate);
            
            atencion.ProgresarAEnProgreso(baseDate.AddMinutes(15));
            atencion.Finalizar(baseDate.AddMinutes(45), "Consulta completada");

            dbContext.Atenciones.Add(atencion);
            await dbContext.SaveChangesAsync();
        }

        var turno2 = TurnoAggregate.Crear(
            paciente2.IdPaciente,
            medico2.IdMedico,
            especialidad.IdEspecialidad,
            sala.IdSala,
            DateTime.UtcNow.AddDays(1),
            30
        );
        typeof(TurnoAggregate).GetProperty("FechaHoraInicio")?.SetValue(turno2, new DateTime(2010, 6, 14, 10, 0, 0, DateTimeKind.Utc));
        
        dbContext.Turnos.Add(turno2);
        await dbContext.SaveChangesAsync();

        var atencion2 = AtencionAggregate.CrearDesdeConfirmacion(turno2.IdTurno, paciente2.IdPaciente, medico2.IdMedico, ModalidadPago.Particular);
        var baseDate2 = new DateTime(2010, 6, 14, 10, 0, 0, DateTimeKind.Utc);
        typeof(AtencionAggregate).GetProperty("FechaHoraAcreditacion")?.SetValue(atencion2, baseDate2);

        atencion2.ProgresarAEnProgreso(baseDate2.AddMinutes(15));
        atencion2.Finalizar(baseDate2.AddMinutes(45), "Consulta completada");

        dbContext.Atenciones.Add(atencion2);
        await dbContext.SaveChangesAsync();

        return (medico1.IdMedico, medico2.IdMedico);
    }

    #endregion

    #region PorFecha Tests

    [Fact]
    public async Task ObtenerPorFecha_WithValidRange_ReturnsOk()
    {
        await SetupFinalizadasAsync();
        var fechaDesde = "2010-06-01";
        var fechaHasta = "2010-06-30";

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-fecha?fechaDesde={fechaDesde}&fechaHasta={fechaHasta}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reportes = await response.ReadAsAsync<List<AtencionResponse>>();
        reportes.Should().NotBeNull();
        reportes!.Should().HaveCountGreaterThanOrEqualTo(4); 
        reportes.Should().AllSatisfy(r => r.Estado.Should().Be("Finalizada"));
    }

    [Fact]
    public async Task ObtenerPorFecha_WithRangeNoResults_ReturnsEmptyArray()
    {
        await SetupFinalizadasAsync();
        var fechaDesde = "2005-01-01";
        var fechaHasta = "2005-12-31";

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-fecha?fechaDesde={fechaDesde}&fechaHasta={fechaHasta}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reportes = await response.ReadAsAsync<List<AtencionResponse>>();
        reportes.Should().NotBeNull();
        reportes!.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerPorFecha_WithInvertedDates_ReturnsBadRequest()
    {
        var fechaDesde = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var fechaHasta = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd");

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-fecha?fechaDesde={fechaDesde}&fechaHasta={fechaHasta}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PorMedico Tests

    [Fact]
    public async Task ObtenerPorMedico_WithValidMedicoId_ReturnsOk()
    {
        var (medicoId1, _) = await SetupFinalizadasAsync();

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-medico?idMedico={medicoId1}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var reportes = await response.ReadAsAsync<List<AtencionResponse>>();
        reportes.Should().NotBeNull();
        reportes!.Should().HaveCount(3);
        reportes.Should().AllSatisfy(r => r.IdMedico.Should().Be(medicoId1));
    }

    [Fact]
    public async Task ObtenerPorMedico_WithNoAtenciones_ReturnsEmptyArray()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SePriseDbContext>();
        
        var mat = $"M9-{Guid.NewGuid().ToString().Substring(0, 4)}";
        var medico = Medico.Crear(mat, "Sin", "Atenciones");
        dbContext.Medicos.Add(medico);
        await dbContext.SaveChangesAsync();
        var medicoId = medico.IdMedico;

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-medico?idMedico={medicoId}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reportes = await response.ReadAsAsync<List<AtencionResponse>>();
        reportes.Should().NotBeNull();
        reportes!.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerPorMedico_WithInvalidMedicoId_ReturnsBadRequest()
    {
        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/por-medico?idMedico=0"
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Resumen Tests

    [Fact]
    public async Task ObtenerResumen_WithValidRange_ReturnsStatistics()
    {
        await SetupFinalizadasAsync();
        var fechaDesde = "2010-06-01";
        var fechaHasta = "2010-06-30";

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/resumen?fechaDesde={fechaDesde}&fechaHasta={fechaHasta}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resumen = await response.ReadAsAsync<ReporteSummaryResponse>();
        resumen.Should().NotBeNull();
        resumen!.TotalAtenciones.Should().BeGreaterThanOrEqualTo(4); 
        resumen.TotalObraSocial.Should().BeGreaterThanOrEqualTo(3);
        resumen.TotalParticular.Should().BeGreaterThanOrEqualTo(1);
        resumen.TiempoPromedioMinutos.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ObtenerResumen_WithInvertedDates_ReturnsBadRequest()
    {
        var fechaDesde = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var fechaHasta = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd");

        var response = await Client.GetAsync(
            $"{TestDefaults.ReportesUrl}/resumen?fechaDesde={fechaDesde}&fechaHasta={fechaHasta}"
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}
