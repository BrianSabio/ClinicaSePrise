namespace SePrise.API.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SePrise.Application.Services.Interfaces;
using SePrise.API.Models.Responses;
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReportesService _reportesService;
    private readonly IMapper _mapper;
public ReportesController(IReportesService reportesService, IMapper mapper)
    {
        _reportesService = reportesService ?? throw new ArgumentNullException(nameof(reportesService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
[HttpGet("por-fecha")]
    public async Task<ActionResult<IEnumerable<AtencionResponse>>> ObtenerPorFecha(
        [FromQuery] DateTime fechaDesde,
        [FromQuery] DateTime fechaHasta)
    {
        try
        {
            if (fechaDesde > fechaHasta)
                return BadRequest(new { error = "fechaDesde no puede ser mayor a fechaHasta" });

            var atenciones = await _reportesService.ObtenerAtencionesPorFechaAsync(fechaDesde, fechaHasta);
            var response = _mapper.Map<IEnumerable<AtencionResponse>>(atenciones);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet("por-medico")]
    public async Task<ActionResult<IEnumerable<AtencionResponse>>> ObtenerPorMedico(
        [FromQuery] int idMedico,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        try
        {
            if (idMedico <= 0)
                return BadRequest(new { error = "idMedico debe ser mayor a 0" });

            // En un caso real podrías pasar fechaDesde/Hasta al servicio
            var atenciones = await _reportesService.ObtenerAtencionesPorMedicoAsync(idMedico);
            var response = _mapper.Map<IEnumerable<AtencionResponse>>(atenciones);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet("por-especialidad")]
    public async Task<ActionResult<IEnumerable<AtencionResponse>>> ObtenerPorEspecialidad(
        [FromQuery] int idEspecialidad,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        try
        {
            if (idEspecialidad <= 0)
                return BadRequest(new { error = "idEspecialidad debe ser mayor a 0" });

            var atenciones = await _reportesService.ObtenerAtencionesPorEspecialidadAsync(idEspecialidad);
            var response = _mapper.Map<IEnumerable<AtencionResponse>>(atenciones);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet("resumen")]
    public async Task<ActionResult<ReporteSummaryResponse>> ObtenerResumen(
        [FromQuery] DateTime fechaDesde,
        [FromQuery] DateTime fechaHasta)
    {
        try
        {
            if (fechaDesde > fechaHasta)
                return BadRequest(new { error = "fechaDesde no puede ser mayor a fechaHasta" });

            var resumen = await _reportesService.ObtenerResumenPorFechaAsync(fechaDesde, fechaHasta);
            var response = _mapper.Map<ReporteSummaryResponse>(resumen);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}


