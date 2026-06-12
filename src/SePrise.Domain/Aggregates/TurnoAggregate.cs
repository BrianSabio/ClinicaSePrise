using SePrise.Domain.Entities;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Aggregates;
public class TurnoAggregate : Entity
{
public int IdTurno { get; private set; }
    public int IdPaciente { get; private set; }
    public int IdMedico { get; private set; }
    public int IdEspecialidad { get; private set; }
    public int IdSala { get; private set; }
    public DateTime FechaHoraInicio { get; private set; }
    public int DuracionMinutos { get; private set; }
    public EstadoTurno Estado { get; private set; }
    public virtual Paciente Paciente { get; private set; } = null!;
    public virtual Medico Medico { get; private set; } = null!;
    public virtual Especialidad Especialidad { get; private set; } = null!;
    public virtual Sala Sala { get; private set; } = null!;
    public virtual AtencionAggregate? Atencion { get; private set; }
    private TurnoAggregate() { }
    public static TurnoAggregate Crear(
        int idPaciente,
        int idMedico,
        int idEspecialidad,
        int idSala,
        DateTime fechaHoraInicio,
        int duracionMinutos)
    {
        if (idPaciente <= 0)
            throw new ArgumentException(
                $"IdPaciente debe ser mayor a cero. Se recibió: {idPaciente}.",
                nameof(idPaciente));

        if (idMedico <= 0)
            throw new ArgumentException(
                $"IdMedico debe ser mayor a cero. Se recibió: {idMedico}.",
                nameof(idMedico));

        if (idEspecialidad <= 0)
            throw new ArgumentException(
                $"IdEspecialidad debe ser mayor a cero. Se recibió: {idEspecialidad}.",
                nameof(idEspecialidad));

        if (idSala <= 0)
            throw new ArgumentException(
                $"IdSala debe ser mayor a cero. Se recibió: {idSala}.",
                nameof(idSala));
        if (fechaHoraInicio <= DateTime.UtcNow)
            throw new ArgumentException(
                $"La fecha y hora de inicio del turno debe ser futura. " +
                $"Se recibió: {fechaHoraInicio:yyyy-MM-dd HH:mm:ss} UTC.",
                nameof(fechaHoraInicio));
        if (duracionMinutos <= 0)
            throw new ArgumentException(
                $"La duración del turno debe ser mayor a cero. Se recibió: {duracionMinutos} minutos.",
                nameof(duracionMinutos));

        var ahora = DateTime.UtcNow;

        return new TurnoAggregate
        {
            IdPaciente = idPaciente,
            IdMedico = idMedico,
            IdEspecialidad = idEspecialidad,
            IdSala = idSala,
            FechaHoraInicio = fechaHoraInicio,
            DuracionMinutos = duracionMinutos,
            // Todo turno comienza en estado Reservado — invariante del dominio
            Estado = EstadoTurno.Reservado,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }

    // ───────────────────────────────────────────
    // MÉTODOS DE TRANSICIÓN DE ESTADO
    // ───────────────────────────────────────────
    public void ConfirmarTurno()
    {
        if (Estado != EstadoTurno.Reservado)
            throw new TurnoException(
                $"No se puede confirmar un turno en estado '{Estado}'. " +
                $"Solo los turnos en estado '{EstadoTurno.Reservado}' pueden confirmarse.");

        Estado = EstadoTurno.Confirmado;
        ActualizarFechaModificacion();
    }
    public void MarcarComoAtendido()
    {
        if (Estado != EstadoTurno.Confirmado)
            throw new TurnoException(
                $"Solo turnos en estado '{EstadoTurno.Confirmado}' pueden marcarse como Atendidos. " +
                $"Estado actual: '{Estado}'.");

        Estado = EstadoTurno.Atendido;
        ActualizarFechaModificacion();
    }
    public void CancelarTurno()
    {
        if (!Estado.PuedeCancelarse())
            throw new TurnoException(
                $"No se puede cancelar un turno en estado '{Estado}'. " +
                $"Solo se puede cancelar desde '{EstadoTurno.Reservado}' o '{EstadoTurno.Confirmado}'.");
        if (Atencion is not null && Atencion.Estado == EstadoAtencion.EnProgreso)
            throw new TurnoException(
                $"No se puede cancelar el turno porque la atención asociada " +
                $"ya está en progreso (estado: '{Atencion.Estado}'). " +
                $"Primero debe cancelarse la atención.");

        Estado = EstadoTurno.Cancelado;
        ActualizarFechaModificacion();
    }
    public void MarcarNoAsistio()
    {
        // Solo los turnos Reservados pueden marcarse como NoAsistio
        // Si ya fue Confirmado, significa que el paciente acreditó — no aplica NoAsistio
        if (Estado != EstadoTurno.Reservado)
            throw new TurnoException(
                $"Solo turnos en estado '{EstadoTurno.Reservado}' pueden marcarse como '{EstadoTurno.NoAsistio}'. " +
                $"Estado actual: '{Estado}'.");

        Estado = EstadoTurno.NoAsistio;
        ActualizarFechaModificacion();
    }
    public void ReprogramarTurno()
    {
        // No se puede reprogramar desde estados terminales definitivos
        if (Estado is EstadoTurno.Atendido or EstadoTurno.NoAsistio or EstadoTurno.Cancelado)
            throw new TurnoException(
                $"No se puede reprogramar un turno en estado '{Estado}'. " +
                $"Los estados terminales no permiten reprogramación.");

        // Si ya fue reprogramado anteriormente, tampoco se puede reprogramar
        if (Estado == EstadoTurno.Reprogramado)
            throw new TurnoException(
                $"El turno ya fue reprogramado previamente. Estado actual: '{Estado}'.");

        // Verificar que la atención (si existe) no esté avanzada
        if (Atencion is not null &&
            Atencion.Estado is EstadoAtencion.EnProgreso or EstadoAtencion.Finalizada)
            throw new TurnoException(
                $"No se puede reprogramar el turno porque la atención asociada " +
                $"se encuentra en estado '{Atencion.Estado}'. " +
                $"Solo se puede reprogramar si la atención está en '{EstadoAtencion.Acreditada}'.");

        Estado = EstadoTurno.Reprogramado;
        ActualizarFechaModificacion();
    }

    // ───────────────────────────────────────────
    // MÉTODOS AUXILIARES DE VALIDACIÓN
    // ───────────────────────────────────────────
    public bool PuedeTransicionar(EstadoTurno nuevoEstado) =>
        Estado switch
        {
            // Desde Reservado se puede ir a: Confirmado, NoAsistio o Cancelado
            EstadoTurno.Reservado =>
                nuevoEstado is EstadoTurno.Confirmado
                            or EstadoTurno.NoAsistio
                            or EstadoTurno.Cancelado,

            // Desde Confirmado se puede ir a: Atendido, Cancelado o Reprogramado
            EstadoTurno.Confirmado =>
                nuevoEstado is EstadoTurno.Atendido
                            or EstadoTurno.Cancelado
                            or EstadoTurno.Reprogramado,

            // Los estados terminales no permiten ninguna transición
            _ => false
        };
}


