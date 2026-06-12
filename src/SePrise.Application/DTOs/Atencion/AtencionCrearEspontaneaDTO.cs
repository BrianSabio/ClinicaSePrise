namespace SePrise.Application.DTOs.Atencion;

/// <summary>
/// DTO para crear una atención espontánea (sin turno previo).
/// </summary>
public class AtencionCrearEspontaneaDTO
{
    /// <summary>
    /// ID del paciente a atender.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// ID del médico solicitado.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// Modalidad de pago declarada por el paciente (ej: "ObraSocial", "Particular").
    /// </summary>
    public string ModalidadPago { get; set; } = string.Empty;
}
