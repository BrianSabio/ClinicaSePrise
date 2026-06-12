namespace SePrise.Application.DTOs.Medico;
public class MedicoDTO
{
public int IdMedico { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }
}


