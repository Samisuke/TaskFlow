using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

// Configuración de Entity Framework Core para la entidad Historial.
// Define las restricciones y relaciones necesarias para registrar los cambios de los proyectos.

namespace TaskFlow.Infrastructure.Data.Configurations;

public class HistorialConfiguration : IEntityTypeConfiguration<Historial>
{
    public void Configure(EntityTypeBuilder<Historial> builder)
    {
        builder.ToTable("Historiales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Accion)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.Property(x => x.ProyectoId)
            .IsRequired();
        builder.HasOne(x => x.Proyecto)
            .WithMany(x => x.Historiales)
            .HasForeignKey(x => x.ProyectoId);

        builder.Property(x => x.UsuarioId)
            .IsRequired();
        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId);
    }
}