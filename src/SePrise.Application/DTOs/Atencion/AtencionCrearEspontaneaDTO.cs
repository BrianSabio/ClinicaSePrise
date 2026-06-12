namespace SePrise.Application.DTOs.Atencion;
public class AtencionCrearEspontaneaDTO
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
}


