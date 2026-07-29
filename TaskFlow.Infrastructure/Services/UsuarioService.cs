using TaskFlow.Core.Common;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Models;

namespace TaskFlow.Infrastructure.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Inyección del repositorio
        private readonly IUsuarioRepository _repoUsuario;
        public UsuarioService(IUsuarioRepository repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        // Métodos GET
        public async Task<Result<IEnumerable<Usuario>>> GetTodosUsuariosAsync()
        {
            var usuarios = await _repoUsuario.ObtenerTodosUsuariosAsync();
            if (!usuarios.Any()) return Result<IEnumerable<Usuario>>.Mal("ERROR. La lista está vacía.");

            return Result<IEnumerable<Usuario>>.Bien(usuarios);
        }

        public async Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosUsuariosPorProyectoAsync(int idProyecto)
        {
            var usuarios = await _repoUsuario.ObtenerTodosUsuariosDeProyectoAsync(idProyecto);
            if (usuarios is null) return Result<IEnumerable<ProyectoUsuario>>.Mal("ERROR. No se encuentra el proyecto.");
            if (!usuarios.Any()) return Result<IEnumerable<ProyectoUsuario>>.Mal("ERROR. La lista está vacía.");

            return Result<IEnumerable<ProyectoUsuario>>.Bien(usuarios);
        }

        public async Task<Result<Usuario>> GetUsuarioPorIdAsync(int idUsuario)
        {
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario is null) return Result<Usuario>.Mal("ERROR. No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }

        public async Task<Result<Usuario>> GetUsuarioPorEmailAsync(string emailUsuario)
        {
            var usuario = await _repoUsuario.ObtenerUsuarioPorEmailAsync(emailUsuario);
            if (usuario is null) return Result<Usuario>.Mal("Error. No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }
        
        // Métodos POST
        public async Task<Result<Usuario>> PostUsuarioAsync(
            string nombreUsuario,
            string apellidosUsuario,
            string emailUsuario,
            string passUsuario,
            bool activoUsuario
        )
        {
            var usuario = new Usuario
            {
                Nombre = nombreUsuario,
                Apellidos = apellidosUsuario,
                Email = emailUsuario,
                PasswordHash = passUsuario,
                FechaRegistro = DateTime.UtcNow,
                Activo = activoUsuario
            };

            await _repoUsuario.CrearUnUsuarioNuevoAsync(usuario);
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("ERROR. Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");
            
            return Result<Usuario>.Bien(usuario);
        }

        // Métodos PATCH
        public async Task<Result<Usuario>> PatchUsuarioAsync(
            int idUsuario,
            string? nombreUsuario,
            string? apellidosUsuario,
            string? emailUsuario,
            bool? activoUsuario
        )
        {
            int numeroCambios = 0;
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario is null) return Result<Usuario>.Mal("ERROR. No se encuentra el usuario.");

            if (nombreUsuario is not null)
            {
                usuario.Nombre = nombreUsuario;
                numeroCambios += 1;
            } 
            if (apellidosUsuario is not null)
            {
                usuario.Apellidos = apellidosUsuario;
                numeroCambios += 1;
            } 
            if (emailUsuario is not null)
            {
                var correoRepetido = await _repoUsuario.ObtenerUsuarioPorEmailAsync(emailUsuario);
                if (correoRepetido is not null) return Result<Usuario>.Mal("ERROR. El email ya está registrado, prueba otro.");
                if(emailUsuario is not null) usuario.Email = emailUsuario;
                numeroCambios += 1;
            } 
            if (activoUsuario.HasValue)
            {
                usuario.Activo = activoUsuario.Value;
                numeroCambios += 1;
            } 

            if (numeroCambios == 0) return Result<Usuario>.Mal("ERROR. No se han detectado cambios.");
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");
            
            return Result<Usuario>.Bien(usuario);
        }

        public async Task<Result<Usuario>> PatchUsuarioPassAsync(int idUsuario, string? passNueva)
        {
            int numeroCambios = 0;
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario is null) return Result<Usuario>.Mal("ERROR. Usuario no encontrado.");
            
            if (passNueva is not null)
            {
                //Convertir contraseña a segura.
                usuario.PasswordHash = passNueva;
                numeroCambios += 1;
            } 

            if (numeroCambios == 0) return Result<Usuario>.Mal("ERROR. No se han detectado cambios");
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Usuario>.Bien(usuario);
        }
    }
}