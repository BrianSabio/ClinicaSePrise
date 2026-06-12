namespace SePrise.Domain.Entities;
public enum TipoSala
{
    Consultorio = 1,
    Procedimientos = 2,
    Espera = 3
}
public class Sala : Entity
{
public int IdSala { get; private set; }
    public string Numero { get; private set; } = string.Empty;
    public TipoSala TipoSala { get; private set; }
    public bool Activo { get; private set; }
    private Sala() { }
    public static Sala Crear(string numero, TipoSala tipoSala)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException(
                "El número de sala no puede ser nulo ni estar vacío.",
                nameof(numero));
        if (numero.Trim().Length > 50)
            throw new ArgumentException(
                $"El número de sala no puede exceder 50 caracteres. " +
                $"Se recibieron {numero.Trim().Length} caracteres.",
                nameof(numero));
        if (!Enum.IsDefined(typeof(TipoSala), tipoSala))
            throw new ArgumentException(
                $"El tipo de sala '{tipoSala}' no es un valor válido.",
                nameof(tipoSala));

        return new Sala
        {
            Numero = numero.Trim(),
            TipoSala = tipoSala,
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


