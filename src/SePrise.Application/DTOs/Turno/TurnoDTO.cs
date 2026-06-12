namespace SePrise.Application.DTOs.Turno;

/// <summary>
/// DTO de lectura completa para un Turno.
/// </summary>
public class TurnoDTO
{
    /// <summary>
    /// Identificador único del turno.
    /// </summary>
    public int IdTurno { get; set; }

    /// <summary>
    /// ID del paciente asignado.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// ID del médico que atenderá.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// ID de la especialidad.
    /// </summary>
    public int IdEspecialidad { get; set; }

    /// <summary>
    /// ID de la sala donde se realizará la atención.
    /// </summary>
    public int IdSala { get; set; }

    /// <summary>
    /// Fecha y hora de inicio del turno.
    /// </summary>
    public DateTime FechaHoraInicio { get; set; }

    /// <summary>
    /// Duración esperada en minutos.
    /// </summary>
    public int DuracionMinutos { get; set; }

    /// <summary>
    /// Estado actual del turno (ej: "Reservado", "Confirmado", "Cancelado", etc.).
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en la que el turno fue creado.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
