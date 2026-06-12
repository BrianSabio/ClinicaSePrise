using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos principal del sistema SePrise.
/// Gestiona todas las entidades y agregados del dominio mediante Entity Framework Core.
/// <br/>
/// Usa <see cref="Microsoft.EntityFrameworkCore.DbContext"/> como base y aplica
/// automáticamente todas las configuraciones fluentes registradas en este assembly.
/// </summary>
public class SePriseDbContext : DbContext
{
    /// <summary>
    /// Colección de pacientes registrados en el sistema.
    /// </summary>
    public DbSet<Paciente> Pacientes { get; set; } = null!;

    /// <summary>
    /// Colección de especialidades médicas disponibles en la clínica.
    /// </summary>
    public DbSet<Especialidad> Especialidades { get; set; } = null!;

    /// <summary>
    /// Colección de médicos registrados en el sistema.
    /// </summary>
    public DbSet<Medico> Medicos { get; set; } = null!;

    /// <summary>
    /// Colección de salas y consultorios de la clínica.
    /// </summary>
    public DbSet<Sala> Salas { get; set; } = null!;

    /// <summary>
    /// Tabla asociativa de la relación N:N entre médicos y especialidades.
    /// </summary>
    public DbSet<MedicoEspecialidad> MedicoEspecialidades { get; set; } = null!;

    /// <summary>
    /// Colección de turnos agendados en el sistema (agregado raíz del ciclo de vida del turno).
    /// </summary>
    public DbSet<TurnoAggregate> Turnos { get; set; } = null!;

    /// <summary>
    /// Colección de atenciones médicas realizadas (agregado raíz del ciclo de vida de la atención).
    /// </summary>
    public DbSet<AtencionAggregate> Atenciones { get; set; } = null!;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SePriseDbContext"/> con las opciones
    /// de configuración proporcionadas por el contenedor de dependencias.
    /// </summary>
    /// <param name="options">
    /// Opciones de configuración del contexto (cadena de conexión, proveedor, etc.).
    /// </param>
    public SePriseDbContext(DbContextOptions<SePriseDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configura el modelo de datos para Entity Framework Core.
    /// Aplica automáticamente todas las clases que implementan
    /// <see cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}"/>
    /// registradas en este assembly (Infrastructure).
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de datos de EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Llama a la implementación base para asegurar comportamiento correcto
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones IEntityTypeConfiguration<T> de este assembly
        // Esto incluye: PacienteConfiguration, MedicoConfiguration, TurnoConfiguration, etc.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SePriseDbContext).Assembly);
    }
}
