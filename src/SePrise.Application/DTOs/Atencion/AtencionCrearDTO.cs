namespace SePrise.Application.DTOs.Atencion;
public class AtencionCrearDTO
{
    public int IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}


