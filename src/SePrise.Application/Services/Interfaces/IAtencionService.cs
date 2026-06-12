namespace SePrise.Application.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using SePrise.Application.DTOs.Atencion;
public interface IAtencionService
{
    Task<AtencionDTO> IniciarAtencionAsync(int idAtencion);
    Task<AtencionDTO> FinalizarAtencionAsync(int idAtencion, string? notas = null);
    Task<AtencionDTO> CancelarAtencionAsync(int idAtencion);
    Task<AtencionDTO> ActualizarNotasAsync(int idAtencion, string notas);
Task<IEnumerable<AtencionDTO>> ObtenerPacientesEsperandoAsync(int idMedico);
Task<AtencionDTO> ObtenerAtencionAsync(int idAtencion);
    Task<IEnumerable<AtencionDTO>> ListarAtencionesAsync(string? estado, int? idPaciente, int? idMedico);
}


