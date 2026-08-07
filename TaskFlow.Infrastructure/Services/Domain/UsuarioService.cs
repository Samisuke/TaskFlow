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
        private readonly IPassPermissionService _passPermission;

        public UsuarioService(
            IUsuarioRepository repoUsuario,
            IPassPermissionService passPermission
        )
        {
            _repoUsuario = repoUsuario;
            _passPermission = passPermission;
        }

        // Métodos GET
        // Obtener todos los usuarios de la base de datos. Solo útil para gestores de la misma.
        public async Task<Result<IEnumerable<Usuario>>> GetTodosUsuariosAsync()
        {
            var usuarios = await _repoUsuario.ObtenerTodosUsuariosAsync();
            if (!usuarios.Any()) return Result<IEnumerable<Usuario>>.Mal("La lista está vacía.");

            return Result<IEnumerable<Usuario>>.Bien(usuarios);
        }

        // Obtener un usuario concreto. Útil para ver tu perfil de usuario.
        public async Task<Result<Usuario>> GetUsuarioPorIdAsync(int idUsuario)
        {
            
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario is null) return Result<Usuario>.Mal("No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }

        // Obtener un usuario por su email. Útil para búsquedas de perfiles de usuario dentro de la aplicación.
        public async Task<Result<Usuario>> GetUsuarioPorEmailAsync(string emailUsuario)
        {
            var usuario = await _repoUsuario.ObtenerUsuarioPorEmailAsync(emailUsuario);
            if (usuario is null) return Result<Usuario>.Mal("No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }
        
        // Métodos POST
        // Crear un usuario.
        public async Task<Result<Usuario>> PostUsuarioAsync(
            string nombreUsuario,
            string apellidosUsuario,
            string emailUsuario,
            string passUsuario
        )
        {   
            // Crear usuario.
            var usuario = new Usuario
            {
                Nombre = nombreUsuario,
                Apellidos = apellidosUsuario,
                Email = emailUsuario,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(passUsuario),
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            // Base de datos.
            await _repoUsuario.CrearUnUsuarioNuevoAsync(usuario);
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");
            
            return Result<Usuario>.Bien(usuario);
        }

        // Métodos PATCH
        // Modificar tu perfil de usuario. No permite cambiar la contraseña.
        public async Task<Result<Usuario>> PatchUsuarioAsync(
            int usuarioId,
            string? nombreUsuario,
            string? apellidosUsuario,
            string? emailUsuario,
            bool? activoUsuario
        )
        {
            int numeroCambios = 0;
            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuario is null) return Result<Usuario>.Mal("No se encuentra el usuario.");

            // Realizar cambios
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
                if (correoRepetido is not null) return Result<Usuario>.Mal("El email ya está registrado, prueba otro.");
                if(emailUsuario is not null) usuario.Email = emailUsuario;
                numeroCambios += 1;
            } 
            if (activoUsuario.HasValue)
            {
                usuario.Activo = activoUsuario.Value;
                numeroCambios += 1;
            } 

            // Base de datos
            if (numeroCambios == 0) return Result<Usuario>.Mal("No se han detectado cambios.");
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");
            
            return Result<Usuario>.Bien(usuario);
        }

        // Modificar la contraseña. Separado del patch normal por seguridad.
        public async Task<Result<Usuario>> PatchUsuarioPassAsync(int usuarioId, string passNueva, string passAntigua)
        {

            var usuario = await _repoUsuario.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuario is null) return Result<Usuario>.Mal("Usuario no encontrado.");
            
            
            // Comprobaciones.
            var comprobaciones = _passPermission.ComprobacionesPass(usuario, passAntigua, passNueva);
            if (!comprobaciones.EsCorrecto) return Result<Usuario>.Mal(comprobaciones.Error);
            
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passNueva);
            
            // Base de datos.
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Usuario>.Bien(usuario);
        }
    }
}