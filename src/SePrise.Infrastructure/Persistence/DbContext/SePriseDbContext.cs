using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence;
public class SePriseDbContext : DbContext
{
    public DbSet<Paciente> Pacientes { get; set; } = null!;
    public DbSet<Especialidad> Especialidades { get; set; } = null!;
    public DbSet<Medico> Medicos { get; set; } = null!;
    public DbSet<Sala> Salas { get; set; } = null!;
    public DbSet<MedicoEspecialidad> MedicoEspecialidades { get; set; } = null!;
    public DbSet<TurnoAggregate> Turnos { get; set; } = null!;
    public DbSet<AtencionAggregate> Atenciones { get; set; } = null!;
    public SePriseDbContext(DbContextOptions<SePriseDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Llama a la implementación base para asegurar comportamiento correcto
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones IEntityTypeConfiguration<T> de este assembly
        // Esto incluye: PacienteConfiguration, MedicoConfiguration, TurnoConfiguration, etc.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SePriseDbContext).Assembly);
    }
}


