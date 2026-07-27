using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Api.Dto.Historial;

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class HistorialConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<Historial, HistorialReadDtoUsuario>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Usuario != null
                ? src.Usuario.Nombre : string.Empty);

            TypeAdapterConfig<Comentario, HistorialReadDtoTarea>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Tarea != null!
                ? src.Tarea.Titulo : string.Empty)
                
                .Map(dest => dest.Descripcion,
                src => src.Tarea != null!
                ? src.Tarea.Descripcion : string.Empty);       
        }   
    }
}