using Microsoft.EntityFrameworkCore;
using Taskflow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly TaskFlowDbContext _context;
        public ProyectoRepository(TaskFlowDbContext context)
        {
            _context = context;
        }
        // GET
        // Proyectos con sus usuarios completos, tareas y etiquetas
        public async Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnUsuarioAsync(int idUsuario)
        {
            return await _context.Proyectos
                .Where(x => x.Usuarios
                    .Any(usuario => usuario.UsuarioId == idUsuario))
                .Include(x => x.Tareas)
                    .ThenInclude(x => x.Etiquetas)
                        .ThenInclude(x => x.Etiqueta)
                .Include(x => x.Usuarios)
                    .ThenInclude(x => x.Usuario)
                .ToListAsync(); 
        }

       // Proyectos simples 
        public async Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int idCreador)
        {
            return await _context.Proyectos
                .Where(x => x.PropietarioId == idCreador)
                .ToListAsync();
        }

        // Misc.
        public async Task CrearProyectoAsync(Proyecto proyecto)
        {
            await _context.Proyectos.AddAsync(proyecto);
        }
        public async Task<bool> GuardarCambiosASync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}