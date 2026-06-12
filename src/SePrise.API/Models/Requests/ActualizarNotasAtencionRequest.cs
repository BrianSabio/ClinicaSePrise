namespace SePrise.API.Models.Requests;

public class ActualizarNotasAtencionRequest
{
    public int IdAtencion { get; set; }
    public string Notas { get; set; } = string.Empty;
}


