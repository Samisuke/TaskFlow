using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

// Notas para un posible reclutador:
//
// Middleware encargado de gestionar las excepciones no controladas de la aplicación.
//
// Responsabilidades:
//  - Capturar las excepciones que se produzcan durante el procesamiento de una petición.
//  - Evitar que las excepciones lleguen directamente al cliente.
//  - Devolver una respuesta HTTP controlada y consistente.
//  - Evitar exponer información interna de la aplicación al cliente.
//
// De esta forma, los servicios y controllers no necesitan utilizar bloques
// try/catch para gestionar errores inesperados, manteniendo separada la gestión
// de excepciones de la lógica de negocio.
//
// Los errores inesperados se registrarán mediante el sistema de logging,
// permitiendo posteriormente utilizar Serilog para centralizar y monitorizar
// estos errores.

namespace TaskFlow.Api.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;
        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger, 
            IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Excepción no controlada capturada: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = _env.IsDevelopment()
                    ? exception.Message
                    : "Ha ocurrido un error inesperado en el servidor. Por favor, intente más tarde.",
                Instance = httpContext.Request.Path
            };

            // Informacion adicional del problema
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["timeStamp"] = DateTime.UtcNow;
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                problemDetails.Extensions["user"] = httpContext.User.Identity.Name;
            }
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}