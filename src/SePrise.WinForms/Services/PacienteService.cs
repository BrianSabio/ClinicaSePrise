using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class PacienteService
{
    private readonly ApiClient _apiClient;

    public PacienteService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<PacienteDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<PacienteDTO>>("/api/pacientes");
        return result ?? new List<PacienteDTO>();
    }

    public async Task<PacienteDTO?> ObtenerPorIdAsync(int id)
    {
        return await _apiClient.GetAsync<PacienteDTO>($"/api/pacientes/{id}");
    }

    public async Task<List<PacienteDTO>> BuscarPorDNIAsync(string dni)
    {
        var result = await _apiClient.GetAsync<List<PacienteDTO>>($"/api/pacientes?dni={dni}");
        return result ?? new List<PacienteDTO>();
    }

    public async Task<PacienteDTO?> CrearAsync(CreatePacienteRequest dto)
    {
        return await _apiClient.PostAsync<PacienteDTO>("/api/pacientes", dto);
    }

    public async Task<PacienteDTO?> ActualizarAsync(int id, UpdatePacienteRequest dto)
    {
        return await _apiClient.PutAsync<PacienteDTO>($"/api/pacientes/{id}", dto);
    }

    public async Task EliminarAsync(int id)
    {
        await _apiClient.DeleteAsync($"/api/pacientes/{id}");
    }
}
