using SePrise.Domain.Entities;
using SePrise.Domain.Exceptions;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Aggregates;

/// <summary>
/// Agregado raíz que encapsula el ciclo de vida completo de una atención médica.
/// Es la puerta de entrada a todos los cambios de estado de la atención — ningún código
/// externo puede modificar el estado directamente, solo a través de los métodos de este agregado.
/// <br/>
/// <b>Dos flujos de creación</b>:
/// <list type="bullet">
///   <item>Con turno previo: <see cref="CrearDesdeConfirmacion"/> — el paciente agendó turno.</item>
///   <item>Sin turno previo: <see cref="CrearDemandaEspontanea"/> — el paciente llegó sin turno.</item>
/// </list>
/// <b>Responsabilidad del Servicio de Aplicación</b>: mantener coherencia con
/// <see cref="TurnoAggregate"/> (marcar turno como Atendido al finalizar, etc.).
/// </summary>
public class AtencionAggregate : Entity
{
    // Longitud máxima permitida para el campo de notas de la atención
    private const int LongitudMaximaNotas = 2000;

    /// <summary>
    /// Identificador único de la atención (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdAtencion { get; private set; }

    /// <summary>
    /// Clave foránea al turno asociado. Es <c>null</c> si la atención corresponde a
    /// demanda espontánea (paciente sin turno previo). Inmutable tras la creación.
    /// </summary>
    public int? IdTurno { get; private set; }

    /// <summary>
    /// Clave foránea al paciente que recibe la atención. Inmutable tras la creación.
    /// </summary>
    public int IdPaciente { get; private set; }

    /// <summary>
    /// Clave foránea al médico que realiza la atención. Inmutable tras la creación.
    /// </summary>
    public int IdMedico { get; private set; }

    /// <summary>
    /// Modalidad de pago declarada por el paciente al acreditarse.
    /// Puede ser <see cref="ModalidadPago.ObraSocial"/> o <see cref="ModalidadPago.Particular"/>.
    /// Inmutable tras la creación.
    /// </summary>
    public ModalidadPago ModalidadPago { get; private set; }

    /// <summary>
    /// Estado actual de la atención en su ciclo de vida.
    /// Solo puede modificarse a través de los métodos de transición del agregado.
    /// Estado inicial: <see cref="EstadoAtencion.Acreditada"/>.
    /// </summary>
    public EstadoAtencion Estado { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) en que el paciente fue acreditado en recepción.
    /// Se establece al momento de creación de la atención y es inmutable.
    /// </summary>
    public DateTime FechaHoraAcreditacion { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) en que el médico comenzó a atender al paciente.
    /// Es <c>null</c> mientras la atención está en estado <see cref="EstadoAtencion.Acreditada"/>.
    /// Se establece al invocar <see cref="ProgresarAEnProgreso"/>.
    /// </summary>
    public DateTime? FechaHoraInicio { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) en que finalizó la atención médica.
    /// Es <c>null</c> hasta que la atención alcanza el estado <see cref="EstadoAtencion.Finalizada"/>.
    /// Se establece al invocar <see cref="Finalizar"/>.
    /// </summary>
    public DateTime? FechaHoraFin { get; private set; }

    /// <summary>
    /// Notas clínicas o administrativas registradas durante la atención.
    /// Puede ser <c>null</c> o vacío. Longitud máxima: 2000 caracteres.
    /// Solo puede actualizarse mientras la atención no esté en estado terminal.
    /// </summary>
    public string? Notas { get; private set; }

    /// <summary>
    /// Propiedad de navegación al turno asociado.
    /// Es <c>null</c> si la atención es por demanda espontánea.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual TurnoAggregate? Turno { get; private set; }

    /// <summary>
    /// Propiedad de navegación al paciente de la atención.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Paciente Paciente { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación al médico que realiza la atención.
    /// Marcada como <c>virtual</c> para habilitar lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Medico Medico { get; private set; } = null!;

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private AtencionAggregate() { }

    // ───────────────────────────────────────────
    // FACTORY METHODS
    // ───────────────────────────────────────────

