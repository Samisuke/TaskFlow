using Mapster;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Historial;

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

            TypeAdapterConfig<Proyecto, HistorialReadDtoProyecto>
                .NewConfig()
                .Map(dest => dest.Nombre,
                src => src.Nombre != null!
                ? src.Nombre : string.Empty)
                
                .Map(dest => dest.Descripcion,
                src => src.Descripcion != null!
                ? src.Descripcion : string.Empty);       
        }   
    }
}