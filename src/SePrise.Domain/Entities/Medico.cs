namespace SePrise.Domain.Entities;
public class Medico : Entity
{
public int IdMedico { get; private set; }
    public string NumeroMatricula { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaAlta { get; private set; }
    private Medico() { }
    public static Medico Crear(
        string numeroMatricula,
        string nombre,
        string apellido,
        string? email = null,
        string? telefono = null)
    {
        if (string.IsNullOrWhiteSpace(numeroMatricula))
            throw new ArgumentException(
                "El número de matrícula no puede ser nulo ni estar vacío.",
                nameof(numeroMatricula));

        if (numeroMatricula.Trim().Length > 50)
            throw new ArgumentException(
                $"El número de matrícula no puede exceder 50 caracteres. " +
                $"Se recibieron {numeroMatricula.Trim().Length} caracteres.",
                nameof(numeroMatricula));
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del médico no puede ser nulo ni estar vacío.",
                nameof(nombre));

        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido del médico no puede ser nulo ni estar vacío.",
                nameof(apellido));

        if (apellido.Trim().Length > 100)
            throw new ArgumentException(
                $"El apellido no puede exceder 100 caracteres. " +
                $"Se recibieron {apellido.Trim().Length} caracteres.",
                nameof(apellido));
        if (email is not null && email.Length > 100)
            throw new ArgumentException(
                $"El email no puede exceder 100 caracteres. " +
                $"Se recibieron {email.Length} caracteres.",
                nameof(email));
        if (telefono is not null && telefono.Length > 20)
            throw new ArgumentException(
                $"El teléfono no puede exceder 20 caracteres. " +
                $"Se recibieron {telefono.Length} caracteres.",
                nameof(telefono));

        // Capturar el momento exacto de alta para consistencia
        var ahora = DateTime.UtcNow;

        return new Medico
        {
            NumeroMatricula = numeroMatricula.Trim(),
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Email = email,
            Telefono = telefono,
            Activo = true,
            FechaAlta = ahora,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}


