using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;
public class AtencionService
{
    private readonly ApiClient _apiClient;

    // La ruta base correcta es /api/Atencions porque el controller se llama AtencionsController
    private const string BaseRoute = "/api/Atencions";

    public AtencionService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task<AtencionDTO?> AcreditarPacienteAsync(AcreditarPacienteRequest dto)
    {
        return await _apiClient.PostAsync<AtencionDTO>($"{BaseRoute}/acreditar", dto);
    }
    public async Task<AtencionDTO?> CrearDemandaEspontaneaAsync(CrearDemandaEspontaneaRequest dto)
    {
        return await _apiClient.PostAsync<AtencionDTO>($"{BaseRoute}/demanda-espontanea", dto);
    }
    public async Task<List<AtencionDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<AtencionDTO>>(BaseRoute);
        return result ?? new List<AtencionDTO>();
    }
    public async Task<AtencionDTO?> ObtenerPorIdAsync(int id)
    {
        return await _apiClient.GetAsync<AtencionDTO>($"{BaseRoute}/{id}");
    }
    public async Task<AtencionDTO?> IniciarAsync(int id)
    {
        // El servidor valida que id del path == IdAtencion del body
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/iniciar", new { IdAtencion = id });
    }
    public async Task<AtencionDTO?> FinalizarAsync(int id, string notas = "")
    {
        // El servidor requiere IdAtencion en el body además de las Notas
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/finalizar",
            new { IdAtencion = id, Notas = notas });
    }
    public async Task<AtencionDTO?> CancelarAsync(int id)
    {
        // El servidor valida que id del path == IdAtencion del body
        return await _apiClient.PatchAsync<AtencionDTO>($"{BaseRoute}/{id}/cancelar", new { IdAtencion = id });
    }
}


