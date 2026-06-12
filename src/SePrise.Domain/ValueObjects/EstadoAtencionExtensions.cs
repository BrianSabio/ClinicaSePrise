namespace SePrise.Domain.ValueObjects;
public static class EstadoAtencionExtensions
{
    public static bool EsTerminal(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Finalizada
               or EstadoAtencion.Cancelada;
    public static bool PuedeProgresarAEnProgreso(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Acreditada;
    public static bool PuedeFinalizarse(this EstadoAtencion estado) =>
        estado is EstadoAtencion.EnProgreso;
    public static bool PuedeCancelarse(this EstadoAtencion estado) =>
        estado is EstadoAtencion.Acreditada
               or EstadoAtencion.EnProgreso;
}


