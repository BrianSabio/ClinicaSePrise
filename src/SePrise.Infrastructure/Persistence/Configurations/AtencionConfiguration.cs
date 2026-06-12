using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Aggregates;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para el agregado <see cref="AtencionAggregate"/>.
/// Define tabla, columnas, relaciones y conversión de los enums
/// <see cref="Domain.ValueObjects.ModalidadPago"/> y <see cref="Domain.ValueObjects.EstadoAtencion"/>.
/// <br/>
/// <b>Nota de diseño</b>: La relación 1:0..1 con <see cref="TurnoAggregate"/> se configura en
/// <see cref="TurnoConfiguration"/> para evitar duplicación. Aquí solo se configura
/// <c>IdTurno</c> como nullable (demanda espontánea).
/// </summary>
public class AtencionConfiguration : IEntityTypeConfiguration<AtencionAggregate>
{
    public void Configure(EntityTypeBuilder<AtencionAggregate> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Atenciones");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(a => a.IdAtencion);
        builder.Property(a => a.IdAtencion)
            .ValueGeneratedOnAdd();

        // ── Clave foránea a Turno (nullable) ──────────────────────────────────
        // null = demanda espontánea (paciente sin turno previo)
        // La relación 1:0..1 con Turno se declara en TurnoConfiguration
        builder.Property(a => a.IdTurno)
            .IsRequired(false);

        // ── Claves foráneas obligatorias ──────────────────────────────────────
        builder.Property(a => a.IdPaciente).IsRequired();
        builder.Property(a => a.IdMedico).IsRequired();

        // ── Enums como int ────────────────────────────────────────────────────
        // ModalidadPago: 1=ObraSocial, 2=Particular
        builder.Property(a => a.ModalidadPago)
            .HasConversion<int>()
            .IsRequired();

        // EstadoAtencion: 1=Acreditada, 2=EnProgreso, 3=Finalizada, 4=Cancelada
        builder.Property(a => a.Estado)
            .HasConversion<int>()
            .IsRequired();

        // ── Timestamps propios de la atención ─────────────────────────────────
        // FechaHoraAcreditacion: cuándo el paciente fue registrado en recepción
        builder.Property(a => a.FechaHoraAcreditacion)
            .HasColumnType("datetime2")
            .IsRequired();

        // FechaHoraInicio: cuándo el médico comenzó la atención (null si aún no inició)
        builder.Property(a => a.FechaHoraInicio)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // FechaHoraFin: cuándo terminó la atención (null hasta que esté Finalizada)
        builder.Property(a => a.FechaHoraFin)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Notas clínicas: opcionales, máximo 2000 caracteres
        builder.Property(a => a.Notas)
            .HasMaxLength(2000)
            .IsRequired(false);

        // ── Relaciones ────────────────────────────────────────────────────────
        // Restricción: no se puede eliminar un Paciente si tiene atenciones
        builder.HasOne(a => a.Paciente)
            .WithMany()
            .HasForeignKey(a => a.IdPaciente)
            .OnDelete(DeleteBehavior.Restrict);

        // Restricción: no se puede eliminar un Médico si tiene atenciones
        builder.HasOne(a => a.Medico)
            .WithMany()
            .HasForeignKey(a => a.IdMedico)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(a => a.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        // ── Índices para queries frecuentes ───────────────────────────────────
        // Historial de atenciones de un paciente
        builder.HasIndex(a => new { a.IdPaciente, a.FechaHoraAcreditacion })
            .HasDatabaseName("IX_Atenciones_Paciente_Fecha");

        // Historial de atenciones de un médico
        builder.HasIndex(a => new { a.IdMedico, a.FechaHoraAcreditacion })
            .HasDatabaseName("IX_Atenciones_Medico_Fecha");

        // Filtrado por estado (ej: todas las atenciones EnProgreso)
        builder.HasIndex(a => a.Estado)
            .HasDatabaseName("IX_Atenciones_Estado");
    }
}
