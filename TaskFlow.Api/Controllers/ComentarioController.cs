using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Comentario;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;
        private readonly ComentarioValidator _validator;
        private readonly ComentarioPatchValidator _validatorPatch;

        public ComentarioController(IComentarioService comentarioService,
            ComentarioValidator validator,
            ComentarioPatchValidator validatorPatch
        )
        {
            _comentarioService = comentarioService;
            _validator = validator;
            _validatorPatch = validatorPatch;
        }

        [HttpGet("mis-comentarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosPropios()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(id);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetComentarioId(int id)
        {
            var comentario = await _comentarioService.GetComentarioPorIdAsync(id);
            if (!comentario.EsCorrecto ||comentario.Valor is null) return NotFound(comentario.MensajeError);

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }

        [HttpGet("usuario/{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosUsuario(int id)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(id);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        [HttpGet("tarea/{idTarea}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosTarea(int idTarea)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

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

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentario = await _comentarioService.PostComentarioAsync(
                comentarioWriteDto.Contenido,
                id,
                comentarioWriteDto.TareaId
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError);

            var comentarioDto = comentario.Valor.Adapt<ComentarioReadDto>();
            return CreatedAtAction(nameof(GetComentarioId), new {id = comentarioDto.Id}, comentarioDto);
        }

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
            
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentario = await _comentarioService.PatchComentarioAsync(
                id,
                idComentario,
                comentarioPatchDto.Contenido
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError); 

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }
    }
}