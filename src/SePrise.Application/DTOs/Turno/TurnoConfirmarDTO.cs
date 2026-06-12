namespace SePrise.Application.DTOs.Turno;

/// <summary>
/// DTO para la confirmación de un turno (acreditación del paciente).
/// Utilizado para transicionar un turno de Reservado a Confirmado.
/// </summary>
public class TurnoConfirmarDTO
{
    public int IdTurno { get; set; }
}
