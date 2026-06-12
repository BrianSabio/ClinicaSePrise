namespace SePrise.Application.DTOs.Atencion;

/// <summary>
/// DTO de lectura completa para una Atención.
/// </summary>
public class AtencionDTO
{
    /// <summary>
    /// Identificador único de la atención.
    /// </summary>
    public int IdAtencion { get; set; }

    /// <summary>
    /// ID del turno asociado (puede ser nulo si es atención espontánea).
    /// </summary>
    public int? IdTurno { get; set; }

    /// <summary>
    /// ID del paciente atendido.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// ID del médico que atiende.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// Modalidad de pago declarada por el paciente (ej: "ObraSocial", "Particular").
    /// </summary>
    public string ModalidadPago { get; set; } = string.Empty;

    /// <summary>
    /// Estado actual de la atención (ej: "Acreditada", "EnProgreso", "Finalizada", "Cancelada").
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en la que el paciente se acreditó en recepción.
    /// </summary>
    public DateTime FechaHoraAcreditacion { get; set; }

    /// <summary>
    /// Fecha y hora en la que el médico inició la atención.
    /// </summary>
    public DateTime? FechaHoraInicio { get; set; }

    /// <summary>
    /// Fecha y hora en la que el médico finalizó la atención.
    /// </summary>
    public DateTime? FechaHoraFin { get; set; }

    /// <summary>
    /// Notas clínicas o comentarios adicionales ingresados por el médico.
    /// </summary>
    public string? Notas { get; set; }

    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación en el sistema.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
