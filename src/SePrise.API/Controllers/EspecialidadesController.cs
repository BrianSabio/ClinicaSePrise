using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.API.Models.Responses;

namespace SePrise.API.Controllers;

/// <summary>
/// Controlador REST para la consulta del catálogo de especialidades médicas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EspecialidadesController : ControllerBase
{
    private readonly IEspecialidadRepository _especialidadRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa el controller inyectando el repositorio y el mapper.
    /// </summary>
    public EspecialidadesController(IEspecialidadRepository especialidadRepository, IMapper mapper)
    {
        _especialidadRepository = especialidadRepository ?? throw new ArgumentNullException(nameof(especialidadRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Retorna todas las especialidades activas en el sistema.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EspecialidadResponse>>> GetEspecialidades()
    {
        try
        {
            var especialidades = await _especialidadRepository.GetAllActivasAsync();
            var response = _mapper.Map<IEnumerable<EspecialidadResponse>>(especialidades);
            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Retorna una especialidad por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EspecialidadResponse>> GetEspecialidad(int id)
    {
        try
        {
            var especialidad = await _especialidadRepository.GetByIdAsync(id);
            if (especialidad == null)
                return NotFound(new { error = $"Especialidad con ID {id} no encontrada" });

            var response = _mapper.Map<EspecialidadResponse>(especialidad);
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
