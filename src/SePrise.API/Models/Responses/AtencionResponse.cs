namespace SePrise.API.Models.Responses;

using System;

public class AtencionResponse
{
    public int Id { get; set; }
    public int? IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string ModalidadPago { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string PacienteNombre { get; set; } = string.Empty;
    public string MedicoNombre { get; set; } = string.Empty;
    public string EspecialidadNombre { get; set; } = string.Empty;
    public DateTime FechaHoraAcreditacion { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaUltimaModificacion { get; set; }
}
