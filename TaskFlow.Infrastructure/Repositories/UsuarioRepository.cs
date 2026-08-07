using TaskFlow.Core.Repositories;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace TaskFlow.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        // Inyección del context
        private readonly TaskFlowDbContext _context;
        public UsuarioRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        // Métodos GET
        // Todos los usuarios simples con sus proyectos, sin informacion adicional.
        public async Task <IEnumerable<Usuario>> ObtenerTodosUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Proyectos)
                .ToListAsync();
        }
        
        // Usuario completo por ID
        public async Task <Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.Proyectos)
                .Include(u => u.Comentarios)
                .Include(u => u.TareasAsignadas)
                .Include(u => u.TareasCreadas)
                .FirstOrDefaultAsync(u => u.Id == idUsuario);
        }

        // Usuario completo por email
        public async Task <Usuario?> ObtenerUsuarioPorEmailAsync(string email)
        {
           return await _context.Usuarios
                .Include(u => u.Proyectos)
                .Include(u => u.Comentarios)
                .Include(u => u.TareasAsignadas)
                .Include(u => u.TareasCreadas)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Métodos POST
        public async Task CrearUnUsuarioNuevoAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        // Misc.
        // Guardar
        public async Task <bool> GuardarCambiosAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}