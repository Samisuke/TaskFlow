using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Proyecto;

// Notas para un posible reclutador:
// Mapeo automático de los Models y sus DTO.
// Misión: No tener que mapear en los servicios cada cosa y que quede un servicio más limpio.

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class ProyectoConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Proyecto, ProyectoReadDto>
                .NewConfig()
                .Map(dest => dest.NombrePropietario,
                src => src.Propietario != null
                ? src.Propietario.Nombre : string.Empty);

            TypeAdapterConfig<Tarea, ProyectoReadDtoTareas>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Titulo != null
                ? src.Titulo : string.Empty)

                .Map(dest => dest.Descripcion,
                src => src.Descripcion != null
                ? src.Descripcion : string.Empty)

                .Map(dest => dest.UsuarioASignado,
                src => src.Asignado != null
                ? src.Asignado.Nombre : string.Empty);

            TypeAdapterConfig<ProyectoUsuario, ProyectoReadDtoUsuarios>
                .NewConfig()
                .Map(dest => dest.Id,
                src => src.Usuario != null
                ? src.Usuario.Id : 0)

                .Map(dest => dest.Nombre,
                src => src.Usuario != null
                ? src.Usuario.Nombre : string.Empty)

                .Map(dest => dest.Rol,
                src => src.Rol.ToString())

                .Map(dest => dest.FechaIncorporacion,
                src => src.FechaIncorporacion);
        }   
    }
}