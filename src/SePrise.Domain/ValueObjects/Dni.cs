using System.Text.RegularExpressions;

namespace SePrise.Domain.ValueObjects;

/// <summary>
/// Value Object que encapsula y valida el Documento Nacional de Identidad (DNI) argentino.
/// Es inmutable: una vez creado, su valor no puede modificarse.
/// Centraliza la lógica de validación del DNI en el dominio, siguiendo DDD.
/// </summary>
public sealed class Dni
{
    // Cantidad mínima de dígitos aceptada para un DNI argentino
    private const int LongitudMinima = 7;

    // Cantidad máxima de dígitos aceptada para un DNI argentino
    private const int LongitudMaxima = 9;

    // Patrón regex: solo dígitos, sin puntos ni guiones
    private static readonly Regex PatronSoloDigitos = new(@"^\d+$", RegexOptions.Compiled);

    /// <summary>
    /// El valor del DNI como cadena de dígitos sin formato.
    /// Solo lectura — el DNI es inmutable una vez creado.
    /// </summary>
    public string Valor { get; }

    /// <summary>
    /// Constructor privado reservado para uso interno y para Entity Framework Core.
    /// Usa el factory method <see cref="Crear"/> para instanciar desde código de aplicación.
    /// </summary>
    /// <param name="valor">El valor ya validado del DNI.</param>
    private Dni(string valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Factory method que valida el valor proporcionado y crea una instancia del Value Object DNI.
    /// Aplica todas las reglas de validación del dominio antes de construir el objeto.
    /// </summary>
    /// <param name="valor">
    /// El número de DNI como cadena. Debe contener entre 7 y 9 dígitos numéricos sin puntos ni guiones.
    /// </param>
    /// <returns>Una instancia válida de <see cref="Dni"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el valor es nulo, vacío, contiene caracteres no numéricos,
    /// o si la longitud está fuera del rango válido (7–9 dígitos).
    /// </exception>
    /// <example>
    /// <code>
    /// // ✅ Válido
    /// var dni = Dni.Crear("12345678");
    ///
    /// // ❌ Lanza ArgumentException
    /// var dni = Dni.Crear("ABC123");       // caracteres no numéricos
    /// var dni = Dni.Crear("1.234.567");    // contiene puntos
    /// var dni = Dni.Crear("");             // vacío
    /// var dni = Dni.Crear("123456");       // menos de 7 dígitos
    /// </code>
    /// </example>
    public static Dni Crear(string valor)
    {
        // Validar que el valor no sea nulo ni espacios en blanco
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException(
                "El DNI no puede ser nulo ni estar vacío.",
                nameof(valor));

        // Eliminar espacios laterales por si el valor viene con espacios
        string valorLimpio = valor.Trim();

        // Validar que contenga solo dígitos (sin puntos, guiones ni letras)
        if (!PatronSoloDigitos.IsMatch(valorLimpio))
            throw new ArgumentException(
                $"El DNI '{valorLimpio}' contiene caracteres no válidos. " +
                "Solo se permiten dígitos numéricos, sin puntos ni guiones.",
                nameof(valor));

        // Validar el rango de longitud para DNI argentino (7 a 9 dígitos)
        if (valorLimpio.Length < LongitudMinima || valorLimpio.Length > LongitudMaxima)
            throw new ArgumentException(
                $"El DNI '{valorLimpio}' tiene {valorLimpio.Length} dígito(s), " +
                $"pero debe tener entre {LongitudMinima} y {LongitudMaxima} dígitos.",
                nameof(valor));

        return new Dni(valorLimpio);
    }

    /// <summary>
    /// Compara esta instancia con otro objeto para verificar igualdad por valor.
    /// Dos instancias de <see cref="Dni"/> son iguales si tienen el mismo número.
    /// </summary>
    /// <param name="obj">El objeto a comparar.</param>
    /// <returns><c>true</c> si ambos representan el mismo DNI; <c>false</c> en caso contrario.</returns>
    public override bool Equals(object? obj)
    {
        // Si el objeto es null o no es del tipo Dni, no son iguales
        if (obj is not Dni otroDni)
            return false;

        // Comparar por el valor del DNI (igualdad semántica de Value Object)
        return string.Equals(Valor, otroDni.Valor, StringComparison.Ordinal);
    }

    /// <summary>
    /// Genera un código hash basado en el valor del DNI.
    /// Garantiza que dos instancias iguales produzcan el mismo hash.
    /// </summary>
    /// <returns>El código hash del valor del DNI.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(Valor);

    /// <summary>
    /// Devuelve la representación en cadena del DNI (el número sin formato).
    /// </summary>
    /// <returns>El valor del DNI como cadena de dígitos.</returns>
    public override string ToString() => Valor;

    /// <summary>
    /// Operador de igualdad entre dos instancias de <see cref="Dni"/>.
    /// </summary>
    public static bool operator ==(Dni? izquierda, Dni? derecha) =>
        izquierda?.Equals(derecha) ?? derecha is null;

    /// <summary>
    /// Operador de desigualdad entre dos instancias de <see cref="Dni"/>.
    /// </summary>
    public static bool operator !=(Dni? izquierda, Dni? derecha) =>
        !(izquierda == derecha);
}
