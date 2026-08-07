using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Tarea;

// Notas para un posible reclutador:
// Mapeo automático de los Models y sus DTO.
// Misión: No tener que mapear en los servicios cada cosa y que quede un servicio más limpio.

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