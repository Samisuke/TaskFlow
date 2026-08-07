using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Login;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services.Token;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUsuarioRepository _repoUsuario;

        public AuthController(ITokenService tokenService, IUsuarioRepository repoUsuario)
        {
            _tokenService = tokenService;
            _repoUsuario = repoUsuario;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUsuario([FromBody] LoginDto loginDto)
        {
            var usuario = await _repoUsuario.ObtenerUsuarioPorEmailAsync(loginDto.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(loginDto.Pass, usuario.PasswordHash)) return Unauthorized("Credenciales incorrectas. Inténtalo de nuevo");
            if (!usuario.Activo) return BadRequest("Tu cuenta está inactiva. Ponte en contacto con soporte para activarla.");

            var token = _tokenService.GenerarToken(usuario.Id, usuario.Nombre, usuario.Email);
            return Ok(new {Token = token, Mensaje = "Login correcto"});
        }
    }
}