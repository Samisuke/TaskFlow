using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.ProyectoUsuario;

// Notas para un posible reclutador:
// Mapeo automático de los Models y sus DTO.
// Misión: No tener que mapear en los servicios cada cosa y que quede un servicio más limpio.

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class ProyectoUsuarioConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Usuario, ProyectoUsuarioReadDtoUsuario>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Nombre)

                .Map(dest => dest.Apellidos,
                src => src.Apellidos)

                .Map(dest => dest.Email,
                src => src.Email);

            TypeAdapterConfig<Tarea, ProyectoUsuarioReadDtoTareasAsignadas>
                .NewConfig()
                .Map(dest => dest.Titulo,
                src => src.Titulo)

                .Map(dest => dest.Descripcion,
                src => src.Descripcion)

                .Map(dest => dest.Estado,
                src => src.Estado.ToString())

                .Map(dest => dest.Prioridad,
                src => src.Prioridad.ToString());

            TypeAdapterConfig<Tarea, ProyectoUsuarioReadDtoTareasCreadas>
                .NewConfig()
                .Map(dest => dest.Titulo,
                src => src.Titulo)

                .Map(dest => dest.Descripcion,
                src => src.Descripcion)

                .Map(dest => dest.Estado,
                src => src.Estado.ToString())

                .Map(dest => dest.Prioridad,
                src => src.Prioridad.ToString());
        }   
    }
}