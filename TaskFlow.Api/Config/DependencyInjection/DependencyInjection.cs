using TaskFlow.Infrastructure.Services;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services.Token;
using TaskFlow.Infrastructure.Services.Token;
using TaskFlow.Core.Validations;
using TaskFlow.Api.Middlewares;

namespace TaskFlow.Api.Config.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTaskFlowServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IProyectoRepository, ProyectoRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();
            services.AddScoped<ITareaRepository, TareaRepository>();
            services.AddScoped<IEtiquetaRepository, EtiquetaRepository>();
            services.AddScoped<IHistorialRepository, HistorialRepository>();
            services.AddScoped<IProyectoUsuarioRepository, ProyectoUsuarioRepository>();
            services.AddScoped<ITareaEtiquetaRepository, TareaEtiquetaRepository>();

            // Services
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IProyectoService, ProyectoService>();
            services.AddScoped<IComentarioService, ComentarioService>();
            services.AddScoped<ITareaService, TareaService>();
            services.AddScoped<IEtiquetaService, EtiquetaService>();
            services.AddScoped<IHistorialService, HistorialService>();
            services.AddScoped<IProyectoUsuarioService, ProyectoUsuarioService>();
            services.AddScoped<ITareaEtiquetaService, TareaEtiquetaService>();
            services.AddScoped<ITokenService, TokenService>();

            // Permission Services
            services.AddScoped<IProyectoPermissionService, ProyectoPermissionService>();
            services.AddScoped<IComentarioPermissionService, ComentarioPermissionService>();
            services.AddScoped<ITareaPermissionService, TareaPermissionService>();
            services.AddScoped<IPassPermissionService, PassPermissionService>();

            return services;
        }
    }
}