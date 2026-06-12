using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class MedicoService
{
    private readonly ApiClient _apiClient;

    public MedicoService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<MedicoDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<MedicoDTO>>("/api/medicos");
        return result ?? new List<MedicoDTO>();
    }
}
