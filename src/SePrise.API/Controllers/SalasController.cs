using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.API.Models.Responses;

namespace SePrise.API.Controllers;

/// <summary>
/// Controlador REST para la consulta del catálogo de salas y consultorios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly ISalaRepository _salaRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa el controller inyectando el repositorio y el mapper.
    /// </summary>
    public SalasController(ISalaRepository salaRepository, IMapper mapper)
    {
        _salaRepository = salaRepository ?? throw new ArgumentNullException(nameof(salaRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Retorna todas las salas activas del sistema.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalaResponse>>> GetSalas()
    {
        try
        {
            var salas = await _salaRepository.GetAllActivasAsync();
            var response = _mapper.Map<IEnumerable<SalaResponse>>(salas);
            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Retorna una sala por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SalaResponse>> GetSala(int id)
    {
        try
        {
            var sala = await _salaRepository.GetByIdAsync(id);
            if (sala == null)
                return NotFound(new { error = $"Sala con ID {id} no encontrada" });

            var response = _mapper.Map<SalaResponse>(sala);
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
