using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Services;
using TaskFlow.Api.Dto.Usuario;
using Mapster;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Obtener un usuario por ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuario(int id) // CAMBIO: Cambiar esta ID por la de JWT
        {
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener tu perfil personal.
        [HttpGet("perfil")]
        public async Task<ActionResult<UsuarioReadDto>> GetTuPerfil(int id) // CAMBIO: Cambiar esta ID por la de JWT
        {
            var usuario = await _usuarioService.GetUsuarioPorIdAsync(id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Obtener un usuario por email.
        [HttpGet("buscar/{email}")]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuarioPorEmail(string email) // CAMBIO: Cambiar esta ID por la de JWT
        {
            var usuario = await _usuarioService.GetUsuarioPorEmailAsync(email);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<UsuarioReadDto>());
        } 

        // Crear un usuario.
        [HttpPost]
        public async Task<ActionResult> PostUsuario([FromBody] UsuarioWriteDto usuarioWriteDto)
        {
            var usuario = await _usuarioService.PostUsuarioAsync(
                usuarioWriteDto.Nombre,
                usuarioWriteDto.Apellidos,
                usuarioWriteDto.Email,
                usuarioWriteDto.PasswordHash,
                usuarioWriteDto.Activo
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Valor.Adapt<UsuarioReadDto>();
            return CreatedAtAction(nameof(GetUsuario), new {id = usuarioDto.Id}, usuarioDto);
        }

        // Modificar tu perfil.
        [HttpPatch]
        public async Task<ActionResult> PatchUsuario(int id, [FromBody] UsuarioPatchDto usuarioPatchDto) // CAMBIO: Cambiar esta ID por la de JWT
        {
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

        // Cambiar tu contraseña.
        public async Task<ActionResult> PatchPassUsuario(int id, [FromBody] UsuarioPassDto usuarioPassDto) // CAMBIO: Cambiar esta ID por la de JWT
        {
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