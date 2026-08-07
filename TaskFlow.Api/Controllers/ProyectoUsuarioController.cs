using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.ProyectoUsuario;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProyectoUsuarioController : ControllerBase
    {
        private readonly IProyectoUsuarioService _pusuarioService;
        private readonly ProyectoUsuarioValidator _validator;

        public ProyectoUsuarioController(IProyectoUsuarioService pusuarioService,
            ProyectoUsuarioValidator validator
        )
        {
            _pusuarioService = pusuarioService;
            _validator = validator;
        }

        [HttpGet("proyecto/{idProyecto}/usuario/{id}")]
        [Authorize]
        public async Task<ActionResult<ProyectoUsuarioReadDto>> GetProyectoUsuario(int idProyecto, int id)
        {
            var usuario = await _pusuarioService.GetUsuarioDeUnProyectoAsync(idProyecto, id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<ProyectoUsuarioReadDto>());
        }

        [HttpGet("proyecto/{idProyecto}/usuarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoUsuarioReadDto>>> GetUsuariosProyecto(int idProyecto)
        {
            var usuario = await _pusuarioService.GetTodosLosUsuariosDeUnProyectoAsync(idProyecto);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<IEnumerable<ProyectoUsuarioReadDto>>());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostProyectoUsuario([FromBody] ProyectoUsuarioWriteDto proyetoUsuarioWriteDto)
        {
            var validationResult = await _validator.ValidateAsync(proyetoUsuarioWriteDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var miembroProyecto = await _pusuarioService.PostUsuarioAsync(
                id,
                proyetoUsuarioWriteDto.UsuarioId,
                proyetoUsuarioWriteDto.ProyectoId,
                proyetoUsuarioWriteDto.Rol
            );
            if (!miembroProyecto.EsCorrecto ||miembroProyecto.Valor is null) return NotFound(miembroProyecto.MensajeError);

            var usuarioDto = miembroProyecto.Valor.Adapt<ProyectoUsuarioReadDto>();
            return Ok(usuarioDto);
        }

        [HttpPatch("proyecto/{idProyecto}/usuario/{idUsuario}")]
        [Authorize]
        public async Task<ActionResult> PatchUsuarioProyecto(int idProyecto,int idUsuario, [FromBody] ProyectoUsuarioPathcDto proyetoUsuarioPathcDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _pusuarioService.PatchUsuarioAsync(
                id,
                idUsuario,
                idProyecto,
                proyetoUsuarioPathcDto.Activo,
                proyetoUsuarioPathcDto.Rol
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            var usuarioDto = usuario.Adapt<ProyectoUsuarioReadDto>();
            return Ok(usuarioDto);
        }
    }
}