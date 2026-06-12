namespace SePrise.API.Models.Responses;
public class MedicoResponse
{
    public int Id { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public List<EspecialidadResponse> Especialidades { get; set; } = new();
}


