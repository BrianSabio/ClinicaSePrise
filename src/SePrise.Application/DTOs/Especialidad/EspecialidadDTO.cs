namespace SePrise.Application.DTOs.Especialidad;

/// <summary>
/// DTO de lectura para la entidad Especialidad.
/// </summary>
public class EspecialidadDTO
{
    /// <summary>
    /// Identificador de la especialidad.
    /// </summary>
    public int IdEspecialidad { get; set; }

    /// <summary>
    /// Nombre de la especialidad.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la especialidad.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Duración en minutos por defecto de los turnos para esta especialidad.
    /// </summary>
    public int DuracionMinutos { get; set; }

    /// <summary>
    /// Indica si se permiten reservar múltiples turnos en el mismo bloque horario.
    /// </summary>
    public bool PermiteMultiplesTurnos { get; set; }

    /// <summary>
    /// Indica si la especialidad está activa.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de creación en el sistema.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
