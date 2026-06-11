namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Representa los posibles estados de un turno a lo largo de su ciclo de vida en el sistema.
/// Los estados terminales (Atendido, NoAsistio, Cancelado, Reprogramado) no admiten más transiciones.
/// </summary>
public enum EstadoTurno
{
    /// <summary>
    /// Turno creado y agendado, pendiente de acreditación en recepción.
    /// Es el estado inicial de todo turno en el sistema.
    /// </summary>
    Reservado = 1,

    /// <summary>
    /// Paciente acreditado exitosamente en recepción. Se creó la atención asociada.
    /// Transición válida desde: Reservado.
    /// </summary>
    Confirmado = 2,

    /// <summary>
    /// Atención médica completada correctamente. El ciclo del turno ha concluido.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    Atendido = 3,

    /// <summary>
    /// El paciente no se presentó al turno sin haber acreditado previamente en recepción.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    NoAsistio = 4,

    /// <summary>
    /// Turno anulado antes de que se completara la atención médica.
    /// Puede cancelarse desde estado Reservado o Confirmado.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    Cancelado = 5,

    /// <summary>
    /// Turno derivado a un nuevo horario. El turno actual queda cerrado
    /// y se genera un nuevo turno en estado Reservado.
    /// Estado terminal — no admite más transiciones.
    /// </summary>
    Reprogramado = 6
}
