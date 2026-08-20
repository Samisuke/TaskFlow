using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Permissions;

public class TareaPermissionServiceTests
{
    private readonly IProyectoUsuarioRepository _repo = Substitute.For<IProyectoUsuarioRepository>();
    private readonly ITareaRepository _taskRepo = Substitute.For<ITareaRepository>();
    private readonly TareaPermissionService _sut;

    public TareaPermissionServiceTests() => _sut = new TareaPermissionService(_repo, _taskRepo);

    [Fact]
    public async Task PuedePublicarTareasAsync_DebePermitirManager()
    {
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Manager });
        Assert.True(await _sut.PuedePublicarTareasAsync(1, 2));
    }

    [Fact]
    public async Task PuedePublicarTareasAsync_DebePermitirAdministrador()
    {
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Administrador });
        Assert.True(await _sut.PuedePublicarTareasAsync(1, 2));
    }

    [Fact]
    public async Task PuedePublicarTareasAsync_DebeRechazarMiembroNormal()
    {
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Miembro });
        Assert.False(await _sut.PuedePublicarTareasAsync(1, 2));
    }

    [Fact]
    public async Task PuedeModificarTareasAsync_DebePermitirAlAsignado()
    {
        var task = new Tarea { ProyectoId = 1, AsignadoId = 2, CreadorId = 8 };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Miembro });
        Assert.True(await _sut.PuedeModificarTareasAsync(2, task));
    }

    [Fact]
    public async Task PuedeModificarTareasAsync_DebePermitirAlCreador()
    {
        var task = new Tarea { ProyectoId = 1, AsignadoId = 8, CreadorId = 2 };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Miembro });
        Assert.True(await _sut.PuedeModificarTareasAsync(2, task));
    }

    [Fact]
    public async Task PuedeModificarTareasAsync_DebeRechazarUsuarioSinPermiso()
    {
        var task = new Tarea { ProyectoId = 1, AsignadoId = 8, CreadorId = 9 };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Miembro });
        Assert.False(await _sut.PuedeModificarTareasAsync(2, task));
    }

    [Fact]
    public async Task PuedeModificarEstadoTareaAsync_AdministradorNoPuedeSegunReglaActual()
    {
        var task = new Tarea { ProyectoId = 1, AsignadoId = 8, CreadorId = 9 };
        _repo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Administrador });
        Assert.False(await _sut.PuedeModificarEstadoTareaAsync(2, task));
    }
}
