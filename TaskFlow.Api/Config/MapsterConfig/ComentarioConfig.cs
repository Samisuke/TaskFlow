using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Comentario;

// Notas para un posible reclutador:
// Mapeo automático de los Models y sus DTO.
// Misión: No tener que mapear en los servicios cada cosa y que quede un servicio más limpio.

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class ComentarioConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Comentario, ComentarioReadDto>
                .NewConfig()
                .Map(dest => dest.Usuario.NombreUsuario,
                src => src.Usuario != null
                ? src.Usuario.Nombre : string.Empty)

                .Map(dest => dest.Tarea.NombreTarea,
                src => src.Tarea != null!
                ? src.Tarea.Titulo : string.Empty);    
        }   
    }
}