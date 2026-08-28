using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TaskFlow.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;



namespace TaskFlow.IntegrationTests
{
    // Creación de la Factory (entorno de pruebas) usando la configuración de nuestro Program.
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // Creamos los parametros del contenedor para Docker que almacenará nuestro DB
        private readonly PostgreSqlContainer _postgreContainer = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("taskflow_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        // Iniciamos el container
        public async Task InitializeAsync()
        {
            await _postgreContainer.StartAsync();
        }

        // Configuraciones.
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

        public TaskFlowDbContext GetDbContext()
        {
            // Creamos un scope dentro de la factory
            var scope = Services.CreateScope();

            // Creamos el context dentro de este scope
            var context = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

            return context;
        } 
        

        // Borramos el container
        public new async Task DisposeAsync()
        {
            await _postgreContainer.DisposeAsync();
        }
    }
}