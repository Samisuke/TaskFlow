using TaskFlow.Core.Enums;
using TaskFlow.Core.Models;
using TaskFlow.Core.Repositories;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Permissions;

public class ProyectoPermissionServiceTests
{
    private readonly IProyectoUsuarioRepository _projectUserRepo = Substitute.For<IProyectoUsuarioRepository>();
    private readonly IUsuarioRepository _userRepo = Substitute.For<IUsuarioRepository>();
    private readonly ProyectoPermissionService _sut;

    public ProyectoPermissionServiceTests() => _sut = new ProyectoPermissionService(_projectUserRepo, _userRepo);

    [Fact]
    public async Task EsMiembroActivoAsync_DebePermitirAMiembroActivo()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true });
        Assert.True(await _sut.EsMiembroActivoAsync(1, 2));
    }

    [Fact]
    public async Task EsMiembroActivoAsync_DebeRechazarMiembroInactivo()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = false });
        Assert.False(await _sut.EsMiembroActivoAsync(1, 2));
    }

    [Fact]
    public async Task PuedeModificarProyectoAsync_SoloManagerPuedeModificar()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Manager });
        Assert.True(await _sut.PuedeModificarProyectoAsync(1, 2));
    }

    [Fact]
    public async Task PuedeModificarProyectoAsync_AdministradorNoPuedeModificarSegunReglaActual()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Administrador });
        Assert.False(await _sut.PuedeModificarProyectoAsync(1, 2));
    }

    [Fact]
    public async Task PuedesTransferirProyectoAsync_DebePermitirAlDueñoHaciaMiembroActivo()
    {
        var project = new Proyecto { Id = 1, PropietarioId = 2 };
        var newOwner = new Usuario { Id = 3, Activo = true };
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns(new ProyectoUsuario { Activo = true });

        Assert.True(await _sut.PuedesTransferirProyectoAsync(project, newOwner, 2));
    }

    [Fact]
    public async Task PuedesTransferirProyectoAsync_DebeRechazarSiNoEresDueño()
    {
        var project = new Proyecto { Id = 1, PropietarioId = 2 };
        var newOwner = new Usuario { Id = 3, Activo = true };

        Assert.False(await _sut.PuedesTransferirProyectoAsync(project, newOwner, 99));
    }

    [Fact]
    public async Task PuedeAñadirPersonasAsync_DebePermitirManagerConUsuarioActivoNoMiembro()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Manager });
        _userRepo.ObtenerUsuarioPorIdAsync(3).Returns(new Usuario { Id = 3, Activo = true });
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns((ProyectoUsuario?)null);

        Assert.True(await _sut.PuedeAñadirPersonasAsync(1, 3, 2));
    }

    [Fact]
    public async Task PuedeAñadirPersonasAsync_DebeRechazarUsuarioYaMiembro()
    {
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 2).Returns(new ProyectoUsuario { Activo = true, Rol = RolProyecto.Manager });
        _userRepo.ObtenerUsuarioPorIdAsync(3).Returns(new Usuario { Id = 3, Activo = true });
        _projectUserRepo.ObtenerUnUsuarioDeUnProyectoAsync(1, 3).Returns(new ProyectoUsuario { Activo = true });

        Assert.False(await _sut.PuedeAñadirPersonasAsync(1, 3, 2));
    }
}
