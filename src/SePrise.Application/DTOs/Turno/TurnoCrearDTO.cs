namespace SePrise.Application.DTOs.Turno;
public class TurnoCrearDTO
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public int IdSala { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
}


