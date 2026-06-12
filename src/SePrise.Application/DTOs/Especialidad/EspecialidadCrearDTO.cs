namespace SePrise.Application.DTOs.Especialidad;

public class EspecialidadCrearDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionMinutos { get; set; }
    public bool PermiteMultiplesTurnos { get; set; }
}
