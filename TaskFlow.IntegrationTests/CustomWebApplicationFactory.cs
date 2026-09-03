using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TaskFlow.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

// <summary>
// Configura un entorno aislado para las pruebas de integración de la aplicación.

// Utiliza WebApplicationFactory para levantar la aplicación real y Testcontainers
// para ejecutar una instancia temporal de PostgreSQL dentro de Docker.

// También sustituye la autenticación real por un esquema de autenticación específico
// para las pruebas.
// </summary>

namespace TaskFlow.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // Contenedor PostgreSQL utilizado como base de datos durante las pruebas de integración.
        private readonly PostgreSqlContainer _postgreContainer = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("taskflow_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        // Inicia el contenedor de PostgreSQL antes de ejecutar las pruebas.
        public async Task InitializeAsync()
        {
            await _postgreContainer.StartAsync();
        }

        // Configura la aplicación para ejecutarse en el entorno de pruebas.
        // Sustituye la cadena de conexión de producción por la del contenedor PostgreSQL
        // y reemplaza el sistema de autenticación real por el utilizado en las pruebas.
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Usamos el DB_test, no el DB de producción.
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        _postgreContainer.GetConnectionString()
                });
            });

            // JWT para saltarnos los authorize.
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestAuthentication";
                    options.DefaultChallengeScheme = "TestAuthentication";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "TestAuthentication",
                    options => { });
            });
        }

        // Restablece el estado de la base de datos de pruebas eliminándola y aplicando
        ///nuevamente las migraciones de Entity Framework Core. 
        /// De esta forma, cada prueba comienza con un estado conocido y aislado.
        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<TaskFlowDbContext>();
            
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }

        // Ejecuta una operación sobre el DbContext dentro de un ámbito controlado de
        // inyección de dependencias, garantizando la liberación de los servicios con
        // ciclo de vida Scoped al finalizar la operación.
        public async Task ExecuteDbContextAsync(
            Func<TaskFlowDbContext, Task> action)
        {
            // Creamos el scope para que viva solo mientras se ejecute el bloque
            await using var scope = Services.CreateAsyncScope();
            
            var context = scope.ServiceProvider
                .GetRequiredService<TaskFlowDbContext>();

            await action(context);
        }
        

        // Libera el contenedor PostgreSQL utilizado durante las pruebas.
        public new async Task DisposeAsync()
        {
            await _postgreContainer.DisposeAsync();
        }
    }
}