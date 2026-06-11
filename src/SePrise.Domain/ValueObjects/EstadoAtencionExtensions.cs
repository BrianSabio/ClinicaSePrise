namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Métodos de extensión para el enum <see cref="EstadoAtencion"/>.
/// Encapsulan las reglas de transición de la máquina de estados de la Atención,
/// centralizando la lógica en un único lugar del dominio.
/// </summary>
public static class EstadoAtencionExtensions
{
    /// <summary>
    /// Indica si el estado actual es terminal, es decir, no admite más transiciones.
    /// Los estados terminales son: <see cref="EstadoAtencion.Finalizada"/>
    /// y <see cref="EstadoAtencion.Cancelada"/>.
    /// </summary>
    /// <param name="estado">El estado de la atención a evaluar.</param>
    /// <returns>
    /// <c>true</c> si el estado es terminal y no permite transiciones;
    /// <c>false</c> si la atención aún puede cambiar de estado.
    /// </returns>
    public static bool EsTerminal(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Finalizada
               or EstadoAtencion.Cancelada;

    /// <summary>
    /// Indica si la atención puede progresar al estado <see cref="EstadoAtencion.EnProgreso"/>.
    /// Solo las atenciones en estado <see cref="EstadoAtencion.Acreditada"/> pueden iniciarse,
    /// ya que el médico debe recibir al paciente una vez acreditado.
    /// </summary>
    /// <param name="estado">El estado de la atención a evaluar.</param>
    /// <returns>
    /// <c>true</c> si la atención puede iniciarse; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeProgresarAEnProgreso(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Acreditada;

    /// <summary>
    /// Indica si la atención puede finalizar desde su estado actual.
    /// Solo las atenciones en estado <see cref="EstadoAtencion.EnProgreso"/> pueden finalizarse,
    /// ya que la finalización cierra el acto clínico en curso.
    /// </summary>
    /// <param name="estado">El estado de la atención a evaluar.</param>
    /// <returns>
    /// <c>true</c> si la atención puede finalizarse; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeFinalizarse(this EstadoAtencion estado) =>
        estado is EstadoAtencion.EnProgreso;

    /// <summary>
    /// Indica si la atención puede cancelarse desde su estado actual.
    /// La cancelación es válida desde <see cref="EstadoAtencion.Acreditada"/>
    /// o <see cref="EstadoAtencion.EnProgreso"/> (antes o durante la atención,
    /// pero nunca después de finalizar).
    /// </summary>
    /// <param name="estado">El estado de la atención a evaluar.</param>
    /// <returns>
    /// <c>true</c> si la atención puede cancelarse; <c>false</c> en caso contrario.
    /// </returns>
    public static bool PuedeCancelarse(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Acreditada
               or EstadoAtencion.EnProgreso;
}
