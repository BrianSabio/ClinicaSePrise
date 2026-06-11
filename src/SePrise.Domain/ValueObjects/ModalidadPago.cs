namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Representa la modalidad de cobertura o pago declarada por el paciente al acreditarse.
/// Se registra en la entidad Atencion, no en el Turno.
/// </summary>
public enum ModalidadPago
{
    /// <summary>
    /// La atención será cubierta por la obra social o prepaga del paciente.
    /// Requiere validación de afiliación en el sistema de cobertura.
    /// </summary>
    ObraSocial = 1,

    /// <summary>
    /// El paciente abona la atención de forma particular, sin cobertura médica.
    /// </summary>
    Particular = 2
}
