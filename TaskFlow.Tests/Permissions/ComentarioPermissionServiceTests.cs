using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Permissions;

public class ComentarioPermissionServiceTests
{
    private readonly IProyectoUsuarioRepository _repo = Substitute.For<IProyectoUsuarioRepository>();
    private readonly ComentarioPermissionService _sut;

    public ComentarioPermissionServiceTests() => _sut = new ComentarioPermissionService(_repo);

    [Fact]
    public async Task PuedeCambiarComentarioAsync_DebePermitirAlAutorActivoDelProyecto()
    {
        var comentario = new Comentario { UsuarioId = 7, Tarea = new Tarea { ProyectoId = 10 } };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(10, 7).Returns(new ProyectoUsuario { Activo = true });

        Assert.True(await _sut.PuedeCambiarComentarioAsync(7, comentario));
    }

    [Fact]
    public async Task PuedeCambiarComentarioAsync_DebeRechazarSiNoEsAutor()
    {
        var comentario = new Comentario { UsuarioId = 8, Tarea = new Tarea { ProyectoId = 10 } };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(10, 7).Returns(new ProyectoUsuario { Activo = true });

        Assert.False(await _sut.PuedeCambiarComentarioAsync(7, comentario));
    }

    [Fact]
    public async Task PuedeCambiarComentarioAsync_DebeRechazarSiMiembroInactivo()
    {
        var comentario = new Comentario { UsuarioId = 7, Tarea = new Tarea { ProyectoId = 10 } };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(10, 7).Returns(new ProyectoUsuario { Activo = false });

        Assert.False(await _sut.PuedeCambiarComentarioAsync(7, comentario));
    }
}
