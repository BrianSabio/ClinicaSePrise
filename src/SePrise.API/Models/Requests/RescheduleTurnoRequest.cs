namespace SePrise.API.Models.Requests;

using System;

public class RescheduleTurnoRequest
{
    public int IdTurno { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public int DuracionMinutos { get; set; }
}


