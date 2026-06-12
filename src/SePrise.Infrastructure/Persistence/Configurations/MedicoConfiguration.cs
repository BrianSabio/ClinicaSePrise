using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad <see cref="Medico"/>.
/// Define tabla, columnas, índice único de matrícula y restricciones.
/// </summary>
public class MedicoConfiguration : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Medicos");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(m => m.IdMedico);
        builder.Property(m => m.IdMedico)
            .ValueGeneratedOnAdd();

        // ── Propiedades escalares ─────────────────────────────────────────────
        builder.Property(m => m.NumeroMatricula)
            .HasMaxLength(50)
            .IsRequired();

        // Matrícula única: no puede haber dos médicos con la misma matrícula
        builder.HasIndex(m => m.NumeroMatricula)
            .IsUnique()
            .HasDatabaseName("IX_Medicos_NumeroMatricula_Unique");

        builder.Property(m => m.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Apellido)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(m => m.Telefono)
            .HasMaxLength(20)
            .IsRequired(false);

        // Soft delete: activo por defecto
        builder.Property(m => m.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Fecha de alta en el sistema (datetime2 para precisión UTC)
        builder.Property(m => m.FechaAlta)
            .HasColumnType("datetime2")
            .IsRequired();

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(m => m.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
