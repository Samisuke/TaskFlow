using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Services;

public class EtiquetaServiceTests
{
    private readonly IEtiquetaRepository _repo = Substitute.For<IEtiquetaRepository>();
    private readonly EtiquetaService _sut;

    public EtiquetaServiceTests() => _sut = new EtiquetaService(_repo);

    [Fact]
    public async Task GetEtiquetaPorIdAsync_CuandoExiste_DebeDevolverla()
    {
        var etiqueta = new Etiqueta { Id = 1, Nombre = "Backend", Color = "#fff" };
        _repo.ObtenerUnaEtiquetaPorIdAsync(1).Returns(etiqueta);

        var result = await _sut.GetEtiquetaPorIdAsync(1);

        Assert.True(result.EsCorrecto);
        Assert.Same(etiqueta, result.Valor);
    }

    [Fact]
    public async Task GetEtiquetaPorIdAsync_CuandoNoExiste_DebeFallar()
    {
        _repo.ObtenerUnaEtiquetaPorIdAsync(99).Returns((Etiqueta?)null);

        var result = await _sut.GetEtiquetaPorIdAsync(99);

        Assert.False(result.EsCorrecto);
        Assert.Equal("La etiqueta no existe", result.MensajeError);
    }
}
