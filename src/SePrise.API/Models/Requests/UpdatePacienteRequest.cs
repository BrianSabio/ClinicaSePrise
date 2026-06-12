namespace SePrise.API.Models.Requests;

public class UpdatePacienteRequest
{
    public string? DNI { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public char? Genero { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
}


