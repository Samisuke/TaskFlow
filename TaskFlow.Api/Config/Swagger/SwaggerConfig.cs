using Microsoft.OpenApi;

namespace TaskFlow.Api.Config
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddTaskFlowSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TaskFlow API",
                    Version = "v1",
                    Description = "API REST para la gestión de proyectos y tareas."
                });

                // Configuración del JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Introduce tu token JWT."
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
            });

            return services;
        }
    }
}