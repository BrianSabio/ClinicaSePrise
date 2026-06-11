using SePrise.Domain.Entities;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Aggregates;

/// <summary>
/// Agregado raíz que encapsula el ciclo de vida completo de un turno médico.
/// Es la puerta de entrada a todos los cambios de estado del turno — ningún código
/// externo puede modificar el estado directamente, solo a través de los métodos de este agregado.
/// <br/>
/// <b>Responsabilidad del Servicio de Aplicación</b>: mantener la coherencia con
/// <see cref="AtencionAggregate"/> (crear atención al confirmar, cancelar en cascada, etc.).
/// </summary>
public class TurnoAggregate : Entity
{
    /// <summary>
    /// Identificador único del turno (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdTurno { get; private set; }

    /// <summary>
    /// Clave foránea al paciente al que pertenece el turno. Inmutable tras la creación.
    /// </summary>
    public int IdPaciente { get; private set; }

    /// <summary>
    /// Clave foránea al médico que atenderá el turno. Inmutable tras la creación.
    /// </summary>
    public int IdMedico { get; private set; }

    /// <summary>
    /// Clave foránea a la especialidad del turno. Inmutable tras la creación.
    /// </summary>
    public int IdEspecialidad { get; private set; }

    /// <summary>
    /// Clave foránea a la sala o consultorio asignado al turno. Inmutable tras la creación.
    /// </summary>
    public int IdSala { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) de inicio del turno según el agendamiento.
    /// Inmutable tras la creación — la reprogramación genera un nuevo turno.
    /// </summary>
    public DateTime FechaHoraInicio { get; private set; }

    /// <summary>
    /// Duración estándar del turno en minutos, heredada de la Especialidad al momento del agendamiento.
    /// Inmutable tras la creación.
    /// </summary>
    public int DuracionMinutos { get; private set; }

    /// <summary>
    /// Estado actual del turno en su ciclo de vida.
    /// Solo puede ser modificado a través de los métodos de transición del agregado.
    /// Estado inicial: <see cref="EstadoTurno.Reservado"/>.
    /// </summary>
    public EstadoTurno Estado { get; private set; }

    /// <summary>
    /// Propiedad de navegación al paciente del turno.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Paciente Paciente { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación al médico del turno.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Medico Medico { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación a la especialidad del turno.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Especialidad Especialidad { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación a la sala del turno.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Sala Sala { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación a la atención médica asociada (relación 0..1).
    /// Puede ser <c>null</c> si el turno aún no fue confirmado o si la atención corresponde
    /// a demanda espontánea (en ese caso <see cref="AtencionAggregate.IdTurno"/> es null).
    /// </summary>
    public virtual AtencionAggregate? Atencion { get; private set; }

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private TurnoAggregate() { }

