using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.API.Models.Responses;

namespace SePrise.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoRepository _medicoRepository;
    private readonly IMapper _mapper;
public MedicosController(IMedicoRepository medicoRepository, IMapper mapper)
    {
        _medicoRepository = medicoRepository ?? throw new ArgumentNullException(nameof(medicoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicoResponse>>> GetMedicos()
    {
        try
        {
            var medicos = await _medicoRepository.GetAllActivosAsync();

            // Cargar las especialidades de cada médico para el filtrado en el Frontend
            var medicosList = medicos.ToList();
            var responses = new List<MedicoResponse>();

            foreach (var medico in medicosList)
            {
                var especialidades = await _medicoRepository.GetEspecialidadesByMedicoAsync(medico.IdMedico);
                var response = _mapper.Map<MedicoResponse>(medico);
                response.Especialidades = _mapper.Map<List<EspecialidadResponse>>(especialidades);
                responses.Add(response);
            }

            return Ok(responses);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<MedicoResponse>> GetMedico(int id)
    {
        try
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico == null)
                return NotFound(new { error = $"Médico con ID {id} no encontrado" });

            var especialidades = await _medicoRepository.GetEspecialidadesByMedicoAsync(id);
            var response = _mapper.Map<MedicoResponse>(medico);
            response.Especialidades = _mapper.Map<List<EspecialidadResponse>>(especialidades);
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


