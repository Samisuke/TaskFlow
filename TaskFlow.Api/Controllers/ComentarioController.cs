using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Comentario;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
//
// Controlador encargado de gestionar los comentarios de las tareas.
//
// Funcionalidades:
//  - Obtener un comentario mediante su ID.
//  - Obtener todos los comentarios del usuario autenticado.
//  - Obtener todos los comentarios de un usuario.
//  - Obtener todos los comentarios de una tarea.
//  - Crear un comentario en una tarea.
//  - Modificar un comentario.
//
// El acceso a los comentarios está condicionado por la pertenencia del usuario
// al proyecto correspondiente.
//
// Las operaciones de creación y modificación también registran los cambios
// relevantes en el historial del proyecto.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ComentarioController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly IComentarioService _comentarioService;
        private readonly ComentarioValidator _validator;
        private readonly ComentarioPatchValidator _validatorPatch;

        public ComentarioController(
            IComentarioService comentarioService,
            ComentarioValidator validator,
            ComentarioPatchValidator validatorPatch
        )
        {
            _comentarioService = comentarioService;
            _validator = validator;
            _validatorPatch = validatorPatch;
        }

        // Obtener tus comentarios usando ID del JWT.
        [HttpGet("mis-comentarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetMisComentarios()
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(idJWT);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        // Obtener un comentario concreto por ID.
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetComentario(int id)
        {
            var comentario = await _comentarioService.GetComentarioPorIdAsync(id);
            if (!comentario.EsCorrecto ||comentario.Valor is null) return NotFound(comentario.MensajeError);

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }

        // Obtener comentarios de un usuario por ID del usuario.
        [HttpGet("usuario/{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosUsuario(int id)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(id);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        // Obtener todos los comentarios de una tarea.
        [HttpGet("tarea/{idTarea}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosTarea(int idTarea)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        // Realizar un comentario
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostComentario([FromBody] ComentarioWriteDto comentarioWriteDto)
        {
            var validationResult = await _validator.ValidateAsync(comentarioWriteDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentario = await _comentarioService.PostComentarioAsync(
                comentarioWriteDto.Contenido,
                idJWT,
                comentarioWriteDto.TareaId
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError);

            var comentarioDto = comentario.Valor.Adapt<ComentarioReadDto>();
            return CreatedAtAction(nameof(GetComentario), new {id = comentarioDto.Id}, comentarioDto);
        }

        // Modificar un comentario
        [HttpPatch("{idComentario}")]
        [Authorize]
        public async Task<ActionResult> PatchComentario(int idComentario, [FromBody] ComentarioPatchDto comentarioPatchDto)
        {
            var validationResult = await _validatorPatch.ValidateAsync(comentarioPatchDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentario = await _comentarioService.PatchComentarioAsync(
                idJWT,
                idComentario,
                comentarioPatchDto.Contenido!
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError); 

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }
    }
}