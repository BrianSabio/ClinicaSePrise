namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Métodos de extensión para el enum <see cref="EstadoTurno"/>.
/// Encapsulan las reglas de transición de la máquina de estados del Turno,
/// centralizando la lógica en un único lugar del dominio.
/// </summary>
public static class EstadoTurnoExtensions
{
    /// <summary>
    /// Indica si el estado actual es terminal, es decir, no admite más transiciones.
    /// Los estados terminales son: <see cref="EstadoTurno.Atendido"/>,
    /// <see cref="EstadoTurno.NoAsistio"/>, <see cref="EstadoTurno.Cancelado"/>
    /// y <see cref="EstadoTurno.Reprogramado"/>.
    /// </summary>
    /// <param name="estado">El estado del turno a evaluar.</param>
    /// <returns>
    /// <c>true</c> si el estado es terminal y no permite transiciones;
    /// <c>false</c> si el turno aún puede cambiar de estado.
    /// </returns>
    public static bool EsTerminal(this EstadoTurno estado) =>
        estado is EstadoTurno.Atendido
               or EstadoTurno.NoAsistio
               or EstadoTurno.Cancelado
               or EstadoTurno.Reprogramado;

    /// <summary>
    /// Indica si el turno puede ser cancelado desde su estado actual.
    /// La cancelación es válida únicamente desde <see cref="EstadoTurno.Reservado"/>
    /// o <see cref="EstadoTurno.Confirmado"/>.
    /// </summary>
    /// <param name="estado">El estado del turno a evaluar.</param>
    /// <returns>
    /// <c>true</c> si el turno admite cancelación; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeCancelarse(this EstadoTurno estado) =>
        estado is EstadoTurno.Reservado
               or EstadoTurno.Confirmado;

    /// <summary>
    /// Indica si el turno puede ser confirmado (acreditado en recepción) desde su estado actual.
    /// Solo los turnos en estado <see cref="EstadoTurno.Reservado"/> pueden confirmarse.
    /// </summary>
    /// <param name="estado">El estado del turno a evaluar.</param>
    /// <returns>
    /// <c>true</c> si el turno puede confirmarse; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeConfirmarse(this EstadoTurno estado) =>
        estado is EstadoTurno.Reservado;

    /// <summary>
    /// Indica si el turno puede progresar al estado <see cref="EstadoTurno.Atendido"/>.
    /// Solo los turnos en estado <see cref="EstadoTurno.Confirmado"/> pueden marcarse como atendidos,
    /// ya que la atención requiere acreditación previa.
    /// </summary>
    /// <param name="estado">El estado del turno a evaluar.</param>
    /// <returns>
    /// <c>true</c> si el turno puede pasar a Atendido; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeProgresarAAtendido(this EstadoTurno estado) =>
        estado is EstadoTurno.Confirmado;
}
