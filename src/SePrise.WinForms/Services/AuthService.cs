using SePrise.WinForms.Models;

namespace SePrise.WinForms.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;
    public string? UsuarioDNI { get; set; }
    public bool EstaAutenticado => !string.IsNullOrEmpty(UsuarioDNI);
    // Cambiar este valor por el DNI que se quiera usar para testing.
    // Para deshabilitar el bypass, dejar en null o string vacío.
    private const string DNI_PRUEBA = "12345678";

    public AuthService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<bool> LoginAsync(string dni)
    {
        // Bypass de autenticación para pruebas: si el DNI coincide, ingresa sin llamar a la API
        if (!string.IsNullOrWhiteSpace(DNI_PRUEBA) && dni == DNI_PRUEBA)
        {
            UsuarioDNI = dni;
            return true;
        }

        try
        {
            var result = await _apiClient.GetAsync<IEnumerable<PacienteDTO>>($"/api/pacientes?dni={dni}");
            
            if (result != null && result.Any())
            {
                UsuarioDNI = dni;
                return true;
            }
        }
        catch 
        {
        }
        return false;
    }

    public void Logout()
    {
        UsuarioDNI = null;
    }
}


