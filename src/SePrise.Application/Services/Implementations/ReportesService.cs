namespace SePrise.Application.Services.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.DTOs.Reportes;
using SePrise.Application.Services.Interfaces;
public class ReportesService : IReportesService
{
    private readonly IAtencionRepository _atencionRepository;
    private readonly IMapper _mapper;
public ReportesService(IAtencionRepository atencionRepository, IMapper mapper)
    {
        _atencionRepository = atencionRepository ?? throw new ArgumentNullException(nameof(atencionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta)
    {
        if (fechaDesde == default)
            throw new ArgumentException("Fecha desde es requerida", nameof(fechaDesde));

        if (fechaHasta == default)
            throw new ArgumentException("Fecha hasta es requerida", nameof(fechaHasta));

        if (fechaDesde > fechaHasta)
            throw new ArgumentException("Fecha desde no puede ser mayor a fecha hasta");

        var atenciones = await _atencionRepository.GetFinalizadasAsync(fechaDesde, fechaHasta);
        return _mapper.Map<IEnumerable<AtencionDTO>>(atenciones);
    }
    public async Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorMedicoAsync(
        int idMedico, 
        DateTime? fechaDesde = null, 
        DateTime? fechaHasta = null)
    {
        if (idMedico <= 0)
            throw new ArgumentException("ID de médico debe ser > 0", nameof(idMedico));

        var atenciones = await _atencionRepository.GetByMedicoAsync(idMedico);

        // Filtrar solo finalizadas
        var atencionesFinalizadas = atenciones.Where(a => a.Estado == EstadoAtencion.Finalizada);

        // Filtrar por rango si se proporciona
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            if (fechaDesde > fechaHasta)
                throw new ArgumentException("Fecha desde no puede ser mayor a fecha hasta");

            atencionesFinalizadas = atencionesFinalizadas
                .Where(a => a.FechaHoraAcreditacion >= fechaDesde && a.FechaHoraAcreditacion <= fechaHasta);
        }

        return _mapper.Map<IEnumerable<AtencionDTO>>(atencionesFinalizadas);
    }
    public async Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorEspecialidadAsync(
        int idEspecialidad, 
        DateTime? fechaDesde = null, 
        DateTime? fechaHasta = null)
    {
        if (idEspecialidad <= 0)
            throw new ArgumentException("ID de especialidad debe ser > 0", nameof(idEspecialidad));

        // Obtener todas las atenciones finalizadas
        var atenciones = await _atencionRepository.GetByEstadoAsync(EstadoAtencion.Finalizada);

        // Filtrar por especialidad (a través de Turno)
        var atencionesEspecialidad = atenciones.Where(a => 
            a.Turno != null && a.Turno.IdEspecialidad == idEspecialidad);

        // Filtrar por rango si se proporciona
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            if (fechaDesde > fechaHasta)
                throw new ArgumentException("Fecha desde no puede ser mayor a fecha hasta");

            atencionesEspecialidad = atencionesEspecialidad
                .Where(a => a.FechaHoraAcreditacion >= fechaDesde && a.FechaHoraAcreditacion <= fechaHasta);
        }

        return _mapper.Map<IEnumerable<AtencionDTO>>(atencionesEspecialidad);
    }
    public async Task<ReporteSummaryDTO> ObtenerResumenPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta)
    {
        if (fechaDesde == default)
            throw new ArgumentException("Fecha desde es requerida", nameof(fechaDesde));

        if (fechaHasta == default)
            throw new ArgumentException("Fecha hasta es requerida", nameof(fechaHasta));

        if (fechaDesde > fechaHasta)
            throw new ArgumentException("Fecha desde no puede ser mayor a fecha hasta");

        var atenciones = await _atencionRepository.GetFinalizadasAsync(fechaDesde, fechaHasta);
        var atencionesLista = atenciones.ToList();

        // Calcular métricas
        var totalAtenciones = atencionesLista.Count;
        var totalObraSocial = atencionesLista.Count(a => a.ModalidadPago == ModalidadPago.ObraSocial);
        var totalParticular = atencionesLista.Count(a => a.ModalidadPago == ModalidadPago.Particular);
        var totalPacientes = atencionesLista.Select(a => a.IdPaciente).Distinct().Count();

        var tiemposMinutos = atencionesLista
            .Where(a => a.FechaHoraInicio.HasValue && a.FechaHoraFin.HasValue)
            .Select(a => (a.FechaHoraFin.GetValueOrDefault() - a.FechaHoraInicio.GetValueOrDefault()).TotalMinutes);

        var tiempoPromedioMinutos = tiemposMinutos.Any() ? tiemposMinutos.Average() : 0;

        return new ReporteSummaryDTO
        {
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            TotalAtenciones = totalAtenciones,
            TotalObraSocial = totalObraSocial,
            TotalParticular = totalParticular,
            TotalPacientesUnicos = totalPacientes,
            TiempoPromedioMinutos = (int)tiempoPromedioMinutos
        };
    }
}


