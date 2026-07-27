using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Core.Models;

namespace TaskFlow.Api.Config.MapsterConfig
{
    public static class MapsterConfiguration
    {
        public static void AddMapsterMappings(this IServiceCollection services)
        {
            ComentarioConfig.RegisterMapsterConfiguration(services);
            ProyectoUsuarioConfig.RegisterMapsterConfiguration(services);
            HistorialConfig.RegisterMapsterConfiguration(services);
            ProyectoConfig.RegisterMapsterConfiguration(services);
            TareaConfig.RegisterMapsterConfiguration(services);
            UsuarioConfig.RegisterMapsterConfiguration(services);
        }
    }
}