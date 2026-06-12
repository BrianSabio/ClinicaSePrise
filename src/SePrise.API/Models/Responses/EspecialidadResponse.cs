namespace SePrise.API.Models.Responses;

/// <summary>
/// Modelo de respuesta HTTP con los datos de una especialidad médica.
/// Incluye DuracionMinutos para que el Frontend pre-rellene la duración del turno.
/// </summary>
public class EspecialidadResponse
{
    /// <summary>Identificador único de la especialidad.</summary>
    public int Id { get; set; }

    /// <summary>Nombre descriptivo de la especialidad (ej: "Cardiología").</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción opcional de la especialidad.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Duración estándar en minutos por defecto para turnos de esta especialidad.</summary>
    public int DuracionMinutos { get; set; }
}
