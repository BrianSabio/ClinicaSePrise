namespace SePrise.Application.DTOs.Atencion;

/// <summary>
/// DTO para actualizar notas de una atención.
/// </summary>
public class AtencionActualizarNotasDTO
{
    public int IdAtencion { get; set; }
    public string Notas { get; set; } = string.Empty;
}
