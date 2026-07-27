using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Api.Dto.Tarea;

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class TareaConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Usuario, TareaReadDtoUsuarioAsignado>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Nombre);

            TypeAdapterConfig<Usuario, TareaReadDtoUsuarioCreador>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Nombre);

            TypeAdapterConfig<Proyecto, TareaReadDtoProyecto>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Nombre);

        }   
    }
}