using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Usuario;

// Notas para un posible reclutador:
// Mapeo automático de los Models y sus DTO.
// Misión: No tener que mapear en los servicios cada cosa y que quede un servicio más limpio.

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class UsuarioConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<ProyectoUsuario, UsuarioReadDtoProyectos>
                .NewConfig()
                .Map(dest => dest.Nombre, 
                src => src.Proyecto != null
                ? src.Proyecto.Nombre : string.Empty);

            TypeAdapterConfig<Comentario, UsuarioReadDtoComentarios>
                .NewConfig()
                .Map(dest => dest.Contenido,
                src => src.Contenido)

                .Map(dest => dest.PerteneceA,
                src => src.Tarea != null!
                ? src.Tarea.Titulo : string.Empty);
        }   
    }
}