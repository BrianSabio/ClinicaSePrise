using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class SalaService
{
    private readonly ApiClient _apiClient;

    public SalaService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<SalaDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<SalaDTO>>("/api/salas");
        return result ?? new List<SalaDTO>();
    }
}
