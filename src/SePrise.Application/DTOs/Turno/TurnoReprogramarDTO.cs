namespace SePrise.Application.DTOs.Turno;

/// <summary>
/// DTO para la reprogramación de un turno.
/// Contiene los datos necesarios para generar la nueva reserva.
/// </summary>
public class TurnoReprogramarDTO
{
    /// <summary>
    /// Nueva fecha y hora de inicio solicitada.
    /// </summary>
    public DateTime FechaHoraInicio { get; set; }

    /// <summary>
    /// Nueva duración solicitada en minutos.
    /// </summary>
    public int DuracionMinutos { get; set; }
}
