namespace SePrise.Application.Services.Interfaces;

using System.Threading.Tasks;
using SePrise.Application.DTOs.Turno;

/// <summary>
/// Interfaz para el servicio de aplicación de agendamiento de turnos.
/// </summary>
public interface IAgendamientoService
{
    /// <summary>
    /// Obtiene un turno por su ID.
    /// </summary>
    Task<TurnoDTO> ObtenerTurnoAsync(int idTurno);

    /// <summary>
    /// Lista los turnos de un paciente.
    /// </summary>
    Task<IEnumerable<TurnoDTO>> ListarTurnosPorPacienteAsync(int idPaciente);

    /// <summary>
    /// Lista los turnos de un médico en una fecha específica.
    /// </summary>
    Task<IEnumerable<TurnoDTO>> ListarTurnosDelDiaPorMedicoAsync(int idMedico, DateTime fecha);

    /// <summary>
    /// Crea un nuevo turno en estado Reservado.
    /// </summary>
    Task<TurnoDTO> CrearTurnoAsync(TurnoCrearDTO dto);

    /// <summary>
    /// Confirma un turno pasándolo de Reservado a Confirmado.
    /// </summary>
    Task<TurnoDTO> ConfirmarTurnoAsync(int idTurno);

    /// <summary>
    /// Cancela un turno existente y su atención asociada si corresponde.
    /// </summary>
    Task CancelarTurnoAsync(int idTurno);

    /// <summary>
    /// Reprograma un turno, cerrando el actual y creando uno nuevo.
    /// </summary>
    Task<TurnoDTO> ReprogramarTurnoAsync(int idTurno, TurnoReprogramarDTO dto);

    /// <summary>
    /// Lista todos los turnos del sistema sin filtros.
    /// </summary>
    Task<IEnumerable<TurnoDTO>> ListarTodosTurnosAsync();
}
