namespace SePrise.Application.DTOs.Sala;

/// <summary>
/// DTO de lectura para la entidad Sala.
/// </summary>
public class SalaDTO
{
    /// <summary>
    /// Identificador único de la sala.
    /// </summary>
    public int IdSala { get; set; }

    /// <summary>
    /// Número o identificador visual de la sala.
    /// </summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de sala (ej: "Consultorio", "Procedimientos", "Espera").
    /// </summary>
    public string TipoSala { get; set; } = string.Empty;

    /// <summary>
    /// Indica si la sala está activa y disponible para uso.
    /// </summary>
    public bool Activo { get; set; }
}
