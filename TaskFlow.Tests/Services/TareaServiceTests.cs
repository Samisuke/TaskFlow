using TaskFlow.Core.Common;
using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Requests;
using TaskFlow.Core.Services;
using TaskFlow.Infrastructure.Services;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Services;

public class TareaServiceTests
{
    [Fact]
    public async Task GetTareasPendientesDeUsuarioAsync_CuandoNoHay_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        repo.ObtenerTareasPendientesPorUsuarioIdAsync(1, 1, 5).Returns((Array.Empty<Tarea>(), 10));
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetTareasPendientesDeUsuarioAsync(1, 1, 5);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No tienes tareas pendientes.", result.MensajeError);
    }

    [Fact]
    public async Task GetTareasDadasDeUsuarioAsync_CuandoHay_DebeDevolverlas()
    {
        using var context = TestDbContextFactory.Create();
        var tasks = new[] { new Tarea { Id = 1 } };
        var repo = Substitute.For<ITareaRepository>();
        repo.ObtenerTareasDadasPorUsuarioIdAsync(2, 1, 5).Returns((tasks, 1));
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetTareasDadasDeUsuarioAsync(2, 1, 5);

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!.Items);
    }

    [Fact]
    public async Task GetTareaPorIdAsync_CuandoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        repo.ObtenerTareaPorIdAsync(99).Returns((Tarea?)null);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.GetTareaPorIdAsync(99);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se ha encontrado una tarea.", result.MensajeError);
    }

    [Fact]
    public async Task PostTareaAsync_SiNoTienePermiso_DebeFallarYSinCrear()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        permission.PuedePublicarTareasAsync(10, 2).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PostTareaAsync(2, "T", "D", EstadoTarea.Pendiente, PrioridadTarea.Media,
            DateTimeOffset.UtcNow.AddDays(1), 10, 3, []);

        Assert.False(result.EsCorrecto);
        await repo.DidNotReceive().CrearTareaAsync(Arg.Any<Tarea>());
    }

    [Fact]
    public async Task PostTareaAsync_SinEtiquetas_DebeCrearTareaYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var history = Substitute.For<IHistorialService>();
        permission.PuedePublicarTareasAsync(10, 2).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission, history: history);

        var result = await sut.PostTareaAsync(2, "T", "D", EstadoTarea.Pendiente, PrioridadTarea.Alta,
            DateTimeOffset.UtcNow.AddDays(1), 10, 3, []);

        Assert.True(result.EsCorrecto);
        Assert.Equal("T", result.Valor!.Titulo);
        Assert.Equal(10, result.Valor.ProyectoId);
        Assert.Equal(2, result.Valor.CreadorId);
        await repo.Received(1).CrearTareaAsync(Arg.Any<Tarea>());
        await history.Received(1).RegistrarTareaAsync(Arg.Any<Tarea>());
    }

    [Fact]
    public async Task PostTareaAsync_SiCreacionDeEtiquetasFalla_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var tags = Substitute.For<ITareaEtiquetaService>();
        permission.PuedePublicarTareasAsync(10, 2).Returns(true);
        tags.ComprobarSiEtiquetaExisteOCrearASync(Arg.Any<IEnumerable<NuevaEtiqueta>>())
            .Returns(Result<List<Etiqueta>>.Mal("tag error"));
        var sut = CreateSut(context, repo: repo, permission: permission, tags: tags);

        var result = await sut.PostTareaAsync(2, "T", "D", EstadoTarea.Pendiente, PrioridadTarea.Alta,
            DateTimeOffset.UtcNow.AddDays(1), 10, 3,
            [new NuevaEtiqueta { Nombre = "api", Color = "blue" }]);

        Assert.False(result.EsCorrecto);
        Assert.Equal("tag error", result.MensajeError);
    }

    [Fact]
    public async Task PatchTareaAsync_CuandoNoExiste_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        repo.ObtenerTareaPorIdAsync(1).Returns((Tarea?)null);
        var sut = CreateSut(context, repo: repo);

        var result = await sut.PatchTareaAsync(2, 1, "x", null, null, null, []);

        Assert.False(result.EsCorrecto);
    }

    [Fact]
    public async Task PatchTareaAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var task = new Tarea { Id = 1, ProyectoId = 10 };
        repo.ObtenerTareaPorIdAsync(1).Returns(task);
        permission.PuedeModificarTareasAsync(2, task).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchTareaAsync(2, 1, "x", null, null, null, []);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No puedes modificar la tarea.", result.MensajeError);
    }

    [Fact]
    public async Task PatchTareaAsync_SinCambios_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var task = new Tarea { Id = 1, ProyectoId = 10 };
        repo.ObtenerTareaPorIdAsync(1).Returns(task);
        permission.PuedeModificarTareasAsync(2, task).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchTareaAsync(2, 1, null, null, null, null, []);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No se han detectado cambios.", result.MensajeError);
    }

    [Fact]
    public async Task PatchTareaAsync_ConCambios_DebeActualizarYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var history = Substitute.For<IHistorialService>();
        var task = new Tarea { Id = 1, ProyectoId = 10, Titulo = "old", Prioridad = PrioridadTarea.Baja };
        repo.ObtenerTareaPorIdAsync(1).Returns(task);
        permission.PuedeModificarTareasAsync(2, task).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission, history: history);

        var result = await sut.PatchTareaAsync(2, 1, "new", null, PrioridadTarea.Critica, null, []);

        Assert.True(result.EsCorrecto);
        Assert.Equal("new", task.Titulo);
        Assert.Equal(PrioridadTarea.Critica, task.Prioridad);
        await history.Received(1).ModificarTareaAsync(task, 2);
    }

    [Fact]
    public async Task PatchEstadoTareaAsync_SinPermiso_DebeFallar()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var task = new Tarea { Id = 1, ProyectoId = 10, Estado = EstadoTarea.Pendiente };
        repo.ObtenerTareaPorIdAsync(1).Returns(task);
        permission.PuedeModificarEstadoTareaAsync(2, task).Returns(false);
        var sut = CreateSut(context, repo: repo, permission: permission);

        var result = await sut.PatchEstadoTareaAsync(2, 1, EstadoTarea.Completada);

        Assert.False(result.EsCorrecto);
        Assert.Equal("No puedes modificar el estado de una tarea.", result.MensajeError);
    }

    [Fact]
    public async Task PatchEstadoTareaAsync_ConPermiso_DebeCambiarEstadoYRegistrarHistorial()
    {
        using var context = TestDbContextFactory.Create();
        var repo = Substitute.For<ITareaRepository>();
        var permission = Substitute.For<ITareaPermissionService>();
        var history = Substitute.For<IHistorialService>();
        var task = new Tarea { Id = 1, ProyectoId = 10, Estado = EstadoTarea.Pendiente };
        repo.ObtenerTareaPorIdAsync(1).Returns(task);
        permission.PuedeModificarEstadoTareaAsync(2, task).Returns(true);
        var sut = CreateSut(context, repo: repo, permission: permission, history: history);

        var result = await sut.PatchEstadoTareaAsync(2, 1, EstadoTarea.Completada);

        Assert.True(result.EsCorrecto);
        Assert.Equal(EstadoTarea.Completada, task.Estado);
        await history.Received(1).ModificarEstadoTareaAsync(task, 2);
    }

    private static TareaService CreateSut(
        TaskFlow.Infrastructure.Data.TaskFlowDbContext context,
        ITareaRepository? repo = null,
        ITareaEtiquetaService? tags = null,
        ITareaPermissionService? permission = null,
        IHistorialService? history = null)
        => new(
            repo ?? Substitute.For<ITareaRepository>(),
            tags ?? Substitute.For<ITareaEtiquetaService>(),
            permission ?? Substitute.For<ITareaPermissionService>(),
            history ?? Substitute.For<IHistorialService>(),
            context);
}
