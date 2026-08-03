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
            if (usuario is null) return Result<Usuario>.Mal("ERROR. No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }

        // Obtener un usuario por su email. Útil para búsquedas de perfiles de usuario dentro de la aplicación.
        public async Task<Result<Usuario>> GetUsuarioPorEmailAsync(string emailUsuario)
        {
            var usuario = await _repoUsuario.ObtenerUsuarioPorEmailAsync(emailUsuario);
            if (usuario is null) return Result<Usuario>.Mal("Error. No se encuentra el usuario.");

            return Result<Usuario>.Bien(usuario);
        }
        
        // Métodos POST
        // Crear un usuario.
        public async Task<Result<Usuario>> PostUsuarioAsync(
            string nombreUsuario,
            string apellidosUsuario,
            string emailUsuario,
            string passUsuario,
            bool activoUsuario
        )
        {   
            // Crear usuario.
            var usuario = new Usuario
            {
                Nombre = nombreUsuario,
                Apellidos = apellidosUsuario,
                Email = emailUsuario,
                PasswordHash = passUsuario,
                FechaRegistro = DateTime.UtcNow,
                Activo = activoUsuario
            };

            // Base de datos.
            await _repoUsuario.CrearUnUsuarioNuevoAsync(usuario);
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("ERROR. Fallo inesperado al guardar el usuario. Inténtalo de nuevo más tarde.");
            
            return Result<Usuario>.Bien(usuario);
        }

        // Métodos PATCH
        // Modificar un usuario.
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
                if (correoRepetido is not null) return Result<Usuario>.Mal("ERROR. El email ya está registrado, prueba otro.");
                if(emailUsuario is not null) usuario.Email = emailUsuario;
                numeroCambios += 1;
            } 
            if (activoUsuario.HasValue)
            {
                usuario.Activo = activoUsuario.Value;
                numeroCambios += 1;
            } 

            // Base de datos
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
            
            // Cambios.
            if (passNueva is not null)
            {
                //Convertir contraseña a segura.
                usuario.PasswordHash = passNueva;
                numeroCambios += 1;
            } 

            // Base de datos.
            if (numeroCambios == 0) return Result<Usuario>.Mal("ERROR. No se han detectado cambios");
            var guardadoExitoso = await _repoUsuario.GuardarCambiosAsync();
            if (!guardadoExitoso) return Result<Usuario>.Mal("ERROR. Fallo inesperado al guardar los cambios. Inténtalo de nuevo más tarde.");

            return Result<Usuario>.Bien(usuario);
        }
    }
}