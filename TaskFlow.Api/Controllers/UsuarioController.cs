using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Services;
using TaskFlow.Core.Dto.Usuario;
using Mapster;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
//
// Controlador encargado de gestionar los usuarios.
//
// Funcionalidades:
//  - Obtener un usuario por su ID.
//  - Obtener el perfil del usuario autenticado mediante su JWT.
//  - Buscar un usuario mediante su email.
//  - Crear un usuario.
//  - Modificar los datos personales de un usuario.
//  - Modificar la contraseña de un usuario comprobando previamente su contraseña actual.
//
// La identidad del usuario autenticado se obtiene del JWT en las operaciones
// que afectan al propio usuario, evitando que el cliente pueda modificar
// arbitrariamente la identidad sobre la que se realiza la operación.
//
// Las comprobaciones de permisos y las reglas de negocio se delegan en los servicios.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsuarioController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly IUsuarioService _usuarioService;
        private readonly UsuarioValidator _validator;
        private readonly UsuarioPatchValidator _validatorPatch;
        private readonly UsuarioPatchPassValidator _validatorPass;

        public UsuarioController(
            IUsuarioService usuarioService,
            UsuarioValidator validator,
            UsuarioPatchValidator validatorPatch,
            UsuarioPatchPassValidator validatorPass
        )
        {
            _usuarioService = usuarioService;
            _validator = validator;
            _validatorPass = validatorPass;
            _validatorPatch = validatorPatch;
        }

        // Obtener un usuario por ID.
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuario(int usuarioId)
        {
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(usuarioId);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener tu perfil personal.
        [HttpGet("mi-perfil-personal")]
        [Authorize]
        public async Task<ActionResult<UsuarioReadDto>> GetMiPerfilPersonal()
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(idJWT);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener un usuario por email.
        [HttpGet("buscar/{usuarioEmail}")]
        [Authorize]
        public async Task<ActionResult<UsuarioResumenDto>> GetUsuarioEmail(string usuarioEmail)
        {
            var usuario = await _usuarioService.GetUsuarioPorEmailAsync(usuarioEmail);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioResumenDto>());
        } 

        // Crear un usuario (una cuenta).
        [HttpPost]
        public async Task<ActionResult> PostUsuario([FromBody] UsuarioWriteDto usuarioWriteDto)
        {
            var validationResult = await _validator.ValidateAsync(usuarioWriteDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var usuario = await _usuarioService.PostUsuarioAsync(
                usuarioWriteDto.Nombre,
                usuarioWriteDto.Apellidos,
                usuarioWriteDto.Email,
                usuarioWriteDto.Password
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Valor.Adapt<UsuarioReadDto>();
            return CreatedAtAction(nameof(GetUsuario), new {id = usuarioDto.Id}, usuarioDto);
        }

        // Modificar tu perfil personal.
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> PatchUsuario([FromBody] UsuarioPatchDto usuarioPatchDto)
        {
            var validationResult = await _validatorPatch.ValidateAsync(usuarioPatchDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.PatchUsuarioAsync(
                idJWT,
                usuarioPatchDto.Nombre,
                usuarioPatchDto.Apellidos,
                usuarioPatchDto.Email,
                usuarioPatchDto.Activo
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Valor.Adapt<UsuarioReadDto>();
            return Ok(usuarioDto);

        }
        [HttpPatch("cambiar-pass")]
        [Authorize]
        // Cambiar tu contraseña.
        public async Task<ActionResult> PatchPassUsuario([FromBody] UsuarioPassDto usuarioPassDto)
        {
            var validationResult = await _validatorPass.ValidateAsync(usuarioPassDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.PatchUsuarioPassAsync(
                idJWT,
                usuarioPassDto.PassNueva,
                usuarioPassDto.PassAntigua
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Valor.Adapt<UsuarioReadDto>();
            return Ok(usuarioDto);
        }
    }
}