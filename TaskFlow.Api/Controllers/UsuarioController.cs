using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Services;
using TaskFlow.Core.Dto.Usuario;
using Mapster;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;
namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UsuarioValidator _validator;
        private readonly UsuarioPatchValidator _validatorPatch;
        private readonly UsuarioPatchPassValidator _validatorPass;

        public UsuarioController(IUsuarioService usuarioService,
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
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuario()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener tu perfil personal.
        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<UsuarioReadDto>> GetTuPerfil()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener un usuario por email.
        [HttpGet("buscar/{email}")]
        [Authorize]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuarioPorEmail(string email)
        {
            var usuario = await _usuarioService.GetUsuarioPorEmailAsync(email);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Crear un usuario.
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

        // Modificar tu perfil.
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

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.PatchUsuarioAsync(
                id,
                usuarioPatchDto.Nombre,
                usuarioPatchDto.Apellidos,
                usuarioPatchDto.Email,
                usuarioPatchDto.Activo
            );
            if (usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

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

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _usuarioService.PatchUsuarioPassAsync(
                id,
                usuarioPassDto.PassNueva,
                usuarioPassDto.PassAntigua
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Valor.Adapt<UsuarioReadDto>();
            return Ok(usuarioDto);
        }
    }
}