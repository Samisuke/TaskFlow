using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Services;

public class ComentarioServiceTests
{
    [Fact]
    public async Task GetComentariosDeUnUsuarioAsync_CuandoNoHay_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();

        repo.ObtenerComentariosDeUnUsuarioAsync(1)
            .Returns(Array.Empty<Comentario>());

        var sut = CreateSut(
            context,
            repoComentario: repo);

        var result = await sut.GetComentariosDeUnUsuarioAsync(1);

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "El usuario no tiene comentarios.",
            result.MensajeError);
    }

    [Fact]
    public async Task GetComentariosDeUnaTareaAsync_CuandoHay_DebeDevolverlos()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();

        repo.ObtenerComentariosDeUnaTareaAsync(10)
            .Returns(new[]
            {
                new Comentario { Id = 1 }
            });

        var sut = CreateSut(
            context,
            repoComentario: repo);

        var result = await sut.GetComentariosDeUnaTareaAsync(10);

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!);
    }

    [Fact]
    public async Task GetComentarioPorIdAsync_CuandoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns((Comentario?)null);

        var sut = CreateSut(
            context,
            repoComentario: repo);

        var result = await sut.GetComentarioPorIdAsync(1);

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "Este comentario no existe.",
            result.MensajeError);
    }

    [Fact]
    public async Task PostComentarioAsync_SiTareaNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repoComentario = Substitute.For<IComentarioRepository>();
        var repoTarea = Substitute.For<ITareaRepository>();

        repoTarea.ObtenerTareaPorIdAsync(10)
            .Returns((Tarea?)null);

        var sut = CreateSut(
            context,
            repoComentario: repoComentario,
            repoTarea: repoTarea);

        var result = await sut.PostComentarioAsync(
            "hello",
            2,
            10);

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "La tarea a la que quieres añadir el comentario no existe.",
            result.MensajeError);
    }

    [Fact]
    public async Task PostComentarioAsync_SiNoEsMiembro_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repoComentario = Substitute.For<IComentarioRepository>();
        var repoTarea = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();

        var task = new Tarea
        {
            Id = 10,
            ProyectoId = 20
        };

        repoTarea.ObtenerTareaPorIdAsync(10)
            .Returns(task);

        permission.EsMiembroActivoAsync(20, 2)
            .Returns(false);

        var sut = CreateSut(
            context,
            repoComentario: repoComentario,
            repoTarea: repoTarea,
            proyectoPermission: permission);

        var result = await sut.PostComentarioAsync(
            "hello",
            2,
            10);

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "No puedes comentar en esta tarea.",
            result.MensajeError);

        await repoComentario
            .DidNotReceive()
            .CrearComentarioAsync(Arg.Any<Comentario>());
    }

    [Fact]
    public async Task PostComentarioAsync_ConPermiso_DebeCrearComentarioYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();

        var repoComentario = Substitute.For<IComentarioRepository>();
        var repoTarea = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<IProyectoPermissionService>();
        var history = Substitute.For<IHistorialService>();

        var task = new Tarea
        {
            Id = 10,
            ProyectoId = 20
        };

        repoTarea.ObtenerTareaPorIdAsync(10)
            .Returns(task);

        permission.EsMiembroActivoAsync(20, 2)
            .Returns(true);

        var sut = CreateSut(
            context,
            repoComentario: repoComentario,
            repoTarea: repoTarea,
            proyectoPermission: permission,
            historialService: history);

        var result = await sut.PostComentarioAsync(
            "hello",
            2,
            10);

        Assert.True(result.EsCorrecto);
        Assert.Equal("hello", result.Valor!.Contenido);
        Assert.Equal(2, result.Valor.UsuarioId);
        Assert.Equal(10, result.Valor.TareaId);

        await repoComentario
            .Received(1)
            .CrearComentarioAsync(Arg.Any<Comentario>());

        await history
            .Received(1)
            .RegistrarComentarioAsync(
                Arg.Any<Comentario>(),
                2);
    }

    [Fact]
    public async Task PatchComentarioAsync_SiNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns((Comentario?)null);

        var sut = CreateSut(
            context,
            repoComentario: repo);

        var result = await sut.PatchComentarioAsync(
            2,
            1,
            "new");

        Assert.False(result.EsCorrecto);
    }

    [Fact]
    public async Task PatchComentarioAsync_SiComentarioNoTieneTarea_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns(new Comentario
            {
                Id = 1
            });

        var sut = CreateSut(
            context,
            repoComentario: repo);

        var result = await sut.PatchComentarioAsync(
            2,
            1,
            "new");

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "No existe la tarea del comentario que quieres modificar.",
            result.MensajeError);
    }

    [Fact]
    public async Task PatchComentarioAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();
        var permission = Substitute.For<IComentarioPermissionService>();

        var comment = new Comentario
        {
            Id = 1,
            Tarea = new Tarea
            {
                ProyectoId = 10
            }
        };

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns(comment);

        permission
            .PuedeCambiarComentarioAsync(2, comment)
            .Returns(false);

        var sut = CreateSut(
            context,
            repoComentario: repo,
            comentarioPermission: permission);

        var result = await sut.PatchComentarioAsync(
            2,
            1,
            "new");

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "No puedes modificar este comentario.",
            result.MensajeError);
    }

    [Fact]
    public async Task PatchComentarioAsync_SinCambios_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();
        var permission = Substitute.For<IComentarioPermissionService>();

        var comment = new Comentario
        {
            Id = 1,
            Tarea = new Tarea
            {
                ProyectoId = 10
            }
        };

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns(comment);

        permission
            .PuedeCambiarComentarioAsync(2, comment)
            .Returns(true);

        var sut = CreateSut(
            context,
            repoComentario: repo,
            comentarioPermission: permission);

        var result = await sut.PatchComentarioAsync(
            2,
            1,
            null);

        Assert.False(result.EsCorrecto);
        Assert.Equal(
            "No se han detectado cambios.",
            result.MensajeError);
    }

    [Fact]
    public async Task PatchComentarioAsync_ConCambio_DebeActualizarYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();

        var repo = Substitute.For<IComentarioRepository>();
        var permission = Substitute.For<IComentarioPermissionService>();
        var history = Substitute.For<IHistorialService>();

        var comment = new Comentario
        {
            Id = 1,
            Contenido = "old",
            Tarea = new Tarea
            {
                ProyectoId = 10
            }
        };

        repo.ObtenerComentarioPorIdAsync(1)
            .Returns(comment);

        permission
            .PuedeCambiarComentarioAsync(2, comment)
            .Returns(true);

        var sut = CreateSut(
            context,
            repoComentario: repo,
            comentarioPermission: permission,
            historialService: history);

        var result = await sut.PatchComentarioAsync(
            2,
            1,
            "new");

        Assert.True(result.EsCorrecto);
        Assert.Equal("new", comment.Contenido);

        await history
            .Received(1)
            .ModificarComentarioAsync(
                comment,
                2);
    }

    private static ComentarioService CreateSut(
        TaskFlowDbContext context,
        IComentarioRepository? repoComentario = null,
        ITareaRepository? repoTarea = null,
        IProyectoPermissionService? proyectoPermission = null,
        IComentarioPermissionService? comentarioPermission = null,
        IHistorialService? historialService = null)
    {
        return new ComentarioService(
            repoComentario ?? Substitute.For<IComentarioRepository>(),
            repoTarea ?? Substitute.For<ITareaRepository>(),
            proyectoPermission ?? Substitute.For<IProyectoPermissionService>(),
            comentarioPermission ?? Substitute.For<IComentarioPermissionService>(),
            historialService ?? Substitute.For<IHistorialService>(),
            context
        );
    }
}