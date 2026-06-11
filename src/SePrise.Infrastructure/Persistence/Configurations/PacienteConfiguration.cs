using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SePrise.Domain.Entities;

namespace SePrise.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Paciente.
/// </summary>
public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        // Configuraciones de tabla y columnas se agregarán en Microtarea 2.2
    }
}
