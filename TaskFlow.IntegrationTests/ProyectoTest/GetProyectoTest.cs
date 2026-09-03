using System.Net;
using FluentAssertions;
using TaskFlow.Core.Models;
using TaskFlow.Core.Dto.Proyecto;
using System.Net.Http.Json;
using TaskFlow.IntegrationTests.Resources;

// <summary>
// Pruebas de integración de los endpoints de consulta de proyectos.

// Estas pruebas verifican el flujo completo de una petición HTTP a través
// de la aplicación, incluyendo autenticación, lógica de negocio, acceso
// a datos mediante Entity Framework Core y persistencia en PostgreSQL.

// También se comprueban las reglas de autorización relacionadas con la
// pertenencia de los usuarios a los proyectos.
// </summary>

namespace TaskFlow.IntegrationTests.ProyectoTest
{
    public class GetProyectoTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        public GetProyectoTest(
            CustomWebApplicationFactory factory)
        {   
            _client = factory.CreateClient();
            _factory = factory;
        } 

        // Comprueba que un usuario autenticado recibe los proyectos a los que pertenece.
        [Fact]
        public async Task GetMisProyectos_CuandoElUsuarioTieneProyectos_ReturnsOk()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            var usuario = ProyectoResources.CrearUsuario(1);
            var proyecto = ProyectoResources.CrearProyecto(1);
            var proyectoUsuario = ProyectoResources.CrearProyectoUsuario(usuario, proyecto);

            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddAsync(usuario);
                await context.Proyectos.AddAsync(proyecto);
                await context.ProyectoUsuario.AddAsync(proyectoUsuario);
                await context.SaveChangesAsync();
            });

            // Act
            TestAuthenticationHandler.UserId = 1;
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoReadDto>>();

            proyectos.Should().NotBeNull();
            proyectos.Should().ContainSingle();
            
            proyectos![0].Nombre.Should().Be("Test 1");
            proyectos[0].Descripcion.Should().Be("Test Desc.");
        }

        // Comprueba que la API devuelve únicamente los proyectos asociados al usuario autenticado.
        // Se utilizan diferentes combinaciones de usuarios y proyectos mediante un Theory
        // para verificar el filtrado según la pertenencia de cada usuario.
        [Theory]
        [InlineData(1, new[] {1, 2, 3})]
        [InlineData(2, new[] {1, 2})]
        [InlineData(3, new [] {3})]
        public async Task GetMisProyectos_DevuelveSoloLosProyectosDelUsuarioAutenticado(int usuarioId, int[] proyectosEsperados)
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            var usuarioUno = ProyectoResources.CrearUsuario(1);
            var usuarioDos = ProyectoResources.CrearUsuario(2);
            var usuarioTres = ProyectoResources.CrearUsuario(3);

            var proyectoUno = ProyectoResources.CrearProyecto(1);
            var proyectoDos = ProyectoResources.CrearProyecto(2);
            var proyectoTres = ProyectoResources.CrearProyecto(3);

            var proyectoUsuarioUno = ProyectoResources.CrearProyectoUsuario(usuarioUno, proyectoUno);
            var proyectoUsuarioDos = ProyectoResources.CrearProyectoUsuario(usuarioDos, proyectoDos);
            var proyectoUsuarioTres = ProyectoResources.CrearProyectoUsuario(usuarioTres, proyectoTres);
            var proyectoUserUnoenTres = ProyectoResources.CrearProyectoUsuario(usuarioUno, proyectoTres);
            var proyectoUserUnoenDos = ProyectoResources.CrearProyectoUsuario(usuarioUno, proyectoDos);
            var proyectoUserDosenUno = ProyectoResources.CrearProyectoUsuario(usuarioDos, proyectoUno);

            List<Usuario> listaUsuarios = [usuarioUno, usuarioDos, usuarioTres];
            List<Proyecto> listaProyectos = [proyectoUno, proyectoDos, proyectoTres];
            List<ProyectoUsuario> listaProyectoUsuarios = [
                proyectoUsuarioUno, proyectoUsuarioDos, proyectoUsuarioTres,
                proyectoUserUnoenTres, proyectoUserUnoenDos, proyectoUserDosenUno];

            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddRangeAsync(listaUsuarios);
                await context.Proyectos.AddRangeAsync(listaProyectos);
                await context.ProyectoUsuario.AddRangeAsync(listaProyectoUsuarios);
                await context.SaveChangesAsync();
            });

            // Act
            TestAuthenticationHandler.UserId = usuarioId;
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content
                .ReadFromJsonAsync<List<ProyectoReadDto>>();
            
            proyectos.Should().NotBeNull();
            proyectos!
                .Select(p => p.Id)
                .Should().BeEquivalentTo(proyectosEsperados);
        }
        
        // Comprueba que un usuario autenticado que no pertenece a ningún proyecto
        // recibe correctamente una colección vacía.
        [Fact]
        public async Task GetMisProyectos_CuandoElUsuarioNoTieneProyectos_ReturnsOkEmpty()
        {
            await _factory.ResetDatabaseAsync();
            
            // Arrange
            var usuario = ProyectoResources.CrearUsuario(1);

            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddAsync(usuario);
                await context.SaveChangesAsync();
            });

            // Act
            TestAuthenticationHandler.UserId = usuario.Id;
            var response = await _client.GetAsync("/api/proyecto/mis-proyectos");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoReadDto>>();

            proyectos.Should().NotBeNull();
            proyectos.Should().BeEmpty();
        }

        // Comprueba que un usuario autenticado no puede acceder a un proyecto
        // al que no pertenece y recibe una respuesta 403 Forbidden.
        // No se incluye un test específico para 404 en este endpoint.
        // Por regla de negocio, un usuario solo puede acceder a proyectos a los que pertenece.
        // Por tanto, un proyecto inexistente no puede cumplir la condición de pertenencia
        // y la API lo trata como un acceso prohibido (403).
        [Fact]
        public async Task GetProyecto_CuandoElUsuarioNoPertenece_ReturnsForbidden()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            var usuarioUno = ProyectoResources.CrearUsuario(1);
            var usuarioDos = ProyectoResources.CrearUsuario(2);
            var proyecto = ProyectoResources.CrearProyecto(1);
            var proyectoUser = ProyectoResources.CrearProyectoUsuario(usuarioUno, proyecto);

            List<Usuario> listaUsuario = [usuarioUno, usuarioDos];
            await _factory.ExecuteDbContextAsync(async context =>
            {
                await context.Usuarios.AddRangeAsync(listaUsuario);
                await context.Proyectos.AddAsync(proyecto);
                await context.ProyectoUsuario.AddAsync(proyectoUser);
                await context.SaveChangesAsync();
            });

            // Act
            TestAuthenticationHandler.UserId = 2;
            var response = await _client.GetAsync("/api/proyecto/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}