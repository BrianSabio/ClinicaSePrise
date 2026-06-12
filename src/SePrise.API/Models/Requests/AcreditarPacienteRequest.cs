namespace SePrise.API.Models.Requests;

public class AcreditarPacienteRequest
{
    public int IdTurno { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}


