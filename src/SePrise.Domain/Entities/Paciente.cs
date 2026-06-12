using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Entities;
public class Paciente : Entity
{
public int IdPaciente { get; private set; }
    public Dni DNI { get; private set; } = null!;
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public DateTime FechaNacimiento { get; private set; }
    public char Genero { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public string? Ciudad { get; private set; }
    public string? Provincia { get; private set; }
    public string? CodigoPostal { get; private set; }
    public bool Activo { get; private set; }
    private Paciente() { }
    public static Paciente Crear(
        Dni dni,
        string nombre,
        string apellido,
        DateTime fechaNacimiento,
        char genero,
        string email,
        string? telefono = null,
        string? direccion = null,
        string? ciudad = null,
        string? provincia = null,
        string? codigoPostal = null)
    {
        if (dni is null)
            throw new ArgumentNullException(nameof(dni), "El DNI no puede ser nulo.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del paciente no puede ser nulo ni estar vacío.",
                nameof(nombre));

        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido del paciente no puede ser nulo ni estar vacío.",
                nameof(apellido));

        if (apellido.Trim().Length > 100)
            throw new ArgumentException(
                $"El apellido no puede exceder 100 caracteres. " +
                $"Se recibieron {apellido.Trim().Length} caracteres.",
                nameof(apellido));
        if (fechaNacimiento.Date >= DateTime.UtcNow.Date)
            throw new ArgumentException(
                $"La fecha de nacimiento '{fechaNacimiento:yyyy-MM-dd}' debe ser anterior a la fecha actual.",
                nameof(fechaNacimiento));
        char generoNormalizado = char.ToUpper(genero);
        if (generoNormalizado != 'M' && generoNormalizado != 'F' && generoNormalizado != 'O')
            throw new ArgumentException(
                $"El género '{genero}' no es válido. Solo se aceptan: 'M' (Masculino), 'F' (Femenino), 'O' (Otro).",
                nameof(genero));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "El email del paciente no puede ser nulo ni estar vacío.",
                nameof(email));

        if (email.Length > 100)
            throw new ArgumentException(
                $"El email no puede exceder 100 caracteres. " +
                $"Se recibieron {email.Length} caracteres.", nameof(email));

        if (telefono is not null && telefono.Length > 20)
            throw new ArgumentException(
                $"El teléfono no puede exceder 20 caracteres. " +
                $"Se recibieron {telefono.Length} caracteres.", nameof(telefono));

        if (direccion is not null && direccion.Length > 200)
            throw new ArgumentException(
                $"La dirección no puede exceder 200 caracteres. " +
                $"Se recibieron {direccion.Length} caracteres.", nameof(direccion));

        if (ciudad is not null && ciudad.Length > 50)
            throw new ArgumentException(
                $"La ciudad no puede exceder 50 caracteres. " +
                $"Se recibieron {ciudad.Length} caracteres.", nameof(ciudad));

        if (provincia is not null && provincia.Length > 50)
            throw new ArgumentException(
                $"La provincia no puede exceder 50 caracteres. " +
                $"Se recibieron {provincia.Length} caracteres.", nameof(provincia));

        if (codigoPostal is not null && codigoPostal.Length > 10)
            throw new ArgumentException(
                $"El código postal no puede exceder 10 caracteres. " +
                $"Se recibieron {codigoPostal.Length} caracteres.", nameof(codigoPostal));

        var ahora = DateTime.UtcNow;

        return new Paciente
        {
            DNI = dni,
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            FechaNacimiento = fechaNacimiento.Date, // Solo guardar la fecha, sin hora
            Genero = generoNormalizado,
            Email = email,
            Telefono = telefono,
            Direccion = direccion,
            Ciudad = ciudad,
            Provincia = provincia,
            CodigoPostal = codigoPostal,
            Activo = true,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }
    public void ActualizarDatos(
        string? dniStr,
        DateTime? fechaNacimiento,
        char? genero,
        string? nombre,
        string? apellido,
        string? email,
        string? telefono,
        string? direccion,
        string? ciudad,
        string? provincia,
        string? codigoPostal)
    {
        if (!string.IsNullOrWhiteSpace(dniStr))
        {
            DNI = Dni.Crear(dniStr);
        }

        if (fechaNacimiento.HasValue)
        {
            if (fechaNacimiento.Value.Date >= DateTime.UtcNow.Date)
                throw new ArgumentException("La fecha de nacimiento debe ser anterior a la actual", nameof(fechaNacimiento));
            FechaNacimiento = fechaNacimiento.Value.Date;
        }

        if (genero.HasValue)
        {
            char generoNormalizado = char.ToUpper(genero.Value);
            if (generoNormalizado != 'M' && generoNormalizado != 'F' && generoNormalizado != 'O')
                throw new ArgumentException("El género no es válido", nameof(genero));
            Genero = generoNormalizado;
        }
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            if (nombre.Trim().Length > 100) throw new ArgumentException("Nombre excede 100 caracteres", nameof(nombre));
            Nombre = nombre.Trim();
        }

        if (!string.IsNullOrWhiteSpace(apellido))
        {
            if (apellido.Trim().Length > 100) throw new ArgumentException("Apellido excede 100 caracteres", nameof(apellido));
            Apellido = apellido.Trim();
        }

        if (email != null)
        {
            if (email.Length > 100) throw new ArgumentException("Email excede 100 caracteres", nameof(email));
            Email = email;
        }

        if (telefono != null)
        {
            if (telefono.Length > 20) throw new ArgumentException("Teléfono excede 20 caracteres", nameof(telefono));
            Telefono = telefono;
        }

        if (direccion != null)
        {
            if (direccion.Length > 200) throw new ArgumentException("Dirección excede 200 caracteres", nameof(direccion));
            Direccion = direccion;
        }

        if (ciudad != null)
        {
            if (ciudad.Length > 50) throw new ArgumentException("Ciudad excede 50 caracteres", nameof(ciudad));
            Ciudad = ciudad;
        }

        if (provincia != null)
        {
            if (provincia.Length > 50) throw new ArgumentException("Provincia excede 50 caracteres", nameof(provincia));
            Provincia = provincia;
        }

        if (codigoPostal != null)
        {
            if (codigoPostal.Length > 10) throw new ArgumentException("Código postal excede 10 caracteres", nameof(codigoPostal));
            CodigoPostal = codigoPostal;
        }

        ActualizarFechaModificacion();
    }
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}


