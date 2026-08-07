using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

// Configuración de Entity Framework Core para la relación entre Proyecto y Usuario.
// Define las claves, restricciones y relaciones de la tabla intermedia.

namespace TaskFlow.Infrastructure.Data.Configurations;

public class ProyectoUsuarioConfiguration : IEntityTypeConfiguration<ProyectoUsuario>
{
    public void Configure(EntityTypeBuilder<ProyectoUsuario> builder)
    {
        builder.ToTable("ProyectoUsuario");
        builder.HasKey(x => new
        {
            x.UsuarioId,
            x.ProyectoId
        });

        builder.Property(x => x.FechaIncorporacion)
            .IsRequired();
        builder.Property(x => x.Rol)
            .IsRequired();
        builder.Property(x => x.Activo)
            .IsRequired();

        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.Proyectos)
            .HasForeignKey(x => x.UsuarioId);
        builder.HasOne(x => x.Proyecto)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.ProyectoId);
    }
}