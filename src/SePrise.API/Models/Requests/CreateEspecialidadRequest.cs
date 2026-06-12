namespace SePrise.API.Models.Requests;

public class CreateEspecialidadRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public bool PermiteMultiplesTurnos { get; set; }
}


