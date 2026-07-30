using Microsoft.EntityFrameworkCore;
using Taskflow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProyectoUsuarioRepository : IProyectoUsuarioRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        public ProyectoUsuarioRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Peticiones GET
        // Todos los usuarios de un proyecto
        public async Task <IEnumerable<ProyectoUsuario>> ObtenerTodosUsuariosDeUnProyectoAsync(int idProyecto)
        {
            return await _context.ProyectoUsuario
            .Where(u => u.ProyectoId == idProyecto)
            .Include(u => u.Usuario)
            .ToListAsync(); 
        }
        
        // Un solo usuario de un proyecto
        public async Task <ProyectoUsuario?> ObtenerUnUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario)
        {
            return await _context.ProyectoUsuario
            .Where(u => u.ProyectoId == idProyecto)
            .Include(u => u.Usuario)
            .FirstOrDefaultAsync(u => u.UsuarioId == idUsuario);
        }

        // Peticiones POST
        public async Task CrearUsuarioAsync(ProyectoUsuario usuario)
        {
            await _context.AddAsync(usuario);
        }

        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}