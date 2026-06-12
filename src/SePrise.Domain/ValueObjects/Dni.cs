using System.Text.RegularExpressions;

namespace SePrise.Domain.ValueObjects;
public sealed class Dni
{
    // Cantidad mínima de dígitos aceptada para un DNI argentino
    private const int LongitudMinima = 7;

    // Cantidad máxima de dígitos aceptada para un DNI argentino
    private const int LongitudMaxima = 9;

    // Patrón regex: solo dígitos, sin puntos ni guiones
    private static readonly Regex PatronSoloDigitos = new(@"^\d+$", RegexOptions.Compiled);
    public string Valor { get; }
    private Dni()
    {
        // EF Core pobla la propiedad Valor mediante reflexión después de instanciar
        Valor = string.Empty;
    }
    private Dni(string valor)
    {
        Valor = valor;
    }
    public static Dni Crear(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException(
                "El DNI no puede ser nulo ni estar vacío.",
                nameof(valor));

        // Eliminar espacios laterales por si el valor viene con espacios
        string valorLimpio = valor.Trim();
        if (!PatronSoloDigitos.IsMatch(valorLimpio))
            throw new ArgumentException(
                $"El DNI '{valorLimpio}' contiene caracteres no válidos. " +
                "Solo se permiten dígitos numéricos, sin puntos ni guiones.",
                nameof(valor));
        if (valorLimpio.Length < LongitudMinima || valorLimpio.Length > LongitudMaxima)
            throw new ArgumentException(
                $"El DNI '{valorLimpio}' tiene {valorLimpio.Length} dígito(s), " +
                $"pero debe tener entre {LongitudMinima} y {LongitudMaxima} dígitos.",
                nameof(valor));

        return new Dni(valorLimpio);
    }
    public override bool Equals(object? obj)
    {
        // Si el objeto es null o no es del tipo Dni, no son iguales
        if (obj is not Dni otroDni)
            return false;

        // Comparar por el valor del DNI (igualdad semántica de Value Object)
        return string.Equals(Valor, otroDni.Valor, StringComparison.Ordinal);
    }
    public override int GetHashCode() =>
        HashCode.Combine(Valor);
    public override string ToString() => Valor;
    public static bool operator ==(Dni? izquierda, Dni? derecha) =>
        izquierda?.Equals(derecha) ?? derecha is null;
    public static bool operator !=(Dni? izquierda, Dni? derecha) =>
        !(izquierda == derecha);
}


