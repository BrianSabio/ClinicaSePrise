namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Representa los posibles estados de una atención médica durante su ciclo de vida.
/// Los estados terminales (Finalizada, Cancelada) no admiten más transiciones.
/// </summary>
public enum EstadoAtencion
{
    /// <summary>
    /// Paciente acreditado en recepción. La atención fue creada pero aún no inició.
    /// Es el estado inicial de toda atención.
    /// </summary>
    Acreditada = 1,

    /// <summary>
    /// El médico está atendiendo al paciente en este momento.
    /// Transición válida desde: Acreditada.
    /// </summary>
    EnProgreso = 2,

    /// <summary>
    /// Atención médica completada correctamente. El médico cerró el acto clínico.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    Finalizada = 3,

    /// <summary>
    /// La atención no pudo completarse y fue cancelada.
    /// Puede cancelarse desde estado Acreditada o EnProgreso.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    Cancelada = 4
}
