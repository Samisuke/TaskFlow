using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Services;

public class ProyectoServiceTests
{
    [Fact]
    public async Task GetProyectoPorIdAsync_CuandoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        repo.ObtenerProyectoPorIdAsync(1).Returns((Proyecto?)null);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetProyectoPorIdAsync(1);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No existe el proyecto,", result.MensajeError);
    }

    [Fact]
    public async Task GetProyectosDeUnaPersonaAsync_CuandoNoTieneProyectos_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        repo.ObtenerProyectosDeUnUsuarioAsync(2).Returns(Array.Empty<Proyecto>());
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetProyectosDeUnaPersonaAsync(2);

        Assert.False(result.EsCorrecto);
        Assert.Equal("Este usuario no pertenece a ningún proyecto aun.", result.MensajeError);
    }

    [Fact]
    public async Task GetProyectosDeUnCreadorAsync_CuandoHayProyectos_DebeDevolverlos()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        repo.ObtenerProyectosDeUnCreadorAsync(2).Returns(new[] { new Proyecto { Id = 1 } });
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetProyectosDeUnCreadorAsync(2);

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!);
    }

    [Fact]
    public async Task PostProyectoAsync_SiNoPuedeRecuperarElProyectoCreado_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var projectUserRepo = Substitute.For<IProyectoUsuarioRepository>();
        repo.ObtenerProyectoPorIdAsync(0).Returns((Proyecto?)null);
        var sut = CreateSut(context, repo: repo, projectUserRepo: projectUserRepo);

        var result = await sut.PostProyectoAsync("P", "D", 2);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se ha podido recuperar el proyecto creado.", result.MensajeError);
        await repo.Received(1).CrearProyectoAsync(Arg.Any<Proyecto>());
        await projectUserRepo.Received(1).CrearUsuarioAsync(Arg.Is<ProyectoUsuario>(x => x.UsuarioId == 2 && x.Rol == RolProyecto.Manager && x.Activo));
    }

    [Fact]
    public async Task PatchProyectoAsync_SinProyecto_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        repo.ObtenerProyectoPorIdAsync(1).Returns((Proyecto?)null);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.PatchProyectoAsync(2, 1, "new", null);

        Assert.False(result.EsCorrecto);
    }

    [Fact]
    public async Task PatchProyectoAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var project = new Proyecto { Id = 1, Nombre = "old" };
        repo.ObtenerProyectoPorIdAsync(1).Returns(project);
        permission.PuedeModificarProyectoAsync(1, 2).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchProyectoAsync(2, 1, "new", null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("no puedes modificar el proyecto.", result.MensajeError);
    }

    [Fact]
    public async Task PatchProyectoAsync_SinCambios_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var project = new Proyecto { Id = 1, Nombre = "old" };
        repo.ObtenerProyectoPorIdAsync(1).Returns(project);
        permission.PuedeModificarProyectoAsync(1, 2).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchProyectoAsync(2, 1, null, null);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se han detectado cambios.", result.MensajeError);
    }

    [Fact]
    public async Task PatchProyectoAsync_ConCambios_DebeActualizarYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var history = Substitute.For<IHistorialService>();
        var project = new Proyecto { Id = 1, Nombre = "old", Descripcion = "old desc" };
        repo.ObtenerProyectoPorIdAsync(1).Returns(project);
        permission.PuedeModificarProyectoAsync(1, 2).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission, history: history);

        var result = await sut.PatchProyectoAsync(2, 1, "new", "new desc");

        Assert.True(result.EsCorrecto);
        Assert.Equal("new", project.Nombre);
        Assert.Equal("new desc", project.Descripcion);
        await history.Received(1).ModificarProyectoAsync(project, 2);
    }

    [Fact]
    public async Task PatchDueñoProyectoAsync_SiUsuarioNuevoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var userRepo = Substitute.For<IUsuarioRepository>();
        repo.ObtenerProyectoPorIdAsync(1).Returns(new Proyecto { Id = 1, PropietarioId = 2 });
        userRepo.ObtenerUsuarioPorIdAsync(3).Returns((Usuario?)null);
        var sut = CreateSut(context, repo: repo, userRepo: userRepo);

        var result = await sut.PatchDueñoProyectoAsync(2, 1, 3);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se encuentra la persona a la que quieres transferir el proyecto.", result.MensajeError);
    }

    [Fact]
    public async Task PatchDueñoProyectoAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<IProyectoRepository>();
        var userRepo = Substitute.For<IUsuarioRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var project = new Proyecto { Id = 1, PropietarioId = 2 };
        var newOwner = new Usuario { Id = 3, Activo = true };
        repo.ObtenerProyectoPorIdAsync(1).Returns(project);
        userRepo.ObtenerUsuarioPorIdAsync(3).Returns(newOwner);
        permission.PuedesTransferirProyectoAsync(project, newOwner, 2).Returns(false);
        var sut = CreateSut(context, repo: repo, userRepo: userRepo, permission: permission);

        var result = await sut.PatchDueñoProyectoAsync(2, 1, 3);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No puedes transferir el proyecto a esta persona.", result.MensajeError);
    }

    private static ProyectoService CreateSut(
        TaskFlow.Infrastructure.Data.TaskFlowDbContext context,
        IProyectoRepository? repo = null,
        IUsuarioRepository? userRepo = null,
        IProyectoPermissionService? permission = null,
        IHistorialService? history = null,
        IProyectoUsuarioRepository? projectUserRepo = null)
        => new(
            repo ?? Substitute.For<IProyectoRepository>(),
            userRepo ?? Substitute.For<IUsuarioRepository>(),
            permission ?? Substitute.For<IProyectoPermissionService>(),
            history ?? Substitute.For<IHistorialService>(),
            context,
            projectUserRepo ?? Substitute.For<IProyectoUsuarioRepository>());
}
