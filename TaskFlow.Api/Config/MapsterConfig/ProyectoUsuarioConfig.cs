using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Api.Dto.ProyectoUsuario;

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

            TypeAdapterConfig<Tarea, ProeyectoUsuarioReadDtoTareasCreadas>
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