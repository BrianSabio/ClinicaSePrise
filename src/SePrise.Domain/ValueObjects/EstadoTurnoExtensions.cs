namespace SePrise.Domain.ValueObjects;
public static class EstadoTurnoExtensions
{
    public static bool EsTerminal(this EstadoTurno estado) =>
        estado is EstadoTurno.Atendido
               or EstadoTurno.NoAsistio
               or EstadoTurno.Cancelado
               or EstadoTurno.Reprogramado;
    public static bool PuedeCancelarse(this EstadoTurno estado) =>
        estado is EstadoTurno.Reservado
               or EstadoTurno.Confirmado;
    public static bool PuedeConfirmarse(this EstadoTurno estado) =>
        estado is EstadoTurno.Reservado;
    public static bool PuedeProgresarAAtendido(this EstadoTurno estado) =>
        estado is EstadoTurno.Confirmado;
}


