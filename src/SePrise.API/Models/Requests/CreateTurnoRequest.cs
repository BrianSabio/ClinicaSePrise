namespace SePrise.API.Models.Requests;

using System;

public class CreateTurnoRequest
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public int IdEspecialidad { get; set; }
    public int IdSala { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
}


