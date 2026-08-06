using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Dto.ProyectoUsuario;
using TaskFlow.Core.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProyectoUsuarioController : ControllerBase
    {
        private readonly IProyectoUsuarioService _pusuarioService;

        public ProyectoUsuarioController(IProyectoUsuarioService pusuarioService)
        {
            _pusuarioService = pusuarioService;
        }

        [HttpGet("proyecto/{idProyecto}/usuario/{id}")]
        public async Task<ActionResult<ProyectoUsuarioReadDto>> GetProyectoUsuario(int idProyecto, int id)
        {
            var usuario = await _pusuarioService.GetUsuarioDeUnProyectoAsync(idProyecto, id);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<ProyectoUsuarioReadDto>());
        }

        [HttpGet("proyecto/{idProyecto}/usuarios")]
        public async Task<ActionResult<IEnumerable<ProyectoUsuarioReadDto>>> GetUsuariosProyecto(int idProyecto)
        {
            var usuario = await _pusuarioService.GetTodosLosUsuariosDeUnProyectoAsync(idProyecto);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<IEnumerable<ProyectoUsuarioReadDto>>());
        }

        [HttpPost]
        public async Task<ActionResult> PostProyectoUsuario([FromBody] ProyectoUsuarioWriteDto proyetoUsuarioWriteDto, int id) // CAMBIO: Cambiar ID por la del JWT
        {
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

        [HttpPatch("proyecto/{idProyecto}/usuario/{id}")]
        public async Task<ActionResult> PatchUsuarioProyecto(int idProyecto,int id, [FromBody] ProyectoUsuarioPathcDto proyetoUsuarioPathcDto, int idPropia) // CAMBIO: Cambiar ID propia por JWT
        {
            var usuario = await _pusuarioService.PatchUsuarioAsync(
                idPropia,
                id,
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