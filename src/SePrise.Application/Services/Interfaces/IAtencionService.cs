namespace SePrise.Application.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.Application.DTOs.Atencion;

/// <summary>
/// Interfaz para el servicio de aplicación de atenciones médicas.
/// Maneja el flujo de pacientes en consultorio.
/// </summary>
public interface IAtencionService
{
    /// <summary>
    /// Inicia una atención (médico comienza a atender paciente).
    /// </summary>
    Task<AtencionDTO> IniciarAtencionAsync(int idAtencion);

    /// <summary>
    /// Finaliza una atención (médico completa la consulta).
    /// </summary>
    Task<AtencionDTO> FinalizarAtencionAsync(int idAtencion, string? notas = null);

    /// <summary>
    /// Cancela una atención en progreso o acreditada.
    /// </summary>
    Task<AtencionDTO> CancelarAtencionAsync(int idAtencion);

    /// <summary>
    /// Actualiza las notas de una atención en progreso.
    /// </summary>
    Task<AtencionDTO> ActualizarNotasAsync(int idAtencion, string notas);

    /// <summary>
    /// Obtiene la lista de pacientes esperando para un médico específico.
    /// </summary>
    Task<IEnumerable<AtencionDTO>> ObtenerPacientesEsperandoAsync(int idMedico);

    /// <summary>
    /// Obtiene una atención por su ID.
    /// </summary>
    Task<AtencionDTO> ObtenerAtencionAsync(int idAtencion);

    /// <summary>
    /// Lista todas las atenciones (o filtra).
    /// </summary>
    Task<IEnumerable<AtencionDTO>> ListarAtencionesAsync(string? estado, int? idPaciente, int? idMedico);
}
