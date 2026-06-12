namespace SePrise.API.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SePrise.Application.DTOs.Paciente;
using SePrise.Application.Services.Interfaces;
using SePrise.API.Models.Requests;
using SePrise.API.Models.Responses;
using SePrise.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;
    private readonly IMapper _mapper;
public PacientesController(IPacienteService pacienteService, IMapper mapper)
    {
        _pacienteService = pacienteService ?? throw new ArgumentNullException(nameof(pacienteService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    [HttpPost]
    public async Task<ActionResult<PacienteResponse>> CreatePaciente(
        [FromBody] CreatePacienteRequest request,
        [FromServices] FluentValidation.IValidator<PacienteCrearDTO> validator)
    {
        try
        {
            var dto = _mapper.Map<PacienteCrearDTO>(request);
            
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { error = validationResult.Errors[0].ErrorMessage });

            var result = await _pacienteService.CrearPacienteAsync(dto);
            var response = _mapper.Map<PacienteResponse>(result);
            return CreatedAtAction(nameof(GetPaciente), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.ToString() });
        }
    }
[HttpGet("{id}")]
    public async Task<ActionResult<PacienteResponse>> GetPaciente(int id)
    {
        try
        {
            var result = await _pacienteService.ObtenerPacienteAsync(id);
            var response = _mapper.Map<PacienteResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
[HttpGet]
    public async Task<ActionResult<IEnumerable<PacienteResponse>>> GetPacientes([FromQuery] string? dni)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(dni))
            {
                try
                {
                    var paciente = await _pacienteService.ObtenerPacientePorDNIAsync(dni);
                    return Ok(new List<PacienteResponse> { _mapper.Map<PacienteResponse>(paciente) });
                }
                catch (PacienteException)
                {
                    return Ok(new List<PacienteResponse>());
                }
            }

            var result = await _pacienteService.ListarPacientesActivosAsync();
            var response = _mapper.Map<IEnumerable<PacienteResponse>>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<PacienteResponse>> UpdatePaciente(
        int id, 
        [FromBody] UpdatePacienteRequest request,
        [FromServices] FluentValidation.IValidator<PacienteActualizarDTO>? validator = null)
    {
        try
        {
            var dto = _mapper.Map<PacienteActualizarDTO>(request);
            
            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { error = validationResult.Errors[0].ErrorMessage });
            }

            var result = await _pacienteService.ActualizarPacienteAsync(id, dto);
            var response = _mapper.Map<PacienteResponse>(result);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        try
        {
            await _pacienteService.DesactivarPacienteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (PacienteException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}


