using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Aggregates;

namespace SePrise.Infrastructure.Persistence.Configurations;
public class TurnoConfiguration : IEntityTypeConfiguration<TurnoAggregate>
{
    public void Configure(EntityTypeBuilder<TurnoAggregate> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Turnos");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(t => t.IdTurno);
        builder.Property(t => t.IdTurno)
            .ValueGeneratedOnAdd();

        // ── Claves foráneas (FKs a entidades de catálogo) ─────────────────────
        builder.Property(t => t.IdPaciente).IsRequired();
        builder.Property(t => t.IdMedico).IsRequired();
        builder.Property(t => t.IdEspecialidad).IsRequired();
        builder.Property(t => t.IdSala).IsRequired();

        // ── Propiedades de datos ──────────────────────────────────────────────
        // datetime2 para máxima precisión temporal (microsegundos) en UTC
        builder.Property(t => t.FechaHoraInicio)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(t => t.DuracionMinutos)
            .IsRequired();

        // Estado del turno: enum almacenado como int en la BD
        // 1=Reservado, 2=Confirmado, 3=Atendido, 4=NoAsistio, 5=Cancelado, 6=Reprogramado
        builder.Property(t => t.Estado)
            .HasConversion<int>()
            .IsRequired();

        // ── Relaciones (FKs con integridad referencial) ───────────────────────

        // Restricción: no se puede eliminar un Paciente si tiene turnos
        builder.HasOne(t => t.Paciente)
            .WithMany()
            .HasForeignKey(t => t.IdPaciente)
            .OnDelete(DeleteBehavior.Restrict);

        // Restricción: no se puede eliminar un Médico si tiene turnos
        builder.HasOne(t => t.Medico)
            .WithMany()
            .HasForeignKey(t => t.IdMedico)
            .OnDelete(DeleteBehavior.Restrict);

        // Restricción: no se puede eliminar una Especialidad si tiene turnos
        builder.HasOne(t => t.Especialidad)
            .WithMany()
            .HasForeignKey(t => t.IdEspecialidad)
            .OnDelete(DeleteBehavior.Restrict);

        // Restricción: no se puede eliminar una Sala si tiene turnos
        builder.HasOne(t => t.Sala)
            .WithMany()
            .HasForeignKey(t => t.IdSala)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1:0..1 Turno → Atención
        // TurnoAggregate es el lado PRINCIPAL (no tiene FK en su tabla)
        // AtencionAggregate.IdTurno es la FK (lado DEPENDIENTE)
        // Si se elimina el turno, se elimina en cascada la atención asociada
        builder.HasOne(t => t.Atencion)
            .WithOne()
            .HasForeignKey<AtencionAggregate>(a => a.IdTurno)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(t => t.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        // ── Índices para queries frecuentes ───────────────────────────────────
        // Búsqueda de turnos de un paciente en una fecha
        builder.HasIndex(t => new { t.IdPaciente, t.FechaHoraInicio })
            .HasDatabaseName("IX_Turnos_Paciente_Fecha");

        // Búsqueda de turnos de un médico en una fecha (agenda médica)
        builder.HasIndex(t => new { t.IdMedico, t.FechaHoraInicio })
            .HasDatabaseName("IX_Turnos_Medico_Fecha");

        // Filtrado por estado (ej: todos los turnos Reservados del día)
        builder.HasIndex(t => t.Estado)
            .HasDatabaseName("IX_Turnos_Estado");
    }
}


