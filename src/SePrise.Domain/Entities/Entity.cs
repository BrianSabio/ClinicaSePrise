namespace SePrise.Domain.Entities;
public abstract class Entity
{
    public DateTime FechaCreacion { get; protected set; } = DateTime.UtcNow;
    public DateTime FechaUltimaModificacion { get; protected set; } = DateTime.UtcNow;
    protected Entity() { }
    protected void ActualizarFechaModificacion()
    {
        FechaUltimaModificacion = DateTime.UtcNow;
    }
}


