using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Dto.Proyecto;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProyectoController : ControllerBase
    {
        private readonly IProyectoService _proyectoService;

        public ProyectoController(
            IProyectoService proyectoService
        )
        {
            _proyectoService = proyectoService;
        }

        // Obtener un solo proyecto por su ID.
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> GetProyecto(int id)
        {
            var proyecto = await _proyectoService.GetProyectoPorIdAsync(id);
            if (!proyecto.EsCorrecto || proyecto.Valor is null) return NotFound(proyecto.MensajeError);

            return Ok(proyecto.Valor.Adapt<ProyectoReadDto>());  
        }

        // Obtener los proyectos en los que trabaja un usuario.
        [HttpGet("usuario/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoReadDto>>> GetProyectosDeUsuario(int usuarioId)
        {
            var proyectos = await _proyectoService.GetProyectosDeUnaPersonaAsync(usuarioId);
            if (!proyectos.EsCorrecto || proyectos.Valor is null) return NotFound(proyectos.MensajeError);

            return Ok(proyectos.Valor.Adapt<IEnumerable<ProyectoReadDto>>());
        }

        // Obtener los proyectos creados por un usuario.
        [HttpGet("propietario/{propietarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoReadDto>>> GetProyectosDeCreador(int propietarioId)
        {
            var proyectos = await _proyectoService.GetProyectosDeUnCreadorAsync(propietarioId);
            if (!proyectos.EsCorrecto || proyectos.Valor is null) return NotFound(proyectos.MensajeError);

            return Ok(proyectos.Valor.Adapt<IEnumerable<ProyectoReadDto>>());
        }

        // Obtener los proyectos propios.
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoReadDto>>> GetProyectosPropios()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var proyectos = await _proyectoService.GetProyectosDeUnaPersonaAsync(id);
            if (!proyectos.EsCorrecto || proyectos.Valor is null) return NotFound(proyectos.MensajeError);

            return Ok(proyectos.Valor.Adapt<IEnumerable<ProyectoReadDto>>());
        }

        // Crear un proyecto.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostProyecto([FromBody] ProyectoWriteDto proyectoDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var proyectoNuevo = await _proyectoService.PostProyectoAsync(
                proyectoDto.Nombre,
                proyectoDto.Descripcion,
                id
            );

            if (!proyectoNuevo.EsCorrecto || proyectoNuevo.Valor is null) return BadRequest(proyectoNuevo.MensajeError);

            var proyectoNuevoDto = proyectoNuevo.Valor.Adapt<ProyectoReadDto>();
            return CreatedAtAction(nameof(GetProyecto), new {id = proyectoNuevoDto.Id}, proyectoNuevoDto);
        }

        // Modificar un proyecto
        [HttpPatch("{idProyecto}")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> PatchProyecto(int idProyecto, [FromBody] ProyectoPatchDto proyectoDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cambios = await _proyectoService.PatchProyectoAsync(
                id,
                idProyecto,
                proyectoDto.Nombre,
                proyectoDto.Descripcion
            );
            if (!cambios.EsCorrecto || cambios.Valor is null) return BadRequest(cambios.MensajeError);

            return Ok(cambios.Valor.Adapt<ProyectoReadDto>());
        }

        // Cambiar propietario de un proyecto
        [HttpPatch("{idProyecto}/owner")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> TransferirPropiedadProyecto(int idProyecto, [FromBody] ProyectoPatchDueñoDto proyectoPatchDueñoDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cambioPropietario = await _proyectoService.PatchDueñoProyectoAsync(
                id,
                idProyecto,
                proyectoPatchDueñoDto.NuevoPropietarioId
            );
            if (!cambioPropietario.EsCorrecto || cambioPropietario.Valor is null) return BadRequest(cambioPropietario.MensajeError);

            return Ok(cambioPropietario.Valor.Adapt<ProyectoReadDto>());
        }
    }
}