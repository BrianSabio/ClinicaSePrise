namespace SePrise.Application.Services.Interfaces;

using System.Threading.Tasks;
using SePrise.Application.DTOs.Turno;
public interface IAgendamientoService
{
Task<TurnoDTO> ObtenerTurnoAsync(int idTurno);
    Task<IEnumerable<TurnoDTO>> ListarTurnosPorPacienteAsync(int idPaciente);
    Task<IEnumerable<TurnoDTO>> ListarTurnosDelDiaPorMedicoAsync(int idMedico, DateTime fecha);
    Task<TurnoDTO> CrearTurnoAsync(TurnoCrearDTO dto);
    Task<TurnoDTO> ConfirmarTurnoAsync(int idTurno);
    Task CancelarTurnoAsync(int idTurno);
    Task<TurnoDTO> ReprogramarTurnoAsync(int idTurno, TurnoReprogramarDTO dto);
    Task<IEnumerable<TurnoDTO>> ListarTodosTurnosAsync();
}


