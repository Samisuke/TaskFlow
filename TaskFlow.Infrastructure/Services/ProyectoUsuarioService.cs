using TaskFlow.Core.Services;
using TaskFlow.Core.Common;
using TaskFlow.Core.Models;
using TaskFlow.Core.Enums;
using Taskflow.Core.Repositories;

namespace TaskFlow.Infrastructure.Services
{
    public class ProyectoUsuarioService : IProyectoUsuarioService
    {
        // Inyección del repositorio
        private readonly IProyectoUsuarioRepository _repoProyectoUsuario;

        public ProyectoUsuarioService(IProyectoUsuarioRepository repoProyectoUsuario)
        {
            _repoProyectoUsuario = repoProyectoUsuario;
        }

        // Peticiones GET
        // Todos los usuarios de un proyecto. Útil para ver de un vistazo quien trabaja en ese proyecto.
        public async Task<Result<IEnumerable<ProyectoUsuario>>> GetTodosLosUsuariosDeUnProyectoAsync(int idProyecto)
        {
            var usuarios = await _repoProyectoUsuario.ObtenerTodosUsuariosDeUnProyectoAsync(idProyecto);
            if (!usuarios.Any()) return Result<IEnumerable<ProyectoUsuario>>.Mal("El Proyecto aun no tiene usuarios.");

            return Result<IEnumerable<ProyectoUsuario>>.Bien(usuarios);
        }

        // Un usuario específico de un proyecto. Incluye la información de su perfil. Útil si te interesa ver más información de un usuario.
        public async Task<Result<ProyectoUsuario?>> GetUsuarioDeUnProyectoAsync(int idProyecto, int idUsuario)
        {
            var usuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idUsuario);
            if (usuario is null) return Result<ProyectoUsuario?>.Mal("El proyecto no tiene ese usuario asignado.");

            return Result<ProyectoUsuario?>.Bien(usuario);
        }
        
        //Petición POST
        // Añadir un usuario a un proyecto.
        public async Task<Result<ProyectoUsuario>> PostUsuarioAsync(
            int usuarioId,
            int proyectoId,
            // El usuario estará activo por defecto cuando lo añadas a un proyecto.
            RolProyecto rolUsuario
        )
        {
            var usuario = new ProyectoUsuario
            {
                UsuarioId = usuarioId,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = proyectoId,
                Rol = rolUsuario,
                Activo = true
            };

            await _repoProyectoUsuario.CrearUsuarioAsync(usuario);
            var guardadoExistoso = await _repoProyectoUsuario.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<ProyectoUsuario>.Mal("ERROR. Fallo inesperado al guardar la tarea. Inténtalo de nuevo más tarde.");

            return Result<ProyectoUsuario>.Bien(usuario);
        }

        //Petición PATCH
        // Modificar el estado y el rol de un usuario de un proyecto. Solo puede hacerse si eres Admin o Gestor del proyecto.
        public async Task<Result<ProyectoUsuario>> PatchUsuarioAsync(
            int idPropia,
            int idUsuarioACambiar,
            int idProyecto,
            bool? activoUsuario,
            RolProyecto? rolUsuario
        )
        {
            int numeroCambios = 0;

            var usuario = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idUsuarioACambiar);
            if (usuario is null) return Result<ProyectoUsuario>.Mal("No se encuentra el usuario.");

            var comprobacion = await _repoProyectoUsuario.ObtenerUnUsuarioDeUnProyectoAsync(idProyecto, idPropia);
            if (comprobacion is null) return Result<ProyectoUsuario>.Mal("Ha ocurrido un error inesperado, intentalo de nuevo mas tarde.");

            if (activoUsuario.HasValue)
            {
                if (comprobacion.Rol.ToString() == "Manager" || comprobacion.Rol.ToString() == "Administrador")
                {
                    usuario.Activo = activoUsuario.Value;
                    numeroCambios += 1;    
                }
            }
            if (rolUsuario.HasValue)
            {
                if (comprobacion.Rol.ToString() == "Manager" || comprobacion.Rol.ToString() == "Administrador")
                {
                    usuario.Rol = rolUsuario.Value;
                    numeroCambios += 1;
                }    
            }

            var guardadoExistoso = await _repoProyectoUsuario.GuardarCambiosAsync();
            if (!guardadoExistoso) return Result<ProyectoUsuario>.Mal("ERROR. Fallo inesperado al guardar la tarea. Inténtalo de nuevo más tarde.");

            return Result<ProyectoUsuario>.Bien(usuario);
        }
    }
}