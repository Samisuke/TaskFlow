using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Services;

public class HistorialServiceTests
{
    private readonly IHistorialRepository _repo = Substitute.For<IHistorialRepository>();
    private readonly HistorialService _sut;

    public HistorialServiceTests() => _sut = new HistorialService(_repo);

    [Fact]
    public async Task GetHistorialAsync_CuandoHayRegistros_DebeDevolverlos()
    {
        var items = new[] { new Historial { Id = 1, ProyectoId = 10, UsuarioId = 2, Accion = "x" } };
        _repo.ObtenerHistorialAsync(10).Returns(items);

        var result = await _sut.GetHistorialAsync(10);

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!);
    }

    [Fact]
    public async Task GetHistorialAsync_CuandoNoHayRegistros_DebeFallar()
    {
        _repo.ObtenerHistorialAsync(10).Returns(Array.Empty<Historial>());

        var result = await _sut.GetHistorialAsync(10);

        Assert.False(result.EsCorrecto);
        Assert.Equal("La tarea no tiene cambios realizados.", result.MensajeError);
    }

    [Fact]
    public async Task RegistrarTareaAsync_DebeCrearHistorialConProyectoYCreador()
    {
        var tarea = new Tarea { ProyectoId = 20, CreadorId = 7 };

        await _sut.RegistrarTareaAsync(tarea);

        await _repo.Received(1).CrearHistorialAsync(Arg.Is<Historial>(h =>
            h.ProyectoId == 20 && h.UsuarioId == 7 && !string.IsNullOrWhiteSpace(h.Accion)));
    }

    [Fact]
    public async Task ModificarTareaAsync_DebeCrearHistorialConUsuarioQueModifica()
    {
        var tarea = new Tarea { ProyectoId = 20 };

        await _sut.ModificarTareaAsync(tarea, 9);

        await _repo.Received(1).CrearHistorialAsync(Arg.Is<Historial>(h =>
            h.ProyectoId == 20 && h.UsuarioId == 9));
    }

    [Fact]
    public async Task RegistrarComentarioAsync_DebeUsarProyectoDeLaTarea()
    {
        var comentario = new Comentario
        {
            UsuarioId = 3,
            Tarea = new Tarea { ProyectoId = 33 }
        };

        await _sut.RegistrarComentarioAsync(comentario, 3);

        await _repo.Received(1).CrearHistorialAsync(Arg.Is<Historial>(h => h.ProyectoId == 33 && h.UsuarioId == 3));
    }
}
