using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class EspecialidadService
{
    private readonly ApiClient _apiClient;

    public EspecialidadService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<EspecialidadDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<EspecialidadDTO>>("/api/especialidades");
        return result ?? new List<EspecialidadDTO>();
    }
}


