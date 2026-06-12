namespace SePrise.Application.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.Application.DTOs.Paciente;
public interface IPacienteService
{
    Task<PacienteDTO> CrearPacienteAsync(PacienteCrearDTO dto);
    Task<PacienteDTO> ObtenerPacienteAsync(int idPaciente);
    Task<PacienteDTO> ObtenerPacientePorDNIAsync(string dni);
    Task<IEnumerable<PacienteDTO>> ListarPacientesActivosAsync();
    Task<PacienteDTO> ActualizarPacienteAsync(int idPaciente, PacienteActualizarDTO dto);
    Task DesactivarPacienteAsync(int idPaciente);
}


