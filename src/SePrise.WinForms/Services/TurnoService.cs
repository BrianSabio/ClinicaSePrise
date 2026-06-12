using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class TurnoService
{
    private readonly ApiClient _apiClient;

    public TurnoService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TurnoDTO>> ObtenerTodosAsync()
    {
        var result = await _apiClient.GetAsync<List<TurnoDTO>>("/api/turnos");
        return result ?? new List<TurnoDTO>();
    }

    public async Task<TurnoDTO?> ObtenerPorIdAsync(int id)
    {
        return await _apiClient.GetAsync<TurnoDTO>($"/api/turnos/{id}");
    }

    public async Task<TurnoDTO?> CrearAsync(CreateTurnoRequest dto)
    {
        return await _apiClient.PostAsync<TurnoDTO>("/api/turnos", dto);
    }

    public async Task<TurnoDTO?> ConfirmarAsync(int id)
    {
        // El servidor valida que id del path == IdTurno del body
        return await _apiClient.PatchAsync<TurnoDTO>($"/api/turnos/{id}/confirmar", new { IdTurno = id });
    }

    public async Task<TurnoDTO?> CancelarAsync(int id)
    {
        // El servidor valida que id del path == IdTurno del body
        return await _apiClient.PatchAsync<TurnoDTO>($"/api/turnos/{id}/cancelar", new { IdTurno = id });
    }

    public async Task<TurnoDTO?> ReprogramarAsync(int id, DateTime nuevaFecha, int duracion)
    {
        var req = new ReprogramarTurnoRequest { NuevaFechaHoraInicio = nuevaFecha, DuracionMinutos = duracion };
        return await _apiClient.PatchAsync<TurnoDTO>($"/api/turnos/{id}/reprogramar", req);
    }
}
