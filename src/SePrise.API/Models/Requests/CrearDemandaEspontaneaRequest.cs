namespace SePrise.API.Models.Requests;

public class CrearDemandaEspontaneaRequest
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}


