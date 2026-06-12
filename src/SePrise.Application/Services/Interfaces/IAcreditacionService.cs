namespace SePrise.Application.Services.Interfaces;

/// <summary>
/// Contrato del servicio de acreditación de pacientes en recepción.
/// </summary>
public interface IAcreditacionService
{
    /// <summary>
    /// Acredita a un paciente que tiene un turno reservado.
    /// Cambia el estado del turno a Confirmado y crea la Atención en estado Acreditada.
    /// </summary>
    System.Threading.Tasks.Task<SePrise.Application.DTOs.Atencion.AtencionDTO> AcreditarPacienteDesdeReservaAsync(int idTurno, SePrise.Domain.ValueObjects.ModalidadPago modalidadPago);

    /// <summary>
    /// Registra una atención por demanda espontánea (sin turno previo).
    /// </summary>
    System.Threading.Tasks.Task<SePrise.Application.DTOs.Atencion.AtencionDTO> RegistrarDemandaEspontaneaAsync(SePrise.Application.DTOs.Atencion.AtencionCrearEspontaneaDTO dto);

    /// <summary>
    /// Registra que el paciente no asistió a un turno reservado.
    /// </summary>
    System.Threading.Tasks.Task RegistrarNoAsistioAsync(int idTurno);
}
