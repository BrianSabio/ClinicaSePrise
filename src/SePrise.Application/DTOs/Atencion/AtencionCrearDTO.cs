namespace SePrise.Application.DTOs.Atencion;

/// <summary>
/// DTO para crear una atención a partir de un turno previamente reservado.
/// Utilizado durante la acreditación del paciente en recepción.
/// </summary>
public class AtencionCrearDTO
{
    /// <summary>
    /// ID del turno a confirmar/acreditar.
    /// </summary>
    public int IdTurno { get; set; }

    /// <summary>
    /// ID del paciente.
    /// </summary>
    public int IdPaciente { get; set; }

    /// <summary>
    /// ID del médico.
    /// </summary>
    public int IdMedico { get; set; }

    /// <summary>
    /// Modalidad de pago indicada por el paciente al momento de presentarse.
    /// Debe mapear al enum ModalidadPago.
    /// </summary>
    public string ModalidadPago { get; set; } = string.Empty;
}
