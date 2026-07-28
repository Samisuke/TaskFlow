using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace TaskFlow.Infrastructure.Repositories
{
    public class UsuarioReposity : IUsuarioRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        public UsuarioReposity(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Obtención de usuarios
        public async Task <IEnumerable<Usuario>> ObtenerTodosUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Proyectos)
                .Include(u => u.Comentarios)
                .Include(u => u.TareasAsignadas)
                .Include(u => u.TareasCreadas)
                .ToListAsync();
        }

        public async Task <IEnumerable<ProyectoUsuario>> ObtenerTodosUsuariosDeProyectoAsync(int idProyecto)
        {
            return await _context.ProyectoUsuario
                .Where(u => u.ProyectoId == idProyecto)
                .Include(u => u.Usuario)
                .Include(u => u.Proyecto)
                .ToListAsync();
        }
        
        public async Task <Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.Proyectos)
                .Include(u => u.Comentarios)
                .Include(u => u.TareasAsignadas)
                .Include(u => u.TareasCreadas)
                .FirstOrDefaultAsync(u => u.Id == idUsuario);
        }
        public async Task <Usuario?> ObtenerUsuarioPorEmailAsync(string email)
        {
           return await _context.Usuarios
                .Include(u => u.Proyectos)
                .Include(u => u.Comentarios)
                .Include(u => u.TareasAsignadas)
                .Include(u => u.TareasCreadas)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Misc.

        public async Task CrearUnUsuarioNuevoAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}