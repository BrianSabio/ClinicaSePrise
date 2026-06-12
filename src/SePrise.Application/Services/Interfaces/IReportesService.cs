namespace SePrise.Application.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.DTOs.Reportes;

/// <summary>
/// Interfaz para el servicio de aplicación de generación de reportes.
/// Proporciona vistas analíticas de atenciones finalizadas.
/// </summary>
public interface IReportesService
{
    /// <summary>
    /// Obtiene todas las atenciones finalizadas en un rango de fechas.
    /// </summary>
    Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta);

    /// <summary>
    /// Obtiene todas las atenciones finalizadas de un médico específico, opcionalmente por fecha.
    /// </summary>
    Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorMedicoAsync(int idMedico, DateTime? fechaDesde = null, DateTime? fechaHasta = null);

    /// <summary>
    /// Obtiene todas las atenciones finalizadas de una especialidad específica, opcionalmente por fecha.
    /// </summary>
    Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorEspecialidadAsync(int idEspecialidad, DateTime? fechaDesde = null, DateTime? fechaHasta = null);

    /// <summary>
    /// Obtiene un resumen estadístico de atenciones finalizadas en un rango.
    /// </summary>
    Task<ReporteSummaryDTO> ObtenerResumenPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta);
}
