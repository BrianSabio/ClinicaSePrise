using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad <see cref="Sala"/>.
/// Define tabla, columnas, índice único de número de sala y conversión del enum <see cref="TipoSala"/>.
/// </summary>
public class SalaConfiguration : IEntityTypeConfiguration<Sala>
{
    public void Configure(EntityTypeBuilder<Sala> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Salas");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(s => s.IdSala);
        builder.Property(s => s.IdSala)
            .ValueGeneratedOnAdd();

        // ── Propiedades escalares ─────────────────────────────────────────────
        builder.Property(s => s.Numero)
            .HasMaxLength(50)
            .IsRequired();

        // Número de sala único en la clínica
        builder.HasIndex(s => s.Numero)
            .IsUnique()
            .HasDatabaseName("IX_Salas_Numero_Unique");

        // TipoSala: enum almacenado como int en la base de datos
        // 1=Consultorio, 2=Procedimientos, 3=Espera
        builder.Property(s => s.TipoSala)
            .HasConversion<int>()
            .IsRequired();

        // Soft delete: activa por defecto
        builder.Property(s => s.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(s => s.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
