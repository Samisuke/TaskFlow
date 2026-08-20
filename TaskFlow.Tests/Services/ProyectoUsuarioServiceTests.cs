using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Services;

public class ProyectoUsuarioServiceTests
{
    [Fact]
    public async Task GetTodosLosUsuariosDeUnProyectoAsync_CuandoNoHay_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        repo.ObtenerTodosUsuariosDeUnProyectoAsync(1).Returns(Array.Empty<ProyectoUsuario>());
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetTodosLosUsuariosDeUnProyectoAsync(1);

        Assert.False(result.EsCorrecto);
        Assert.Equal("El Proyecto aun no tiene usuarios.", result.MensajeError);
    }

    [Fact]
    public async Task GetUsuarioDeUnProyectoAsync_CuandoExiste_DebeDevolverlo()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        var item = new ProyectoUsuario { UsuarioId = 2, ProyectoId = 1 };
        repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(item);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetUsuarioDeUnProyectoAsync(1, 2);

        Assert.True(result.EsCorrecto);
        Assert.Same(item, result.Valor);
    }

    [Fact]
    public async Task PostUsuarioAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        permission.PuedeAñadirPersonasAsync(1, 3, 2).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PostUsuarioAsync(2, 3, 1, RolProyecto.Miembro);

        Assert.False(result.EsCorrecto);
        await repo.DidNotReceive().CrearUsuarioAsync(Arg.Any<ProyectoUsuario>());
    }

    [Fact]
    public async Task PostUsuarioAsync_ConPermiso_DebeCrearUsuarioActivoConRol()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var history = Substitute.For<IHistorialService>();
        permission.PuedeAñadirPersonasAsync(1, 3, 2).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission, history: history);

        var result = await sut.PostUsuarioAsync(2, 3, 1, RolProyecto.Administrador);

        Assert.True(result.EsCorrecto);
        Assert.Equal(3, result.Valor!.UsuarioId);
        Assert.Equal(RolProyecto.Administrador, result.Valor.Rol);
        Assert.True(result.Valor.Activo);
        await repo.Received(1).CrearUsuarioAsync(Arg.Any<ProyectoUsuario>());
        await history.Received(1).AñadirPersonaProyectoAsync(1, 2);
    }

    [Fact]
    public async Task PatchUsuarioAsync_SiNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns((ProyectoUsuario?)null);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.PatchUsuarioAsync(2, 3, 1, true, null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se encuentra el usuario.", result.MensajeError);
    }

    [Fact]
    public async Task PatchUsuarioAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var item = new ProyectoUsuario { UsuarioId = 3, ProyectoId = 1, Activo = true };
        repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns(item);
        permission.PuedeModificarProyectoAsync(1, 2).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchUsuarioAsync(2, 3, 1, false, null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No puedes modificar los usuarios de este proyecto.", result.MensajeError);
    }

    [Fact]
    public async Task PatchUsuarioAsync_SinCambios_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoUsuarioRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var item = new ProyectoUsuario { UsuarioId = 3, ProyectoId = 1, Activo = true };
        repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns(item);
        permission.PuedeModificarProyectoAsync(1, 2).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchUsuarioAsync(2, 3, 1, null, null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se encontraron cambios", result.MensajeError);
    }

    private static ProyectoUsuarioService CreateSut(
        TaskFlow.Infrastructure.Data.TaskFlowDbContext context,
        IProyectoUsuarioRepository? repo = null,
        IProyectoPermissionService? permission = null,
        IHistorialService? history = null)
        => new(
            repo ?? Substitute.For<IProyectoUsuarioRepository>(),
            permission ?? Substitute.For<IProyectoPermissionService>(),
            history ?? Substitute.For<IHistorialService>(),
            context);
}
