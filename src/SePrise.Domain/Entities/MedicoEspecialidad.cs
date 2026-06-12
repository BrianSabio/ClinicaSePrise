namespace SePrise.Domain.Entities;
public class MedicoEspecialidad : Entity
{
    public int IdMedico { get; private set; }
    public int IdEspecialidad { get; private set; }
    public virtual Medico Medico { get; private set; } = null!;
    public virtual Especialidad Especialidad { get; private set; } = null!;
    private MedicoEspecialidad() { }
    public static MedicoEspecialidad Crear(int idMedico, int idEspecialidad)
    {
        if (idMedico <= 0)
            throw new ArgumentException(
                $"El IdMedico debe ser un valor positivo mayor a cero. " +
                $"Se recibió: {idMedico}.",
                nameof(idMedico));
        if (idEspecialidad <= 0)
            throw new ArgumentException(
                $"El IdEspecialidad debe ser un valor positivo mayor a cero. " +
                $"Se recibió: {idEspecialidad}.",
                nameof(idEspecialidad));

        var ahora = DateTime.UtcNow;

        return new MedicoEspecialidad
        {
            IdMedico = idMedico,
            IdEspecialidad = idEspecialidad,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }
    public override bool Equals(object? obj)
    {
        // Si el objeto es null o no es del tipo MedicoEspecialidad, no son iguales
        if (obj is not MedicoEspecialidad otra)
            return false;

        // La igualdad se determina por la clave de negocio compuesta
        return IdMedico == otra.IdMedico && IdEspecialidad == otra.IdEspecialidad;
    }
    public override int GetHashCode() =>
        HashCode.Combine(IdMedico, IdEspecialidad);
    public static bool operator ==(MedicoEspecialidad? izquierda, MedicoEspecialidad? derecha) =>
        izquierda?.Equals(derecha) ?? derecha is null;
    public static bool operator !=(MedicoEspecialidad? izquierda, MedicoEspecialidad? derecha) =>
        !(izquierda == derecha);
}


