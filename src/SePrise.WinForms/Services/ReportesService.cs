namespace SePrise.WinForms.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.WinForms.Models;

public class ReportesService
{
    private readonly ApiClient _apiClient;

    public ReportesService(ApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    /// <summary>
    /// Obtiene atenciones por rango de fechas.
    /// </summary>
    public async Task<IEnumerable<AtencionDTO>> ObtenerPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta)
    {
        try
        {
            var url = $"/api/reportes/por-fecha?fechaDesde={fechaDesde:yyyy-MM-dd}&fechaHasta={fechaHasta:yyyy-MM-dd}";
            return await _apiClient.GetAsync<IEnumerable<AtencionDTO>>(url);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener reportes por fecha: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene atenciones por médico específico.
    /// </summary>
    public async Task<IEnumerable<AtencionDTO>> ObtenerPorMedicoAsync(int idMedico, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
    {
        try
        {
            var url = $"/api/reportes/por-medico?idMedico={idMedico}";
            if (fechaDesde.HasValue)
                url += $"&fechaDesde={fechaDesde:yyyy-MM-dd}";
            if (fechaHasta.HasValue)
                url += $"&fechaHasta={fechaHasta:yyyy-MM-dd}";
            
            return await _apiClient.GetAsync<IEnumerable<AtencionDTO>>(url);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener reportes por médico: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene atenciones por especialidad específica.
    /// </summary>
    public async Task<IEnumerable<AtencionDTO>> ObtenerPorEspecialidadAsync(int idEspecialidad, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
    {
        try
        {
            var url = $"/api/reportes/por-especialidad?idEspecialidad={idEspecialidad}";
            if (fechaDesde.HasValue)
                url += $"&fechaDesde={fechaDesde:yyyy-MM-dd}";
            if (fechaHasta.HasValue)
                url += $"&fechaHasta={fechaHasta:yyyy-MM-dd}";
            
            return await _apiClient.GetAsync<IEnumerable<AtencionDTO>>(url);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener reportes por especialidad: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene resumen estadístico de atenciones (método principal para preview).
    /// </summary>
    public async Task<ReporteSummaryDTO> ObtenerResumenAsync(DateTime fechaDesde, DateTime fechaHasta)
    {
        try
        {
            var url = $"/api/reportes/resumen?fechaDesde={fechaDesde:yyyy-MM-dd}&fechaHasta={fechaHasta:yyyy-MM-dd}";
            return await _apiClient.GetAsync<ReporteSummaryDTO>(url);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener resumen: {ex.Message}", ex);
        }
    }
}
