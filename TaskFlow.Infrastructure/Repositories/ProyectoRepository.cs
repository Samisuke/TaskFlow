using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProyectoRepository : IProyectoRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        public ProyectoRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Métodos GET
        // Proyectos con sus usuarios completos, tareas y etiquetas.
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

        // Proyectos simples de un creador, sin informacion adicional.
        public async Task <IEnumerable<Proyecto>> ObtenerProyectosDeUnCreadorAsync(int idCreador)
        {
            return await _context.Proyectos
                .Where(x => x.PropietarioId == idCreador)
                .ToListAsync();
        }

        // Proyectos simples por ID de proyecto, con sus usuarios.
        public async Task <Proyecto?> ObtenerProyectoPorIdAsync(int idProyecto)
        {
            return await _context.Proyectos
            .Include(x => x.Usuarios)
            .FirstOrDefaultAsync(x => x.Id == idProyecto);
        }

        // Métodos POST
        public async Task CrearProyectoAsync(Proyecto proyecto)
        {
            await _context.Proyectos.AddAsync(proyecto);
        }
    }
}