    /// <summary>
    /// Factory method para crear una atención a partir de la confirmación de un turno agendado.
    /// Se invoca desde el Servicio de Aplicación inmediatamente después de que
    /// <see cref="TurnoAggregate.ConfirmarTurno"/> transite el turno a <see cref="EstadoTurno.Confirmado"/>.
    /// </summary>
    /// <param name="idTurno">Identificador del turno confirmado. Debe ser mayor a cero.</param>
    /// <param name="idPaciente">Identificador del paciente. Debe ser mayor a cero.</param>
    /// <param name="idMedico">Identificador del médico. Debe ser mayor a cero.</param>
    /// <param name="modalidadPago">Modalidad de pago declarada por el paciente al acreditarse.</param>
    /// <returns>
    /// Una instancia válida de <see cref="AtencionAggregate"/> en estado
    /// <see cref="EstadoAtencion.Acreditada"/>, vinculada al turno indicado.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si cualquier identificador es menor o igual a cero.
    /// </exception>
    public static AtencionAggregate CrearDesdeConfirmacion(
        int idTurno,
        int idPaciente,
        int idMedico,
        ModalidadPago modalidadPago)
    {
        // Validar identificadores de entidades relacionadas
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

    /// <summary>
    /// Factory method para crear una atención por demanda espontánea, es decir,
    /// cuando el paciente llega a la clínica sin turno previo agendado.
    /// La atención no tiene turno asociado (<see cref="IdTurno"/> es <c>null</c>).
    /// </summary>
    /// <param name="idPaciente">Identificador del paciente. Debe ser mayor a cero.</param>
    /// <param name="idMedico">Identificador del médico. Debe ser mayor a cero.</param>
    /// <param name="modalidadPago">Modalidad de pago declarada por el paciente al acreditarse.</param>
    /// <returns>
    /// Una instancia válida de <see cref="AtencionAggregate"/> en estado
    /// <see cref="EstadoAtencion.Acreditada"/>, sin turno asociado.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si cualquier identificador es menor o igual a cero.
    /// </exception>
    public static AtencionAggregate CrearDemandaEspontanea(
        int idPaciente,
        int idMedico,
        ModalidadPago modalidadPago)
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

    /// <summary>
    /// Inicia la atención médica, transicionando de <see cref="EstadoAtencion.Acreditada"/> a
    /// <see cref="EstadoAtencion.EnProgreso"/>. Representa el momento en que el médico
    /// comienza a atender al paciente.
    /// <br/>
    /// <b>Precondición</b>: El estado debe ser <see cref="EstadoAtencion.Acreditada"/>.
    /// <br/>
    /// <b>Validación temporal</b>: La fecha de inicio no puede ser anterior al momento
    /// en que el paciente fue acreditado.
    /// </summary>
    /// <param name="fechaInicio">
    /// Fecha y hora (UTC) en que el médico inicia la atención.
    /// Debe ser mayor o igual a <see cref="FechaHoraAcreditacion"/>.
    /// </param>
    /// <exception cref="AtencionException">
    /// Se lanza si el estado no es <see cref="EstadoAtencion.Acreditada"/>,
    /// o si la fecha de inicio es anterior a la acreditación.
    /// </exception>
    public void ProgresarAEnProgreso(DateTime fechaInicio)
    {
        // Validar precondición de estado
        if (!Estado.PuedeProgresarAEnProgreso())
            throw new AtencionException(
                $"No se puede iniciar la atención en estado '{Estado}'. " +
                $"Solo las atenciones en estado '{EstadoAtencion.Acreditada}' pueden iniciarse.");

        // Validar coherencia temporal: inicio no puede ser antes de la acreditación
        if (fechaInicio < FechaHoraAcreditacion)
            throw new AtencionException(
                $"La hora de inicio ({fechaInicio:HH:mm:ss} UTC) no puede ser anterior " +
                $"a la hora de acreditación ({FechaHoraAcreditacion:HH:mm:ss} UTC).");

        Estado = EstadoAtencion.EnProgreso;
        FechaHoraInicio = fechaInicio;
        ActualizarFechaModificacion();
    }

    /// <summary>
    /// Finaliza la atención médica, transicionando de <see cref="EstadoAtencion.EnProgreso"/> a
    /// <see cref="EstadoAtencion.Finalizada"/>. Estado terminal — no admite más transiciones.
    /// <br/>
    /// <b>Precondición</b>: El estado debe ser <see cref="EstadoAtencion.EnProgreso"/>.
    /// <br/>
    /// <b>Validación temporal</b>: La fecha de fin no puede ser anterior al momento de inicio.
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Tras finalizar, el Servicio de Aplicación debe invocar
    /// <see cref="TurnoAggregate.MarcarComoAtendido"/> en el turno asociado (si existe).
    /// </summary>
    /// <param name="fechaFin">
    /// Fecha y hora (UTC) en que finaliza la atención.
    /// Debe ser mayor o igual a <see cref="FechaHoraInicio"/>.
    /// </param>
    /// <param name="notas">
    /// Notas clínicas opcionales del médico. Si se proporcionan, reemplazan las notas previas.
    /// Longitud máxima: 2000 caracteres.
    /// </param>
    /// <exception cref="AtencionException">
    /// Se lanza si el estado no es <see cref="EstadoAtencion.EnProgreso"/>,
    /// o si la fecha de fin es anterior a la fecha de inicio.
    /// </exception>
    public void Finalizar(DateTime fechaFin, string? notas = null)
    {
        // Validar precondición de estado
        if (!Estado.PuedeFinalizarse())
            throw new AtencionException(
                $"Solo las atenciones en estado '{EstadoAtencion.EnProgreso}' pueden finalizarse. " +
                $"Estado actual: '{Estado}'.");

        // Validar coherencia temporal: fin no puede ser antes del inicio
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

    /// <summary>
    /// Cancela la atención médica, transicionando al estado <see cref="EstadoAtencion.Cancelada"/>.
    /// Estado terminal — no admite más transiciones.
    /// Puede cancelarse desde <see cref="EstadoAtencion.Acreditada"/> o <see cref="EstadoAtencion.EnProgreso"/>.
    /// Los timestamps parciales (FechaHoraInicio si existía) se preservan como registro histórico.
    /// <br/>
    /// <b>Responsabilidad del Servicio</b>: Cancelar la atención puede ocurrir en cascada cuando
    /// el <see cref="TurnoAggregate"/> asociado es cancelado o reprogramado.
    /// </summary>
    /// <exception cref="AtencionException">
    /// Se lanza si el estado no permite cancelación
    /// (ya está en estado <see cref="EstadoAtencion.Finalizada"/> o <see cref="EstadoAtencion.Cancelada"/>).
    /// </exception>
    public void Cancelar()
    {
        // Validar precondición: solo se puede cancelar desde estados no terminales
        if (!Estado.PuedeCancelarse())
            throw new AtencionException(
                $"No se puede cancelar la atención en estado '{Estado}'. " +
                $"Solo se puede cancelar desde '{EstadoAtencion.Acreditada}' o '{EstadoAtencion.EnProgreso}'.");

        Estado = EstadoAtencion.Cancelada;
        // FechaHoraInicio y FechaHoraFin se preservan intencionalmente como registro histórico parcial
        ActualizarFechaModificacion();
    }

    /// <summary>
    /// Actualiza las notas clínicas o administrativas de la atención.
    /// Solo puede invocarse mientras la atención no esté en un estado terminal
    /// (<see cref="EstadoAtencion.Finalizada"/> o <see cref="EstadoAtencion.Cancelada"/>).
    /// </summary>
    /// <param name="notas">
    /// Nuevas notas a registrar. No puede ser nulo ni vacío. Longitud máxima: 2000 caracteres.
    /// </param>
    /// <exception cref="AtencionException">
    /// Se lanza si la atención está en estado terminal o si las notas superan el límite.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Se lanza si las notas son nulas o vacías.
    /// </exception>
    public void ActualizarNotas(string notas)
    {
        // Validar que la atención no esté en estado terminal
        if (Estado.EsTerminal())
            throw new AtencionException(
                $"No se pueden actualizar las notas de una atención en estado terminal '{Estado}'. " +
                $"Solo se permiten actualizaciones en estados no terminales.");

        // Validar que el texto de notas no sea nulo ni vacío
        if (string.IsNullOrWhiteSpace(notas))
            throw new ArgumentException(
                "Las notas no pueden ser nulas ni estar vacías.",
                nameof(notas));

        // Validar la longitud máxima permitida para las notas
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

    /// <summary>
    /// Consulta si la atención puede progresar al estado <see cref="EstadoAtencion.EnProgreso"/>
    /// desde su estado actual, sin lanzar una excepción.
    /// </summary>
    /// <returns>
    /// <c>true</c> si el estado actual es <see cref="EstadoAtencion.Acreditada"/>;
    /// <c>false</c> en caso contrario.
    /// </returns>
    public bool PuedeProgresarAEnProgreso() =>
        Estado.PuedeProgresarAEnProgreso();

    /// <summary>
    /// Consulta si la atención puede finalizar desde su estado actual,
    /// sin lanzar una excepción.
    /// </summary>
    /// <returns>
    /// <c>true</c> si el estado actual es <see cref="EstadoAtencion.EnProgreso"/>;
    /// <c>false</c> en caso contrario.
    /// </returns>
    public bool PuedeFinalizarse() =>
        Estado.PuedeFinalizarse();

    /// <summary>
    /// Consulta si la atención puede cancelarse desde su estado actual,
    /// sin lanzar una excepción.
    /// </summary>
    /// <returns>
    /// <c>true</c> si el estado actual es <see cref="EstadoAtencion.Acreditada"/> o
    /// <see cref="EstadoAtencion.EnProgreso"/>; <c>false</c> en caso contrario.
    /// </returns>
    public bool PuedeCancelarse() =>
        Estado.PuedeCancelarse();
}
