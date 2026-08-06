using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Dto.Tarea;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TareaController : ControllerBase
    {
        private readonly ITareaService _tareaService;
        public TareaController(ITareaService tareaService)
        {
            _tareaService = tareaService;
        }

        [HttpGet("proyecto/{idProyecto}/tareas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasProyecto(int idProyecto)
        {
            var tareas = await _tareaService.GetTareasDeUnProyectoAsync(idProyecto);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<TareaReadDto>()); 
        }

        [HttpGet("mis-tareas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasPendientes()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tareas = await _tareaService.GetTareasPendientesDeUsuarioAsync(id);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpGet("tareas-asignadas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasAsignadas() 
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tareas = await _tareaService.GetTareasDadasDeUsuarioAsync(id);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TareaReadDto>> GetTarea(int id)
        {
            var tarea = await _tareaService.GetTareaPorIdAsync(id);
            if(!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            return Ok(tarea.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostTarea([FromBody] TareaWriteDto tareaWriteDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PostTareaAsync(
                id,
                tareaWriteDto.Titulo,
                tareaWriteDto.Descripcion,
                tareaWriteDto.Estado,
                tareaWriteDto.Prioridad,
                tareaWriteDto.FechaLimite,
                tareaWriteDto.ProyectoId,
                tareaWriteDto.AsignadoId,
                tareaWriteDto.Etiquetas
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return CreatedAtAction(nameof(GetTarea), new {id = tareaDto.Id}, tareaDto);
        }

        [HttpPatch("{idTarea}")]
        [Authorize]
        public async Task<ActionResult> PatchTarea(int idTarea, [FromBody] TareaPatchDto tareaPatchDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PatchTareaAsync(
                id,
                idTarea,
                tareaPatchDto.Titulo,
                tareaPatchDto.Descripcion,
                tareaPatchDto.Prioridad,
                tareaPatchDto.FechaLimite,
                tareaPatchDto.Etiquetas
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return Ok(tareaDto);
        }

        [HttpPatch("{idTarea}/estado")]
        [Authorize]
        public async Task<ActionResult> PatchEstadoTarea(int idTarea, [FromBody] TareaEstadoPatchDto tareaPatchDto)
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PatchEstadoTareaAsync(
                id,
                idTarea,
                tareaPatchDto.Estado
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return Ok(tareaDto);
        }
    }
}