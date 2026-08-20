using BCrypt.Net;
using TaskFlow.Core.Models;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Tests.Permissions;

public class PassPermissionServiceTests
{
    private readonly PassPermissionService _sut = new();

    [Fact]
    public void ComprobacionesPass_ConPasswordActualCorrectaYNuevaDistinta_DebePermitir()
    {
        var user = new Usuario { PasswordHash = BCrypt.Net.BCrypt.HashPassword("old-password") };

        var result = _sut.ComprobacionesPass(user, "old-password", "new-password");

        Assert.True(result.EsCorrecto);
    }

    [Fact]
    public void ComprobacionesPass_ConPasswordActualIncorrecta_DebeRechazar()
    {
        var user = new Usuario { PasswordHash = BCrypt.Net.BCrypt.HashPassword("old-password") };

        var result = _sut.ComprobacionesPass(user, "wrong", "new-password");

        Assert.False(result.EsCorrecto);
        Assert.Equal("La contraseña introducida no es correcta", result.Error);
    }

    [Fact]
    public void ComprobacionesPass_SiNuevaEsIgualALaActual_DebeRechazar()
    {
        var user = new Usuario { PasswordHash = BCrypt.Net.BCrypt.HashPassword("old-password") };

        var result = _sut.ComprobacionesPass(user, "old-password", "old-password");

        Assert.False(result.EsCorrecto);
        Assert.Equal("La nueva contraseña no puede ser la misma que la que ya tienes.", result.Error);
    }
}
