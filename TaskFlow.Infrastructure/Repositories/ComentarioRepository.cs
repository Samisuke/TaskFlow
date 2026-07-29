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
        // Obtener comentarios de una tarea con información del usuario.
        public async Task <IEnumerable<Comentario>> ObtenerComentariosDeUnaTareaAsync(int idTarea)
        {
            return await _context.Comentarios
                .Where(c => c.TareaId == idTarea)
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        // Comentarios de un usuario con información de la tarea.
        public async Task <IEnumerable<Comentario>> ObtenerComentariosDeUnUsuarioAsync(int idUsuario)
        {
            return await _context.Comentarios
                .Where(c => c.UsuarioId == idUsuario)
                .Include(c => c.Tarea)
                .ToListAsync();      
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

        // Métodos PATCH
        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}