using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task GetTodosUsuariosAsync_CuandoHayUsuarios_DebeDevolverlos()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerTodosUsuariosAsync().Returns(new[] { new Usuario { Id = 1 } });
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.GetTodosUsuariosAsync();

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!);
    }

    [Fact]
    public async Task GetTodosUsuariosAsync_CuandoListaVacia_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerTodosUsuariosAsync().Returns(Array.Empty<Usuario>());
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.GetTodosUsuariosAsync();

        Assert.False(result.EsCorrecto);
        Assert.Equal("La lista está vacía.", result.MensajeError);
    }

    [Fact]
    public async Task GetUsuarioPorIdAsync_CuandoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorIdAsync(99).Returns((Usuario?)null);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.GetUsuarioPorIdAsync(99);

        Assert.False(result.EsCorrecto);
    }

    [Fact]
    public async Task GetUsuarioPorEmailAsync_CuandoExiste_DebeDevolverUsuario()
    {
        using var context = TestDbContextFactory.Create();
        var user = new Usuario { Id = 3, Email = "test@test.com" };
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorEmailAsync(user.Email).Returns(user);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.GetUsuarioPorEmailAsync(user.Email);

        Assert.True(result.EsCorrecto);
        Assert.Same(user, result.Valor);
    }

    [Fact]
    public async Task PostUsuarioAsync_DebeCrearUsuarioConPasswordHasheada()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IUsuarioRepository>();
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.PostUsuarioAsync("Sam", "Test", "sam@test.com", "secret");

        Assert.True(result.EsCorrecto);
        Assert.NotNull(result.Valor);
        Assert.Equal("Sam", result.Valor!.Nombre);
        Assert.Equal("sam@test.com", result.Valor.Email);
        Assert.NotEqual("secret", result.Valor.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("secret", result.Valor.PasswordHash));
        await repo.Received(1).CrearUnUsuarioNuevoAsync(Arg.Any<Usuario>());
    }

    [Fact]
    public async Task PatchUsuarioAsync_SiNoHayCambios_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var user = new Usuario { Id = 1, Email = "a@test.com" };
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns(user);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.PatchUsuarioAsync(1, null, null, null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se han detectado cambios.", result.MensajeError);
    }

    [Fact]
    public async Task PatchUsuarioAsync_SiEmailYaExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var user = new Usuario { Id = 1, Email = "a@test.com" };
        var other = new Usuario { Id = 2, Email = "b@test.com" };
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns(user);
        repo.ObtenerUsuarioPorEmailAsync("b@test.com").Returns(other);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.PatchUsuarioAsync(1, null, null, "b@test.com");

        Assert.False(result.EsCorrecto);
        Assert.Equal("El email ya está registrado, prueba otro.", result.MensajeError);
    }

    [Fact]
    public async Task PatchUsuarioAsync_ConCambios_DebeActualizarUsuario()
    {
        using var context = TestDbContextFactory.Create();
        var user = new Usuario { Id = 1, Nombre = "Old", Apellidos = "User", Email = "a@test.com" };
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns(user);
        repo.ObtenerUsuarioPorEmailAsync("new@test.com").Returns((Usuario?)null);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.PatchUsuarioAsync(1, "New", "Surname", "new@test.com");

        Assert.True(result.EsCorrecto);
        Assert.Equal("New", user.Nombre);
        Assert.Equal("Surname", user.Apellidos);
        Assert.Equal("new@test.com", user.Email);
    }

    [Fact]
    public async Task PatchUsuarioPassAsync_ConUsuarioInexistente_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns((Usuario?)null);
        var sut = new UsuarioService(repo, Substitute.For<TaskFlow.Core.Services.IPassPermissionService>(), context);

        var result = await sut.PatchUsuarioPassAsync(1, "new", "old");

        Assert.False(result.EsCorrecto);
        Assert.Equal("Usuario no encontrado.", result.MensajeError);
    }

    [Fact]
    public async Task PatchUsuarioPassAsync_SiPermisoRechaza_DebeFallarSinGuardar()
    {
        using var context = TestDbContextFactory.Create();
        var user = new Usuario { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("old") };
        var repo = Substitute.For<IUsuarioRepository>();
        var permission = Substitute.For<TaskFlow.Core.Services.IPassPermissionService>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns(user);
        permission.ComprobacionesPass(user, "wrong", "new").Returns(TaskFlow.Core.Common.Result.Mal("denied"));
        var sut = new UsuarioService(repo, permission, context);

        var result = await sut.PatchUsuarioPassAsync(1, "new", "wrong");

        Assert.False(result.EsCorrecto);
        Assert.Equal("denied", result.MensajeError);
        Assert.Equal(user.PasswordHash, user.PasswordHash);
    }

    [Fact]
    public async Task PatchUsuarioPassAsync_ConPermiso_DebeCambiarHash()
    {
        using var context = TestDbContextFactory.Create();
        var oldHash = BCrypt.Net.BCrypt.HashPassword("old");
        var user = new Usuario { Id = 1, PasswordHash = oldHash };
        var repo = Substitute.For<IUsuarioRepository>();
        var permission = Substitute.For<TaskFlow.Core.Services.IPassPermissionService>();
        repo.ObtenerUsuarioPorIdAsync(1).Returns(user);
        permission.ComprobacionesPass(user, "old", "new").Returns(TaskFlow.Core.Common.Result.Bien());
        var sut = new UsuarioService(repo, permission, context);

        var result = await sut.PatchUsuarioPassAsync(1, "new", "old");

        Assert.True(result.EsCorrecto);
        Assert.NotEqual(oldHash, user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("new", user.PasswordHash));
    }
}
