using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Dto.Comentario;
using TaskFlow.Core.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentarioController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        [HttpGet("mis-comentarios")]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosPropios(int id) // CAMBIO: Cambiar este ID por el JWT
        {
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(id);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComentarioReadDto>> GetComentarioId(int id)
        {
            var comentario = await _comentarioService.GetComentarioPorIdAsync(id);
            if (!comentario.EsCorrecto ||comentario.Valor is null) return NotFound(comentario.MensajeError);

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }

        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosUsuario(int id)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(id);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        [HttpGet("tarea/{idTarea}")]
        public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosTarea(int idTarea)
        {
            var comentarios = await _comentarioService.GetComentariosDeUnaTareaAsync(idTarea);
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            return Ok(comentarios.Valor.Adapt<IEnumerable<ComentarioReadDto>>());
        }

        [HttpPost]
        public async Task<ActionResult> PostComentario([FromBody] ComentarioWriteDto comentarioWriteDto)
        {
            var comentario = await _comentarioService.PostComentarioAsync(
                comentarioWriteDto.Contenido,
                comentarioWriteDto.UsuarioId, // CAMBIO: Cambiar esta id por la del JWT
                comentarioWriteDto.TareaId
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError);

            var comentarioDto = comentario.Valor.Adapt<ComentarioReadDto>();
            return CreatedAtAction(nameof(GetComentarioId), new {id = comentarioDto.Id}, comentarioDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchComentario(int id, [FromBody] ComentarioPatchDto comentarioPatchDto, int idPropia) // CAMBIO: Cambiar esta ID por JWT
        {
           var comentario = await _comentarioService.PatchComentarioAsync(
                idPropia,
                id,
                comentarioPatchDto.Contenido
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError); 

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }
    }
}