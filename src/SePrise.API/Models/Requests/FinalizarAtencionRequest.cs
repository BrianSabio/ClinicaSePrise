namespace SePrise.API.Models.Requests;

public class FinalizarAtencionRequest
{
    public int IdAtencion { get; set; }
    public string? Notas { get; set; }
}
