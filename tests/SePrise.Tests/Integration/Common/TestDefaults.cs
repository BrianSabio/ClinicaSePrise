namespace SePrise.Tests.Integration.Common;

/// <summary>
/// Constantes utilizadas frecuentemente en los tests.
/// </summary>
public static class TestDefaults
{
    public const string BaseUrl = "/api";
    public const string PacientesUrl = "/api/pacientes";
    public const string TurnosUrl = "/api/turnos";
    public const string AtencionsUrl = "/api/atenciones";
    public const string ReportesUrl = "/api/reportes";
    
    public const string DefaultDni = "12345678";
    public const string DefaultMatricula = "MP12345";
    public const string DefaultSalaNumero = "101";
    public const string DefaultEspecialidadNombre = "Clínica Médica";
    
    public const int DefaultDuracionMinutos = 30;
}
