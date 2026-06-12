using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad <see cref="Especialidad"/>.
/// Define tabla, columnas, restricciones e índices para el catálogo de especialidades médicas.
/// </summary>
public class EspecialidadConfiguration : IEntityTypeConfiguration<Especialidad>
{
    public void Configure(EntityTypeBuilder<Especialidad> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Especialidades");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(e => e.IdEspecialidad);
        builder.Property(e => e.IdEspecialidad)
            .ValueGeneratedOnAdd();

        // ── Propiedades escalares ─────────────────────────────────────────────
        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        // Nombre de especialidad único en el sistema
        builder.HasIndex(e => e.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_Especialidades_Nombre_Unique");

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500)
            .IsRequired(false);

        // Duración de la consulta en minutos (mínimo 15 min, por defecto 30)
        builder.Property(e => e.DuracionMinutos)
            .IsRequired()
            .HasDefaultValue(30);

        // Permite múltiples turnos por día por defecto: false
        builder.Property(e => e.PermiteMultiplesTurnos)
            .IsRequired()
            .HasDefaultValue(false);

        // Soft delete: activa por defecto
        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(e => e.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
