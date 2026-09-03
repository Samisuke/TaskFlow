using System.Net;
using FluentAssertions;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Proyecto;
using System.Net.Http.Json;


namespace TaskFlow.IntegrationTests.ProyectoTest
{
    public class PostProyectoTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        // Recibimos la Factory
        public PostProyectoTests(
            CustomWebApplicationFactory factory)
        {   
            // _client es un cliente para el entorno de pruebas.
            _client = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task PostProyecto_ReturnsCreated()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Nombre User",
                Apellidos = "Apellidos User",
                Email = "test@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],
            };

            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddAsync(usuario);
                await context.SaveChangesAsync();
            });

            // Act
            var proyecto = new ProyectoWriteDto
            {
                Nombre  = "Test Nom. Uno",
                Descripcion = "Test Desc.",
            };
        
            TestAuthenticationHandler.UserId = 1;

            var response = await _client.PostAsJsonAsync("/api/proyecto", proyecto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PostProyecto_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Nombre User",
                Apellidos = "Apellidos User",
                Email = "test@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],
            };

            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddAsync(usuario);
                await context.SaveChangesAsync();
            });

            // Act
            var proyecto = new ProyectoWriteDto
            {
                Nombre  = "Test Nom. Uno",
            };
        
            TestAuthenticationHandler.UserId = 1;

            var response = await _client.PostAsJsonAsync("/api/proyecto", proyecto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}