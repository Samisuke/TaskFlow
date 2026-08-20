using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Requests;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Services;

public class TareaEtiquetaServiceTests
{
    private readonly ITareaEtiquetaRepository _relationRepo = Substitute.For<ITareaEtiquetaRepository>();
    private readonly IEtiquetaRepository _tagRepo = Substitute.For<IEtiquetaRepository>();
    private readonly TareaEtiquetaService _sut;

    public TareaEtiquetaServiceTests() => _sut = new TareaEtiquetaService(_relationRepo, _tagRepo);

    [Fact]
    public async Task GetEtiquetasDeUnaTareaAsync_CuandoNoHayEtiquetas_DebeFallar()
    {
        _relationRepo.ObtenerEtiquetasDeUnaTareaAsync(1).Returns(Array.Empty<TareaEtiqueta>());

        var result = await _sut.GetEtiquetasDeUnaTareaAsync(1);

        Assert.False(result.EsCorrecto);
        Assert.Equal("La tarea no tiene etiquetas.", result.MensajeError);
    }

    [Fact]
    public async Task GetEtiquetasDeUnaTareaAsync_CuandoHayEtiquetas_DebeDevolverlas()
    {
        var relations = new[] { new TareaEtiqueta { TareaId = 1, EtiquetaId = 2 } };
        _relationRepo.ObtenerEtiquetasDeUnaTareaAsync(1).Returns(relations);

        var result = await _sut.GetEtiquetasDeUnaTareaAsync(1);

        Assert.True(result.EsCorrecto);
        Assert.Single(result.Valor!);
    }

    [Fact]
    public async Task ComprobarSiEtiquetaExisteOCrearASync_SiExiste_NoDebeCrearOtra()
    {
        var existing = new Etiqueta { Id = 5, Nombre = "api", Color = "red" };
        _tagRepo.ObtenerEtiquetaPorNombreYColorAsync("api", "red").Returns(existing);

        var result = await _sut.ComprobarSiEtiquetaExisteOCrearASync(new[]
        {
            new NuevaEtiqueta { Nombre = "api", Color = "red" }
        });

        Assert.True(result.EsCorrecto);
        Assert.Same(existing, result.Valor![0]);
        await _tagRepo.DidNotReceive().CrearEtiquetaAsync(Arg.Any<Etiqueta>());
    }

    [Fact]
    public async Task ComprobarSiEtiquetaExisteOCrearASync_SiNoExiste_DebeCrearYDevolverla()
    {
        _tagRepo.ObtenerEtiquetaPorNombreYColorAsync("api", "blue").Returns((Etiqueta?)null);

        var result = await _sut.ComprobarSiEtiquetaExisteOCrearASync(new[]
        {
            new NuevaEtiqueta { Nombre = "api", Color = "blue" }
        });

        Assert.True(result.EsCorrecto);
        Assert.Equal("api", result.Valor![0].Nombre);
        Assert.Equal("blue", result.Valor[0].Color);
        await _tagRepo.Received(1).CrearEtiquetaAsync(Arg.Is<Etiqueta>(e => e.Nombre == "api" && e.Color == "blue"));
    }

    [Fact]
    public async Task AsignarEtiquetaATareaASync_DebeCrearUnaRelacionPorEtiqueta()
    {
        var tarea = new Tarea { Id = 10 };
        var etiquetas = new List<Etiqueta>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };

        var result = await _sut.AsignarEtiquetaATareaASync(tarea, etiquetas);

        Assert.True(result.EsCorrecto);
        await _relationRepo.Received(2).CrearTareaEtiquetaAsync(Arg.Any<TareaEtiqueta>());
        await _relationRepo.Received(1).CrearTareaEtiquetaAsync(Arg.Is<TareaEtiqueta>(x => x.Tarea == tarea && x.Etiqueta!.Id == 1));
        await _relationRepo.Received(1).CrearTareaEtiquetaAsync(Arg.Is<TareaEtiqueta>(x => x.Tarea == tarea && x.Etiqueta!.Id == 2));
    }
}
