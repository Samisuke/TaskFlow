using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;

        public ComentarioRepository(TaskFlowDbContext context)
        {
            _context = context;
        }
        
        // Métodos GET
        // Obtener comentarios de una tarea con información del usuario.
        public async Task <(IEnumerable<Comentario> Comentarios, int TotalItems)> ObtenerComentariosDeUnaTareaAsync(
            int tareaId,
            int pagina,
            int tamanoPagina
        )
        {
            var query = _context.Comentarios
                .Where(c => c.TareaId == tareaId)
                .Include(c => c.Usuario);

            var totalItems = await query.CountAsync();
            var comentarios = await query
                .OrderBy(c => c.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();
            
            return (comentarios, totalItems);
        }

        // Comentarios de un usuario con información de la tarea.
        public async Task <(IEnumerable<Comentario> Comentarios, int TotalItems)> ObtenerComentariosDeUnUsuarioAsync(
            int usuarioId,
            int pagina,
            int tamanoPagina
        )
        {
            var query = _context.Comentarios
                .Where(c => c.UsuarioId == usuarioId)
                .Include(c => c.Tarea);

            var totalItems = await query.CountAsync();
            var comentarios = await query
                .OrderBy(c => c.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();   
            
            return (comentarios, totalItems);
        }

        // Comentario completo por su ID.
        public async Task <Comentario?> ObtenerComentarioPorIdAsync(int id)
        {
            return await _context.Comentarios
                .Include(c => c.Usuario)
                .Include(c => c.Tarea)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Métodos POST
        public async Task CrearComentarioAsync(Comentario comentario)
        {
            await _context.Comentarios.AddAsync(comentario);
        }
    }
}