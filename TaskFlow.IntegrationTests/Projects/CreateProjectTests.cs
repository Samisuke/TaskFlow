using System.Net;
using FluentAssertions;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskFlow.Core.Dto.Proyecto;
using System.Net.Http.Json;

namespace TaskFlow.IntegrationTests.Projects
{
    public class ProjectsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        // Recibimos la Factory
        public ProjectsTests(
            CustomWebApplicationFactory factory)
        {   
            // _client es un cliente para el entorno de pruebas.
            _client = factory.CreateClient();
            _factory = factory;
        } 

        [Fact]
        public async Task GetProjects_ReturnsOk()
        {
            
            // Obtenemos el context
            var context = _factory.GetDbContext();

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

            var proyecto = new Proyecto
            {
                Id = 1,
                Nombre  = "Test Nom.",
                Descripcion = "Test Desc.",
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = 1,
                Propietario = null,
                Tareas = [],
                Usuarios = [],
                Historiales = []
            };

            var proyectoUsuario = new ProyectoUsuario
            {
                UsuarioId = 1,
                Usuario = usuario,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 1,
                Proyecto = proyecto,
                Rol = RolProyecto.Manager,
                Activo = true,
            };

            await context.ProyectoUsuario.AddAsync(proyectoUsuario);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoReadDto>>();

            proyectos.Should().NotBeNull();
            proyectos.Should().ContainSingle();
            
            proyectos![0].Nombre.Should().Be("Test Nom.");
            proyectos[0].Descripcion.Should().Be("Test Desc.");
        }
    }
}