using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;
public class MedicoEspecialidadConfiguration : IEntityTypeConfiguration<MedicoEspecialidad>
{
    public void Configure(EntityTypeBuilder<MedicoEspecialidad> builder)
    {
        // ── Tabla ─────────────────────────────────────────────────────────────
        builder.ToTable("MedicoEspecialidades");

        // ── Clave primaria compuesta (IdMedico, IdEspecialidad) ───────────────
        // La combinación médico + especialidad debe ser única
        builder.HasKey(me => new { me.IdMedico, me.IdEspecialidad });

        // ── Relaciones ────────────────────────────────────────────────────────

        // Un médico puede tener muchas especialidades asignadas
        // Si se elimina el médico, se eliminan en cascada sus especializaciones
        builder.HasOne(me => me.Medico)
            .WithMany()
            .HasForeignKey(me => me.IdMedico)
            .OnDelete(DeleteBehavior.Cascade);

        // Una especialidad puede tener muchos médicos asociados
        // Si se elimina la especialidad, se eliminan en cascada sus asociaciones
        builder.HasOne(me => me.Especialidad)
            .WithMany()
            .HasForeignKey(me => me.IdEspecialidad)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Timestamps de auditoría ───────────────────────────────────────────
        builder.Property(me => me.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(me => me.FechaUltimaModificacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}


