using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Tarea;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
//
// Controlador encargado de gestionar las tareas.
//
// Funcionalidades:
//  - Obtener las tareas pertenecientes a un proyecto.
//  - Obtener las tareas pendientes del usuario autenticado.
//  - Obtener las tareas creadas por el usuario autenticado.
//  - Obtener una tarea mediante su ID.
//  - Crear una tarea dentro de un proyecto, incluyendo sus posibles etiquetas.
//  - Modificar la información básica de una tarea.
//  - Modificar el estado de una tarea mediante un endpoint independiente.
//
// La modificación del estado se mantiene separada de la modificación general
// de la tarea debido a que tiene unas reglas de permisos diferentes.
//
// La identidad del usuario se obtiene mediante JWT y las comprobaciones de
// pertenencia y permisos se delegan en los servicios correspondientes.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TareaController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly ITareaService _tareaService;
        private readonly TareaValidator _validator;
        private readonly TareaPatchValidator _validatorPatch;
        private readonly TareaEstadoPatchValidator _validatorEstado;
        public TareaController(
            ITareaService tareaService,
            TareaValidator validator,
            TareaPatchValidator validatorPatch,
            TareaEstadoPatchValidator validatorEstado
        )
        {
            _tareaService = tareaService;
            _validator = validator;
            _validatorPatch = validatorPatch;
            _validatorEstado = validatorEstado;
        }

        // Obtener las tareas pertenecientes a un proyecto.
        [HttpGet("proyecto/{proyectoId}/tareas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetTareasProyecto(int proyectoId)
        {
            var tareas = await _tareaService.GetTareasDeUnProyectoAsync(proyectoId);
            if(!tareas.EsCorrecto || tareas.Valor is null) return NotFound(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<TareaReadDto>()); 
        }

        // Obtener las tareas pendientes propias.
        [HttpGet("mis-tareas-pendientes")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetMisTareasPendientes()
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tareas = await _tareaService.GetTareasPendientesDeUsuarioAsync(idJWT);
            if(!tareas.EsCorrecto || tareas.Valor is null) return NotFound(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }
        
        // Obtener las tareas asignadas propias.
        [HttpGet("mis-tareas-asignadas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TareaReadDto>>> GetMisTareasAsignadas() 
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tareas = await _tareaService.GetTareasDadasDeUsuarioAsync(idJWT);
            if(!tareas.EsCorrecto || tareas.Valor is null) return NotFound(tareas.MensajeError);

            return Ok(tareas.Valor.Adapt<IEnumerable<TareaReadDto>>()); 
        }

        // Obtener una tarea por su ID.
        [HttpGet("{tareaId}")]
        [Authorize]
        public async Task<ActionResult<TareaReadDto>> GetTarea(int tareaId)
        {
            var tarea = await _tareaService.GetTareaPorIdAsync(tareaId);
            if(!tarea.EsCorrecto || tarea.Valor is null) return NotFound(tarea.MensajeError);

            return Ok(tarea.Valor.Adapt<TareaReadDto>());
        }

        // Crear una tarea contigo como creador.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostTarea([FromBody] TareaWriteDto tareaWriteDto)
        {
            var validationResult = await _validator.ValidateAsync(tareaWriteDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PostTareaAsync(
                idJWT,
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

        // Modificar una tarea.
        [HttpPatch("{tareaId}")]
        [Authorize]
        public async Task<ActionResult> PatchTarea(int tareaId, [FromBody] TareaPatchDto tareaPatchDto)
        {
            var validationResult = await _validatorPatch.ValidateAsync(tareaPatchDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PatchTareaAsync(
                idJWT,
                tareaId,
                tareaPatchDto.Titulo,
                tareaPatchDto.Descripcion,
                tareaPatchDto.Prioridad,
                tareaPatchDto.FechaLimite,
                tareaPatchDto.Etiquetas!
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return Ok(tareaDto);
        }

        // Modificar el estado de una tarea.
        [HttpPatch("{tareaId}/estado")]
        [Authorize]
        public async Task<ActionResult> PatchEstadoTarea(int tareaId, [FromBody] TareaEstadoPatchDto tareaPatchDto)
        {
            var validationResult = await _validatorEstado.ValidateAsync(tareaPatchDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tarea = await _tareaService.PatchEstadoTareaAsync(
                idJWT,
                tareaId,
                tareaPatchDto.Estado
            );
            if (!tarea.EsCorrecto || tarea.Valor is null) return BadRequest(tarea.MensajeError);

            var tareaDto = tarea.Valor.Adapt<TareaReadDto>();
            return Ok(tareaDto);
        }
    }
}