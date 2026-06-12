namespace SePrise.Application.DTOs.Turno;
public class TurnoDTO
{
public int IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public int IdSala { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
    public string Estado { get; set; } = string.Empty;

    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}


