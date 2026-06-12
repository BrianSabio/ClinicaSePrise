namespace SePrise.Application.Services.Interfaces;
public interface IAcreditacionService
{
    System.Threading.Tasks.Task<SePrise.Application.DTOs.Atencion.AtencionDTO> AcreditarPacienteDesdeReservaAsync(int idTurno, SePrise.Domain.ValueObjects.ModalidadPago modalidadPago);
    System.Threading.Tasks.Task<SePrise.Application.DTOs.Atencion.AtencionDTO> RegistrarDemandaEspontaneaAsync(SePrise.Application.DTOs.Atencion.AtencionCrearEspontaneaDTO dto);
    System.Threading.Tasks.Task RegistrarNoAsistioAsync(int idTurno);
}


