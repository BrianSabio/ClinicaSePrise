using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Especialidad.
/// </summary>
public class EspecialidadConfiguration : IEntityTypeConfiguration<Especialidad>
{
    public void Configure(EntityTypeBuilder<Especialidad> builder)
    {
        // Configuraciones de tabla y columnas se agregarán en Microtarea 2.2
    }
}
