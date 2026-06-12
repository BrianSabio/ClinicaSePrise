namespace SePrise.Application.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.DTOs.Reportes;
public interface IReportesService
{
Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta);
Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorMedicoAsync(int idMedico, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
Task<IEnumerable<AtencionDTO>> ObtenerAtencionesPorEspecialidadAsync(int idEspecialidad, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
Task<ReporteSummaryDTO> ObtenerResumenPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta);
}


