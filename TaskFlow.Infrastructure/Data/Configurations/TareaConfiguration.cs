using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Data.Configurations;

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.ToTable("Tareas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Titulo)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Descripcion)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Estado)
            .IsRequired();
        builder.Property(x => x.Prioridad)
            .IsRequired();
        builder.Property(x => x.FechaCreacion)
            .IsRequired();
        builder.Property(x => x.FechaLimite)
            .IsRequired();

        builder.Property(x => x.AsignadoId)
            .IsRequired();
        builder.HasOne(x => x.Asignado)
            .WithMany(x => x.TareasAsignadas)
            .HasForeignKey(x => x.AsignadoId);

        builder.Property(x => x.CreadorId)
            .IsRequired();
        builder.HasOne(x => x.Creador)
            .WithMany(x => x.TareasCreadas)
            .HasForeignKey(x => x.CreadorId);

        builder.Property(x => x.ProyectoId)
            .IsRequired();
        builder.HasOne(x => x.Proyecto)
            .WithMany(x => x.Tareas)
            .HasForeignKey(x => x.ProyectoId);

        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Tarea)
            .HasForeignKey(x => x.TareaId);
        builder.HasMany(x => x.Historiales)
            .WithOne(x => x.Tarea)
            .HasForeignKey(x => x.TareaId);
    }
}
