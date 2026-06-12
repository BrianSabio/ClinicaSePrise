namespace SePrise.Application.DTOs.Reportes;

using System;
public class ReporteSummaryDTO
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public int TotalAtenciones { get; set; }
    public int TotalObraSocial { get; set; }
    public int TotalParticular { get; set; }
    public int TotalPacientesUnicos { get; set; }
    public int TiempoPromedioMinutos { get; set; }
}


