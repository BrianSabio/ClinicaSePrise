using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad asociativa <see cref="MedicoEspecialidad"/>.
/// Define la clave primaria compuesta y las relaciones N:N entre Medico y Especialidad.
/// </summary>
public class MedicoEspecialidadConfiguration : IEntityTypeConfiguration<MedicoEspecialidad>
{
    public void Configure(EntityTypeBuilder<MedicoEspecialidad> builder)
    {
        // Configuraciones de clave primaria compuesta y relaciones se agregarán en Microtarea 2.2
    }
}
