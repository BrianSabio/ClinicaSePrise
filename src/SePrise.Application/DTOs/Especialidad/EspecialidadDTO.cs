namespace SePrise.Application.DTOs.Especialidad;
public class EspecialidadDTO
{
public int IdEspecialidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionMinutos { get; set; }
    public bool PermiteMultiplesTurnos { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}


