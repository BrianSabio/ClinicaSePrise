namespace SePrise.Application.DTOs.Medico;

/// <summary>
/// DTO de lectura para la entidad Medico.
/// </summary>
public class MedicoDTO
{
    /// <summary>
    /// Identificador único del médico.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// Número de matrícula profesional.
    /// </summary>
    public string NumeroMatricula { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del médico.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del médico.
    /// </summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto del médico.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Teléfono de contacto del médico.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Indica si el médico está activo en la clínica.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de alta del médico.
    /// </summary>
    public DateTime FechaAlta { get; set; }
}
