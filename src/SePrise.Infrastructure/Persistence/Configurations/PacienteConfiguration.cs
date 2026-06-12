using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad <see cref="Paciente"/>.
/// Define tabla, columnas, restricciones y la conversión del Value Object <see cref="Dni"/>
/// hacia y desde su representación primitiva (string) en la base de datos.
/// </summary>
public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("Pacientes");

        // ── Clave primaria (auto-increment) ───────────────────────────────────
        builder.HasKey(p => p.IdPaciente);
        builder.Property(p => p.IdPaciente)
            .ValueGeneratedOnAdd();

        // ── Value Object DNI con conversión explícita ─────────────────────────
        // HasConversion convierte Dni ↔ string al guardar/leer de la BD
        // No se usa OwnsOne porque Dni se almacena como una sola columna escalar
        builder.Property(p => p.DNI)
            .HasConversion(
                dniValue => dniValue.Valor,      // Guardar: Dni → string (columna BD)
                storedValue => Dni.Crear(storedValue)) // Leer: string → Dni (objeto dominio)
            .HasMaxLength(9)
            .IsRequired()
            .HasColumnName("DNI");

        // Índice único: no puede haber dos pacientes con el mismo DNI
        builder.HasIndex(p => p.DNI)
            .IsUnique()
            .HasDatabaseName("IX_Pacientes_DNI_Unique");

        // ── Propiedades escalares ─────────────────────────────────────────────
        builder.Property(p => p.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Apellido)
            .HasMaxLength(100)
            .IsRequired();

        // Solo fecha, sin componente horario (tipo SQL Server: date)
        builder.Property(p => p.FechaNacimiento)
            .HasColumnType("date")
            .IsRequired();

        // Género: char almacenado como nvarchar(1)
        builder.Property(p => p.Genero)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.Telefono)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(p => p.Direccion)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(p => p.Ciudad)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.Provincia)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(p => p.CodigoPostal)
            .HasMaxLength(10)
            .IsRequired(false);

        // Soft delete: activo por defecto
        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        // CURRENT_TIMESTAMP asegura que la BD registre el timestamp en UTC
        builder.Property(p => p.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
