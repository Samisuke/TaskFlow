using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Login;
using TaskFlow.Core.Repositories;
using TaskFlow.Core.Services.Token;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
// Controlador de LogIn.
// Misión: Un usuario debe poder registrarse en la aplicación para poder interactuar con ella. Este Controller cuenta con
// validación para comprobar que envías las credenciales, comprueba que sean correctas y que no estés 
// inactivo. Si una falla, devuelve un mensaje de error personalizado. En caso de ser correcto, crea y te devuelve un token JWT.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class AuthController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly ITokenService _tokenService;
        private readonly IUsuarioRepository _repoUsuario;
        private readonly LoginValidator _validator;

        public AuthController(
            ITokenService tokenService,
            IUsuarioRepository repoUsuario,
            LoginValidator validator
        )
        {
            _tokenService = tokenService;
            _repoUsuario = repoUsuario;
            _validator = validator;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUsuario([FromBody] LoginDto loginDto)
        {
            var validationResult = await _validator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
        
            var usuario = await _repoUsuario.ObtenerUsuarioPorEmailAsync(loginDto.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(loginDto.Pass, usuario.PasswordHash)) return Unauthorized("Credenciales incorrectas. Inténtalo de nuevo");
            if (!usuario.Activo) return BadRequest("Tu cuenta está inactiva. Ponte en contacto con soporte para activarla.");

            var token = _tokenService.GenerarToken(usuario.Id, usuario.Nombre, usuario.Email);
            return Ok(new {Token = token, Mensaje = "Login correcto"});
        }
    }
}