namespace SePrise.Domain.Entities;
public class Especialidad : Entity
{
public int IdEspecialidad { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public int DuracionMinutos { get; private set; }
    public bool PermiteMultiplesTurnos { get; private set; }
    public bool Activo { get; private set; }
    private Especialidad() { }
    public static Especialidad Crear(
        string nombre,
        string? descripcion = null,
        int duracionMinutos = 30,
        bool permiteMultiplesTurnos = false)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre de la especialidad no puede ser nulo ni estar vacío.",
                nameof(nombre));
        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre de la especialidad no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));
        if (descripcion is not null && descripcion.Length > 500)
            throw new ArgumentException(
                $"La descripción no puede exceder 500 caracteres. " +
                $"Se recibieron {descripcion.Length} caracteres.",
                nameof(descripcion));
        if (duracionMinutos < 15)
            throw new ArgumentException(
                $"La duración mínima de una consulta es 15 minutos. " +
                $"Se recibieron {duracionMinutos} minutos.",
                nameof(duracionMinutos));

        return new Especialidad
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion,
            DuracionMinutos = duracionMinutos,
            PermiteMultiplesTurnos = permiteMultiplesTurnos,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaUltimaModificacion = DateTime.UtcNow
        };
    }
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}


