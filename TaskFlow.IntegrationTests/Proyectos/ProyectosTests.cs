using System.Net;
using FluentAssertions;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskFlow.Core.Dto.Proyecto;
using System.Net.Http.Json;

namespace TaskFlow.IntegrationTests.Proyectos
{
    public class ProyectosTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        // Recibimos la Factory
        public ProyectosTests(
            CustomWebApplicationFactory factory)
        {   
            // _client es un cliente para el entorno de pruebas.
            _client = factory.CreateClient();
            _factory = factory;
        } 

        [Fact]
        public async Task GetProyectos_ReturnsOk()
        {
            // Limpieza de BD entre cada test.
            await _factory.ResetDatabaseAsync();
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

            await context.Usuarios.AddAsync(usuario);
            await context.Proyectos.AddAsync(proyecto);
            await context.ProyectoUsuario.AddAsync(proyectoUsuario);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS: {response.StatusCode}");
            Console.WriteLine($"BODY: {body}");

            var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoReadDto>>();

            proyectos.Should().NotBeNull();
            proyectos.Should().ContainSingle();
            
            proyectos![0].Nombre.Should().Be("Test Nom.");
            proyectos[0].Descripcion.Should().Be("Test Desc.");
        }

        [Theory]
        [InlineData(1, new[] {1, 2, 3})]
        [InlineData(2, new[] {1, 2})]
        [InlineData(3, new [] {3})]
        public async Task GetProyectosConAuthentificacion_ReturnProyectosEsperados(int usuarioId, int [] proyectosEsperados)
        {
            // Limpieza de BD entre cada test.
            await _factory.ResetDatabaseAsync();

            var context = _factory.GetDbContext();

            // Arrange
            var usuarioUno = new Usuario
            {
                Id = 1,
                Nombre = "Nombre Uno",
                Apellidos = "Apellidos User",
                Email = "test1@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],
            };

            var usuarioDos = new Usuario
            {
                Id = 2,
                Nombre = "Nombre Dos",
                Apellidos = "Apellidos User",
                Email = "test2@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [], 
            };

            var usuarioTres = new Usuario
            {
                Id = 3,
                Nombre = "Nombre Tres",
                Apellidos = "Apellidos User",
                Email = "test3@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],
            };

            var proyectoUno = new Proyecto
            {
                Id = 1,
                Nombre  = "Test Nom. Uno",
                Descripcion = "Test Desc.",
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = 1,
                Propietario = null,
                Tareas = [],
                Usuarios = [],
                Historiales = []  
            };

            var proyectoDos = new Proyecto
            {
                Id = 2,
                Nombre  = "Test Nom. Dos",
                Descripcion = "Test Desc.",
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = 2,
                Propietario = null,
                Tareas = [],
                Usuarios = [],
                Historiales = []
            };

            var proyectoTres = new Proyecto
            {
                Id = 3,
                Nombre  = "Test Nom. Tres",
                Descripcion = "Test Desc.",
                FechaCreacion = DateTime.UtcNow,
                PropietarioId = 3,
                Propietario = null,
                Tareas = [],
                Usuarios = [],
                Historiales = []
            };

            var proyectoUsuarioUno = new ProyectoUsuario
            {
                UsuarioId = 1,
                Usuario = usuarioUno,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 1,
                Proyecto = proyectoUno,
                Rol = RolProyecto.Manager,
                Activo = true,
            };

            var proyectoUsuarioDos = new ProyectoUsuario
            {
                UsuarioId = 2,
                Usuario = usuarioDos,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 2,
                Proyecto = proyectoDos,
                Rol = RolProyecto.Manager,
                Activo = true,
            };

            var proyectoUsuarioTres = new ProyectoUsuario
            {
                UsuarioId = 3,
                Usuario = usuarioTres,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 3,
                Proyecto = proyectoTres,
                Rol = RolProyecto.Manager,
                Activo = true,   
            };

            var proyectoUserUnoenTres = new ProyectoUsuario
            {
                UsuarioId = 1,
                Usuario = usuarioUno,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 3,
                Proyecto = proyectoTres,
                Rol = RolProyecto.Manager,
                Activo = true,    
            };

            var proyectoUserUnoenDos = new ProyectoUsuario
            {
                UsuarioId = 1,
                Usuario = usuarioUno,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 2,
                Proyecto = proyectoDos,
                Rol = RolProyecto.Manager,
                Activo = true,    
            };
    
            var proyectoUserDosenUno = new ProyectoUsuario
            {
                UsuarioId = 2,
                Usuario = usuarioDos,
                FechaIncorporacion = DateTime.UtcNow,
                ProyectoId = 1,
                Proyecto = proyectoUno,
                Rol = RolProyecto.Manager,
                Activo = true,    
            };
            List<Usuario> listaUsuarios = [usuarioUno, usuarioDos, usuarioTres];

            List<Proyecto> listaProyectos = [proyectoUno, proyectoDos, proyectoTres];

            List<ProyectoUsuario> listaProyectoUsuarios = [
                proyectoUsuarioUno, proyectoUsuarioDos, proyectoUsuarioTres,
                proyectoUserUnoenTres, proyectoUserUnoenDos, proyectoUserDosenUno];

            await context.Usuarios.AddRangeAsync(listaUsuarios);
            await context.Proyectos.AddRangeAsync(listaProyectos);
            await context.ProyectoUsuario.AddRangeAsync(listaProyectoUsuarios);

            await context.SaveChangesAsync();

            // Act
            TestAuthenticationHandler.UserId = usuarioId;
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS: {response.StatusCode}");
            Console.WriteLine($"BODY: {body}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content
                .ReadFromJsonAsync<List<ProyectoReadDto>>();
            
            proyectos.Should().NotBeNull();
            proyectos!
                .Select(p => p.Id)
                .Should().BeEquivalentTo(proyectosEsperados);
        }
        
        [Fact]
        public async Task GetMisProyectosSinTenerNinguno_ReturnsOkEmpty()
        {
            await _factory.ResetDatabaseAsync();

            var context = _factory.GetDbContext();
            
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Nombre Uno",
                Apellidos = "Apellidos User",
                Email = "test1@testmail.com",
                PasswordHash = "!Password123!",
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                Proyectos = [],
                Comentarios = [],
                TareasAsignadas = [],
                TareasCreadas = [],  
            };

            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();

            // Act
            TestAuthenticationHandler.UserId = usuario.Id;
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");
            
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"STATUS: {response.StatusCode}");
            Console.WriteLine($"BODY: {body}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoReadDto>>();

            proyectos.Should().NotBeNull();
            proyectos.Should().BeEmpty();
        }
    }
}