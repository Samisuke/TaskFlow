using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace TaskFlow.IntegrationTests;

public class TestAuthenticationOptions
{
}

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{

    public static int UserId {get; set;} = 1;

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

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