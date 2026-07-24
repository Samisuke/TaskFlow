using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Comentario> Comentarios {get; set;}
        public DbSet<Etiqueta> Etiquetas {get; set;}
        public DbSet<Historial> Historiales {get; set;}
        public DbSet<Proyecto> Proyectos {get; set;}
        public DbSet<ProyectoUsuario> ProyectoUsuario {get; set;}
        public DbSet<Tarea> Tareas {get; set;}
        public DbSet<TareaEtiqueta> TareaEtiquetas {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}