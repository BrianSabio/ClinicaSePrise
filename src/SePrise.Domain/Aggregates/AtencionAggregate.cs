using SePrise.Domain.Entities;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Aggregates;
public class AtencionAggregate : Entity
{
    // Longitud máxima permitida para el campo de notas de la atención
    private const int LongitudMaximaNotas = 2000;
public int IdAtencion { get; private set; }
    public int? IdTurno { get; private set; }
    public int IdPaciente { get; private set; }
    public int IdMedico { get; private set; }
    public ModalidadPago ModalidadPago { get; private set; }
    public EstadoAtencion Estado { get; private set; }
    public DateTime FechaHoraAcreditacion { get; private set; }
    public DateTime? FechaHoraInicio { get; private set; }
    public DateTime? FechaHoraFin { get; private set; }
    public string? Notas { get; private set; }
    public virtual TurnoAggregate? Turno { get; private set; }
    public virtual Paciente Paciente { get; private set; } = null!;
    public virtual Medico Medico { get; private set; } = null!;
    private AtencionAggregate() { }

    // ───────────────────────────────────────────
    // FACTORY METHODS
    // ───────────────────────────────────────────
    public static AtencionAggregate CrearDesdeConfirmacion(
        int idTurno,
        int idPaciente,
        int idMedico,
        ModalidadPago modalidadPago)
    {
        if (idTurno <= 0)
            throw new ArgumentException(
                $"IdTurno debe ser mayor a cero. Se recibió: {idTurno}.",
                nameof(idTurno));

        if (idPaciente <= 0)
            throw new ArgumentException(
                $"IdPaciente debe ser mayor a cero. Se recibió: {idPaciente}.",
                nameof(idPaciente));

        if (idMedico <= 0)
            throw new ArgumentException(
                $"IdMedico debe ser mayor a cero. Se recibió: {idMedico}.",
                nameof(idMedico));

        var ahora = DateTime.UtcNow;

        return new AtencionAggregate
        {
            IdTurno = idTurno,
            IdPaciente = idPaciente,
            IdMedico = idMedico,
            ModalidadPago = modalidadPago,
            // Toda atención comienza en estado Acreditada — invariante del dominio
            Estado = EstadoAtencion.Acreditada,
            FechaHoraAcreditacion = ahora,
            // FechaHoraInicio y FechaHoraFin son null hasta las transiciones correspondientes
            FechaHoraInicio = null,
            FechaHoraFin = null,
            Notas = null,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }
    public static AtencionAggregate CrearDemandaEspontanea(
        int idPaciente,
        int idMedico,
        ModalidadPago modalidadPago)
    {
        if (idPaciente <= 0)
            throw new ArgumentException(
                $"IdPaciente debe ser mayor a cero. Se recibió: {idPaciente}.",
                nameof(idPaciente));

        if (idMedico <= 0)
            throw new ArgumentException(
                $"IdMedico debe ser mayor a cero. Se recibió: {idMedico}.",
                nameof(idMedico));

        var ahora = DateTime.UtcNow;

        return new AtencionAggregate
        {
            // IdTurno = null: distingue esta atención como demanda espontánea
            IdTurno = null,
            IdPaciente = idPaciente,
            IdMedico = idMedico,
            ModalidadPago = modalidadPago,
            Estado = EstadoAtencion.Acreditada,
            FechaHoraAcreditacion = ahora,
            FechaHoraInicio = null,
            FechaHoraFin = null,
            Notas = null,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }

    // ───────────────────────────────────────────
    // MÉTODOS DE TRANSICIÓN DE ESTADO
    // ───────────────────────────────────────────
    public void ProgresarAEnProgreso(DateTime fechaInicio)
    {
        if (!Estado.PuedeProgresarAEnProgreso())
            throw new AtencionException(
                $"No se puede iniciar la atención en estado '{Estado}'. " +
                $"Solo las atenciones en estado '{EstadoAtencion.Acreditada}' pueden iniciarse.");
        if (fechaInicio < FechaHoraAcreditacion)
            throw new AtencionException(
                $"La hora de inicio ({fechaInicio:HH:mm:ss} UTC) no puede ser anterior " +
                $"a la hora de acreditación ({FechaHoraAcreditacion:HH:mm:ss} UTC).");

        Estado = EstadoAtencion.EnProgreso;
        FechaHoraInicio = fechaInicio;
        ActualizarFechaModificacion();
    }
    public void Finalizar(DateTime fechaFin, string? notas = null)
    {
        if (!Estado.PuedeFinalizarse())
            throw new AtencionException(
                $"Solo las atenciones en estado '{EstadoAtencion.EnProgreso}' pueden finalizarse. " +
                $"Estado actual: '{Estado}'.");
        if (FechaHoraInicio.HasValue && fechaFin < FechaHoraInicio.Value)
            throw new AtencionException(
                $"La hora de fin ({fechaFin:HH:mm:ss} UTC) no puede ser anterior " +
                $"a la hora de inicio ({FechaHoraInicio.Value:HH:mm:ss} UTC).");

        Estado = EstadoAtencion.Finalizada;
        FechaHoraFin = fechaFin;

        // Actualizar notas solo si se proporcionaron (no sobreescribir con null)
        if (notas is not null)
            Notas = notas;

        ActualizarFechaModificacion();
    }
    public void Cancelar()
    {
        if (!Estado.PuedeCancelarse())
            throw new AtencionException(
                $"No se puede cancelar la atención en estado '{Estado}'. " +
                $"Solo se puede cancelar desde '{EstadoAtencion.Acreditada}' o '{EstadoAtencion.EnProgreso}'.");

        Estado = EstadoAtencion.Cancelada;
        // FechaHoraInicio y FechaHoraFin se preservan intencionalmente como registro histórico parcial
        ActualizarFechaModificacion();
    }
    public void ActualizarNotas(string notas)
    {
        if (Estado.EsTerminal())
            throw new AtencionException(
                $"No se pueden actualizar las notas de una atención en estado terminal '{Estado}'. " +
                $"Solo se permiten actualizaciones en estados no terminales.");
        if (string.IsNullOrWhiteSpace(notas))
            throw new ArgumentException(
                "Las notas no pueden ser nulas ni estar vacías.",
                nameof(notas));
        if (notas.Length > LongitudMaximaNotas)
            throw new AtencionException(
                $"Las notas no pueden exceder {LongitudMaximaNotas} caracteres. " +
                $"Se recibieron {notas.Length} caracteres.");

        Notas = notas;
        ActualizarFechaModificacion();
    }

    // ───────────────────────────────────────────
    // MÉTODOS AUXILIARES DE VALIDACIÓN
    // ───────────────────────────────────────────
    public bool PuedeProgresarAEnProgreso() =>
        Estado.PuedeProgresarAEnProgreso();
    public bool PuedeFinalizarse() =>
        Estado.PuedeFinalizarse();
    public bool PuedeCancelarse() =>
        Estado.PuedeCancelarse();
}


