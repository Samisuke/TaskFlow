using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Dto.Tarea;
using TaskFlow.Core.Services;

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
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasProyecto(int idProyecto)
        {
            var tareas = await _tareaService.GetTareasDeUnProyectoAsync(idProyecto);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<TareaReadDto>()); 
        }

        [HttpGet("mis-tareas")]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasPendientes(int id) // CAMBIO: Cambiar este id por el JWT
        {
            var tareas = await _tareaService.GetTareasPendientesDeUsuarioAsync(id);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpGet("tareas-asignadas")]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasAsignadas(int id) // CAMBIO: Cambiar este id por el JWT
        {
            var tareas = await _tareaService.GetTareasDadasDeUsuarioAsync(id);
            if(!tareas.EsCorrecto || tareas.Valor is null) return BadRequest(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TareaReadDto>> GetTarea(int id)
        {
            var tarea = await _tareaService.GetTareaPorIdAsync(id);
            if(!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            return Ok(tarea.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        [HttpPost]
        public async Task<ActionResult> PostTarea(int id, [FromBody] TareaWriteDto tareaWriteDto) // CAMBIO: poner id del JWT
        {
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

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchTarea(int id, [FromBody] TareaPatchDto tareaPatchDto, int idPropia) //CAMBIO: Cambiar idPropia por la del JWT
        {
            var tarea = await _tareaService.PatchTareaAsync(
                idPropia,
                id,
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

        [HttpPatch("{id}/estado")]
        public async Task<ActionResult> PatchEstadoTarea(int id, [FromBody] TareaEstadoPatchDto tareaPatchDto, int idPropia) //CAMBIO: Cambiar idPropia por la del JWT
        {
            var tarea = await _tareaService.PatchEstadoTareaAsync(
                idPropia,
                id,
                tareaPatchDto.Estado
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return Ok(tareaDto);
        }
    }
}