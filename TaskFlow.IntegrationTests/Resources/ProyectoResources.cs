using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;

// <summary>
// Métodos auxiliares para crear entidades con datos de prueba utilizados
// en las pruebas de integración relacionadas con proyectos.
// </summary>

namespace TaskFlow.IntegrationTests.Resources
{
    public static class ProyectoResources
    {
        public static Usuario CrearUsuario(int usuarioId)
        {
            return new Usuario
            {
                Id = usuarioId,
                Nombre = $"Nombre User {usuarioId}",
                Apellidos = $"Nombre User {usuarioId}r",
                Email = $"test{usuarioId}@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],
            };
        }

        public static Proyecto CrearProyecto(int proyectoId)
        {
            return new Proyecto
            {
                Id = proyectoId,
                Nombre  = $"Test {proyectoId}",
                Descripcion = "Test Desc.",
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = 1,
                Propietario = null,
                Tareas = [],
                Usuarios = [],
                Historiales = []
            };
        }
        public static ProyectoUsuario CrearProyectoUsuario(Usuario usuario, Proyecto proyecto)
        {
            return new ProyectoUsuario
            {
                UsuarioId = usuario.Id,
                Usuario = usuario,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = proyecto.Id,
                Proyecto = proyecto,
                Rol = RolProyecto.Manager,
                Activo = true,
            };
        }
    }
}