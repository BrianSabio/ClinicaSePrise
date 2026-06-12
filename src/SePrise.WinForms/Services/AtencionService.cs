using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

/// <summary>
/// Servicio que encapsula las llamadas HTTP a los endpoints de Atención de la API.
/// </summary>
public class AtencionService
{
    private readonly ApiClient _apiClient;

    // La ruta base correcta es /api/Atencions porque el controller se llama AtencionsController
    private const string BaseRoute = "/api/Atencions";

    public AtencionService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>Acredita un paciente por turno previo y crea la Atencion.</summary>
    public async Task<AtencionDTO?> AcreditarPacienteAsync(AcreditarPacienteRequest dto)
    {
        return await _apiClient.PostAsync<AtencionDTO>($"{BaseRoute}/acreditar", dto);
    }

    /// <summary>Crea una atención por demanda espontánea sin turno previo.</summary>
    public async Task<AtencionDTO?> CrearDemandaEspontaneaAsync(CrearDemandaEspontaneaRequest dto)
    {
        return await _apiClient.PostAsync<AtencionDTO>($"{BaseRoute}/demanda-espontanea", dto);
    }

    /// <summary>Obtiene todas las atenciones del sistema.</summary>
    public async Task<List<AtencionDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<AtencionDTO>>(BaseRoute);
        return result ?? new List<AtencionDTO>();
    }

    /// <summary>Obtiene una atención por su ID.</summary>
    public async Task<AtencionDTO?> ObtenerPorIdAsync(int id)
    {
        return await _apiClient.GetAsync<AtencionDTO>($"{BaseRoute}/{id}");
    }

    /// <summary>Inicia una atención (Acreditada → EnProgreso).</summary>
    public async Task<AtencionDTO?> IniciarAsync(int id)
    {
        // El servidor valida que id del path == IdAtencion del body
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/iniciar", new { IdAtencion = id });
    }

    /// <summary>Finaliza una atención (EnProgreso → Finalizada). Notas es opcional.</summary>
    public async Task<AtencionDTO?> FinalizarAsync(int id, string notas = "")
    {
        // El servidor requiere IdAtencion en el body además de las Notas
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/finalizar",
            new { IdAtencion = id, Notas = notas });
    }

    /// <summary>Cancela una atención (Acreditada o EnProgreso → Cancelada).</summary>
    public async Task<AtencionDTO?> CancelarAsync(int id)
    {
        // El servidor valida que id del path == IdAtencion del body
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/cancelar", new { IdAtencion = id });
    }
}