    /// <summary>
    /// Factory method para crear un nuevo turno en estado <see cref="EstadoTurno.Reservado"/>.
    /// Valida todas las precondiciones de dominio antes de construir la instancia.
    /// </summary>
    /// <param name="idPaciente">Identificador del paciente. Debe ser mayor a cero.</param>
    /// <param name="idMedico">Identificador del médico. Debe ser mayor a cero.</param>
    /// <param name="idEspecialidad">Identificador de la especialidad. Debe ser mayor a cero.</param>
    /// <param name="idSala">Identificador de la sala. Debe ser mayor a cero.</param>
    /// <param name="fechaHoraInicio">
    /// Fecha y hora de inicio del turno (UTC). Debe ser una fecha futura (mayor a DateTime.UtcNow).
    /// </param>
    /// <param name="duracionMinutos">Duración en minutos. Debe ser mayor a cero.</param>
    /// <returns>Una instancia válida de <see cref="TurnoAggregate"/> en estado Reservado.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si cualquier identificador es menor o igual a cero,
    /// si la fecha de inicio no es futura, o si la duración es menor o igual a cero.
    /// </exception>
    public static TurnoAggregate Crear(
        int idPaciente,
        int idMedico,
        int idEspecialidad,
        int idSala,
        DateTime fechaHoraInicio,
        int duracionMinutos)
    {
        // Validar identificadores de entidades relacionadas
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

        // Validar que el turno sea en el futuro
        if (fechaHoraInicio <= DateTime.UtcNow)
            throw new ArgumentException(
                $"La fecha y hora de inicio del turno debe ser futura. " +
                $"Se recibió: {fechaHoraInicio:yyyy-MM-dd HH:mm:ss} UTC.",
                nameof(fechaHoraInicio));

        // Validar duración mínima
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

    /// <summary>
    /// Confirma el turno, transicionando de <see cref="EstadoTurno.Reservado"/> a
    /// <see cref="EstadoTurno.Confirmado"/>. Esto ocurre cuando el paciente se acredita
    /// en recepción.
    /// <br/>
    /// <b>Precondición</b>: El turno debe estar en estado <see cref="EstadoTurno.Reservado"/>.
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Tras confirmar, el Servicio de Aplicación debe
    /// crear la <see cref="AtencionAggregate"/> asociada mediante
    /// <see cref="AtencionAggregate.CrearDesdeConfirmacion"/>.
    /// </summary>
    /// <exception cref="TurnoException">
    /// Se lanza si el turno no está en estado <see cref="EstadoTurno.Reservado"/>.
    /// </exception>
    public void ConfirmarTurno()
    {
        // Validar precondición: solo turnos Reservados pueden confirmarse
        if (Estado != EstadoTurno.Reservado)
            throw new TurnoException(
                $"No se puede confirmar un turno en estado '{Estado}'. " +
                $"Solo los turnos en estado '{EstadoTurno.Reservado}' pueden confirmarse.");

        Estado = EstadoTurno.Confirmado;
        ActualizarFechaModificacion();
    }

    /// <summary>
    /// Marca el turno como atendido, transicionando de <see cref="EstadoTurno.Confirmado"/> a
    /// <see cref="EstadoTurno.Atendido"/>. Estado terminal — no admite más transiciones.
    /// <br/>
    /// <b>Precondición</b>: El turno debe estar en estado <see cref="EstadoTurno.Confirmado"/>.
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Este método se invoca en cascada cuando la
    /// <see cref="AtencionAggregate"/> asociada es finalizada.
    /// </summary>
    /// <exception cref="TurnoException">
    /// Se lanza si el turno no está en estado <see cref="EstadoTurno.Confirmado"/>.
    /// </exception>
    public void MarcarComoAtendido()
    {
        // Validar precondición: solo turnos Confirmados pueden ser Atendidos
        if (Estado != EstadoTurno.Confirmado)
            throw new TurnoException(
                $"Solo turnos en estado '{EstadoTurno.Confirmado}' pueden marcarse como Atendidos. " +
                $"Estado actual: '{Estado}'.");

        Estado = EstadoTurno.Atendido;
        ActualizarFechaModificacion();
    }

    /// <summary>
    /// Cancela el turno, transicionando al estado <see cref="EstadoTurno.Cancelado"/>. Estado terminal.
    /// Puede cancelarse desde <see cref="EstadoTurno.Reservado"/> o <see cref="EstadoTurno.Confirmado"/>.
    /// <br/>
    /// <b>Precondición</b>: El turno no debe tener una atención en estado
    /// <see cref="EstadoAtencion.EnProgreso"/> (médico ya atendiendo).
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Si existe una <see cref="AtencionAggregate"/>
    /// en estado <see cref="EstadoAtencion.Acreditada"/>, el servicio debe cancelarla en cascada.
    /// </summary>
    /// <exception cref="TurnoException">
    /// Se lanza si el turno está en un estado que no permite cancelación,
    /// o si existe una atención en progreso.
    /// </exception>
    public void CancelarTurno()
    {
        // Validar precondición: el estado actual debe permitir cancelación
        if (!Estado.PuedeCancelarse())
            throw new TurnoException(
                $"No se puede cancelar un turno en estado '{Estado}'. " +
                $"Solo se puede cancelar desde '{EstadoTurno.Reservado}' o '{EstadoTurno.Confirmado}'.");

        // Validar que no exista una atención ya iniciada por el médico
        if (Atencion is not null && Atencion.Estado == EstadoAtencion.EnProgreso)
            throw new TurnoException(
                $"No se puede cancelar el turno porque la atención asociada " +
                $"ya está en progreso (estado: '{Atencion.Estado}'). " +
                $"Primero debe cancelarse la atención.");

        Estado = EstadoTurno.Cancelado;
        ActualizarFechaModificacion();
    }

    /// <summary>
    /// Marca el turno como no asistencia, transicionando al estado <see cref="EstadoTurno.NoAsistio"/>.
    /// Estado terminal. Solo es válido cuando el paciente nunca llegó (no hubo acreditación).
    /// <br/>
    /// <b>Precondición</b>: El turno debe estar en estado <see cref="EstadoTurno.Reservado"/>
    /// (el paciente nunca acreditó en recepción).
    /// </summary>
    /// <exception cref="TurnoException">
    /// Se lanza si el turno no está en estado <see cref="EstadoTurno.Reservado"/>.
    /// </exception>
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

    /// <summary>
    /// Reprograma el turno, transicionando al estado <see cref="EstadoTurno.Reprogramado"/>. Estado terminal.
    /// La reprogramación cierra el turno actual y genera uno nuevo (responsabilidad del Servicio).
    /// <br/>
    /// <b>Precondición</b>: El turno no debe estar en estados terminales ni tener una atención
    /// en progreso o finalizada.
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Crear un nuevo <see cref="TurnoAggregate"/> en estado
    /// Reservado con la nueva fecha, y cancelar la <see cref="AtencionAggregate"/> en Acreditada si existe.
    /// </summary>
    /// <exception cref="TurnoException">
    /// Se lanza si el turno está en estado terminal incompatible con reprogramación,
    /// o si la atención asociada está en progreso o finalizada.
    /// </exception>
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

    /// <summary>
    /// Consulta si el turno puede transicionar al estado indicado desde su estado actual,
    /// sin lanzar una excepción. Útil para validaciones previas en el Servicio de Aplicación.
    /// </summary>
    /// <param name="nuevoEstado">El estado destino a evaluar.</param>
    /// <returns>
    /// <c>true</c> si la transición es válida según la máquina de estados del dominio;
    /// <c>false</c> si la transición no está permitida.
    /// </returns>
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
