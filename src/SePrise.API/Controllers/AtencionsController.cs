namespace SePrise.API.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.Services.Interfaces;
using SePrise.API.Models.Requests;
using SePrise.API.Models.Responses;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
[ApiController]
[Route("api/[controller]")]
public class AtencionsController : ControllerBase
{
    private readonly IAcreditacionService _acreditacionService;
    private readonly IAtencionService _atencionService;
    private readonly IMapper _mapper;
public AtencionsController(
        IAcreditacionService acreditacionService,
        IAtencionService atencionService,
        IMapper mapper)
    {
        _acreditacionService = acreditacionService ?? throw new ArgumentNullException(nameof(acreditacionService));
        _atencionService = atencionService ?? throw new ArgumentNullException(nameof(atencionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    [HttpPost("acreditar")]
    public async Task<ActionResult<AtencionResponse>> AcreditarPaciente([FromBody] AcreditarPacienteRequest request)
    {
        try
        {
            if (!Enum.TryParse<ModalidadPago>(request.ModalidadPago, out var modalidad))
                return BadRequest(new { error = "Modalidad de pago inválida" });

            var result = await _acreditacionService.AcreditarPacienteDesdeReservaAsync(request.IdTurno, modalidad);
            var response = _mapper.Map<AtencionResponse>(result);
            return CreatedAtAction(nameof(GetAtencion), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TurnoException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPost("demanda-espontanea")]
    public async Task<ActionResult<AtencionResponse>> CrearDemandaEspontanea([FromBody] CrearDemandaEspontaneaRequest request)
    {
        try
        {
            var dto = _mapper.Map<AtencionCrearEspontaneaDTO>(request);
            var result = await _acreditacionService.RegistrarDemandaEspontaneaAsync(dto);
            var response = _mapper.Map<AtencionResponse>(result);
            return CreatedAtAction(nameof(GetAtencion), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (MedicoException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPost("{idTurno}/no-asistio")]
    public async Task<ActionResult> RegistrarNoAsistio(int idTurno)
    {
        try
        {
            await _acreditacionService.RegistrarNoAsistioAsync(idTurno);
            return Ok(new { estado = "NoAsistio" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TurnoException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPatch("{id}/iniciar")]
    public async Task<ActionResult<AtencionResponse>> IniciarAtencion(int id, [FromBody] IniciarAtencionRequest request)
    {
        try
        {
            if (id != request.IdAtencion)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var result = await _atencionService.IniciarAtencionAsync(id);
            var response = _mapper.Map<AtencionResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPatch("{id}/finalizar")]
    public async Task<ActionResult<AtencionResponse>> FinalizarAtencion(int id, [FromBody] FinalizarAtencionRequest request)
    {
        try
        {
            if (id != request.IdAtencion)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var result = await _atencionService.FinalizarAtencionAsync(id, request.Notas);
            var response = _mapper.Map<AtencionResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPatch("{id}/cancelar")]
    public async Task<ActionResult<AtencionResponse>> CancelarAtencion(int id, [FromBody] CancelarAtencionRequest request)
    {
        try
        {
            if (id != request.IdAtencion)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var result = await _atencionService.CancelarAtencionAsync(id);
            var response = _mapper.Map<AtencionResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet("{id}")]
    public async Task<ActionResult<AtencionResponse>> GetAtencion(int id)
    {
        try
        {
            var result = await _atencionService.ObtenerAtencionAsync(id);
            var response = _mapper.Map<AtencionResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet]
    public async Task<ActionResult<IEnumerable<AtencionResponse>>> GetAtenciones(
        [FromQuery] string? estado,
        [FromQuery] int? idPaciente,
        [FromQuery] int? idMedico)
    {
        try
        {
            var result = await _atencionService.ListarAtencionesAsync(estado, idPaciente, idMedico);
            var response = _mapper.Map<IEnumerable<AtencionResponse>>(result);
            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPatch("{id}/notas")]
    public async Task<ActionResult<AtencionResponse>> ActualizarNotas(int id, [FromBody] ActualizarNotasAtencionRequest request)
    {
        try
        {
            if (id != request.IdAtencion)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var result = await _atencionService.ActualizarNotasAsync(id, request.Notas);
            var response = _mapper.Map<AtencionResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (AtencionException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}


