using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(x => x.Apellidos)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(x => x.Email)
            .IsUnique();
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.FechaRegistro)
            .IsRequired();    
        builder.Property(x => x.Activo)
            .IsRequired();
            
        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Usuario)
            .HasForeignKey(x => x.UsuarioId);
        
        builder.HasMany(x => x.TareasAsignadas)
            .WithOne(x => x.Asignado)
            .HasForeignKey(x => x.AsignadoId);

        builder.HasMany(x => x.TareasCreadas)
            .WithOne(x => x.Creador)
            .HasForeignKey(x => x.CreadorId);

    }
}