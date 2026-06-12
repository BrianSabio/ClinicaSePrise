namespace SePrise.API.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.Services.Interfaces;
using SePrise.API.Models.Requests;
using SePrise.API.Models.Responses;
using SePrise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    private readonly IAgendamientoService _agendamientoService;
    private readonly IMapper _mapper;
public TurnosController(IAgendamientoService agendamientoService, IMapper mapper)
    {
        _agendamientoService = agendamientoService ?? throw new ArgumentNullException(nameof(agendamientoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    [HttpPost]
    public async Task<ActionResult<TurnoResponse>> CreateTurno([FromBody] CreateTurnoRequest request)
    {
        try
        {
            var dto = _mapper.Map<TurnoCrearDTO>(request);
            var result = await _agendamientoService.CrearTurnoAsync(dto);
            var response = _mapper.Map<TurnoResponse>(result);
            return CreatedAtAction(nameof(GetTurno), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TurnoException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (MedicoException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (EspecialidadException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (SalaException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet("{id}")]
    public async Task<ActionResult<TurnoResponse>> GetTurno(int id)
    {
        try
        {
            var result = await _agendamientoService.ObtenerTurnoAsync(id);
            var response = _mapper.Map<TurnoResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TurnoException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurnoResponse>>> GetTurnos(
        [FromQuery] int? idPaciente, 
        [FromQuery] int? idMedico, 
        [FromQuery] string? estado)
    {
        try
        {
            IEnumerable<TurnoDTO> turnos;

            if (idPaciente.HasValue)
            {
                // Filtrar por paciente específico
                turnos = await _agendamientoService.ListarTurnosPorPacienteAsync(idPaciente.Value);
            }
            else if (idMedico.HasValue)
            {
                // Filtrar por médico (turnos del día)
                turnos = await _agendamientoService.ListarTurnosDelDiaPorMedicoAsync(idMedico.Value, DateTime.UtcNow.Date);
            }
            else
            {
                // Sin filtros: listar todos los turnos de todos los pacientes
                turnos = await _agendamientoService.ListarTodosTurnosAsync();
            }

            var response = _mapper.Map<IEnumerable<TurnoResponse>>(turnos);
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
    [HttpPatch("{id}/confirmar")]
    public async Task<ActionResult<TurnoResponse>> ConfirmarTurno(int id, [FromBody] ConfirmTurnoRequest request)
    {
        try
        {
            if (id != request.IdTurno)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var result = await _agendamientoService.ConfirmarTurnoAsync(id);
            var response = _mapper.Map<TurnoResponse>(result);
            return Ok(response);
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
    [HttpPatch("{id}/cancelar")]
    public async Task<ActionResult> CancelarTurno(int id, [FromBody] CancelTurnoRequest request)
    {
        try
        {
            if (id != request.IdTurno)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            await _agendamientoService.CancelarTurnoAsync(id);
            return Ok();
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
    [HttpPatch("{id}/reprogramar")]
    public async Task<ActionResult<TurnoResponse>> ReprogramarTurno(int id, [FromBody] RescheduleTurnoRequest request)
    {
        try
        {
            if (id != request.IdTurno)
                return BadRequest(new { error = "El ID de la ruta no coincide con el cuerpo." });

            var dto = _mapper.Map<TurnoReprogramarDTO>(request);
            var result = await _agendamientoService.ReprogramarTurnoAsync(id, dto);
            var response = _mapper.Map<TurnoResponse>(result);
            return Ok(response);
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
}


