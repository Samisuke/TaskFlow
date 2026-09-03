using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

// <summary>
// Manejador de autenticación utilizado exclusivamente durante las pruebas de integración. 
// Sustituye el mecanismo de autenticación real por uno controlado que genera
// las credenciales del usuario simulado, permitiendo probar endpoints protegidos
// sin depender de tokens JWT reales.
// </summary>

namespace TaskFlow.IntegrationTests;

public class TestAuthenticationOptions
{
}

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // Identificador del usuario que se simula como autenticado durante la prueba.
    public static int UserId {get; set;} = 1;

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    // Genera una identidad autenticada con los claims necesarios para la aplicación.
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim(ClaimTypes.Name, "Nombre User"),
            new Claim(ClaimTypes.Email, "test@testmail.com")
        };

        var identity = new ClaimsIdentity(
            claims,
            "TestAuthentication");

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            "TestAuthentication");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}