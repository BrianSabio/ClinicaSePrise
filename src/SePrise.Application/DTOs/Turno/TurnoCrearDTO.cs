namespace SePrise.Application.DTOs.Turno;

/// <summary>
/// DTO para la creación (reserva) de un nuevo turno.
/// </summary>
public class TurnoCrearDTO
{
    /// <summary>
    /// ID del paciente.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// ID del médico.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// ID de la especialidad.
    /// </summary>
    public int IdEspecialidad { get; set; }

    /// <summary>
    /// ID de la sala.
    /// </summary>
    public int IdSala { get; set; }

    /// <summary>
    /// Fecha y hora de inicio solicitada.
    /// </summary>
    public DateTime FechaHoraInicio { get; set; }

    /// <summary>
    /// Duración solicitada en minutos.
    /// </summary>
    public int DuracionMinutos { get; set; }
}
