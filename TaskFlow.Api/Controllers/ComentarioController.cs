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

        // Obtener un comentario concreto por ID.
        [HttpGet("{comentarioId}")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetComentario(int comentarioId)
        {
            var comentario = await _comentarioService.GetComentarioPorIdAsync(comentarioId);
            if (!comentario.EsCorrecto ||comentario.Valor is null) return NotFound(comentario.MensajeError);

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }

        // Obtener tus comentarios usando ID del JWT.
        [HttpGet("mis-comentarios")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetMisComentarios(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 5
        )
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(
                idJWT,
                pagina,
                tamanoPagina
            );
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            var resultado = new
            {
                Items = comentarios.Valor.Items
                    .Adapt<IEnumerable<ComentarioReadDto>>(),
                comentarios.Valor.Pagina,
                comentarios.Valor.TotalPaginas,
                comentarios.Valor.TamanoPagina,
                comentarios.Valor.TotalItems
            };

            return Ok(resultado);
        }

        // Obtener comentarios de un usuario por ID del usuario.
        [HttpGet("usuario/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetComentariosUsuario(
            int usuarioId,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 5
        )
        {
            var comentarios = await _comentarioService.GetComentariosDeUnUsuarioAsync(
                usuarioId,
                pagina,
                tamanoPagina
            );
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            var resultado = new
            {
                Items = comentarios.Valor.Items
                    .Adapt<IEnumerable<ComentarioReadDto>>(),
                comentarios.Valor.Pagina,
                comentarios.Valor.TotalPaginas,
                comentarios.Valor.TamanoPagina,
                comentarios.Valor.TotalItems
            };

            return Ok(resultado);
        }

        // Obtener todos los comentarios de una tarea.
        [HttpGet("tarea/{tareaId}")]
        [Authorize]
        public async Task<ActionResult<ComentarioReadDto>> GetComentariosTarea(
            int tareaId,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 5
        )
        {
            var comentarios = await _comentarioService.GetComentariosDeUnaTareaAsync(
                tareaId,
                pagina,
                tamanoPagina
            );
            if (!comentarios.EsCorrecto || comentarios.Valor is null) return NotFound(comentarios.MensajeError);

            var resultado = new
            {
                Items = comentarios.Valor.Items
                    .Adapt<IEnumerable<ComentarioReadDto>>(),
                comentarios.Valor.Pagina,
                comentarios.Valor.TotalPaginas,
                comentarios.Valor.TamanoPagina,
                comentarios.Valor.TotalItems
            };

            return Ok(resultado);
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
            return CreatedAtAction(nameof(GetComentario), new {comentarioId = comentarioDto.Id}, comentarioDto);
        }

        // Modificar un comentario
        [HttpPatch("{comentarioId}")]
        [Authorize]
        public async Task<ActionResult> PatchComentario(int comentarioId, [FromBody] ComentarioPatchDto comentarioPatchDto)
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
                comentarioId,
                comentarioPatchDto.Contenido!
            );
            if (!comentario.EsCorrecto ||comentario.Valor is null) return BadRequest(comentario.MensajeError); 

            return Ok(comentario.Valor.Adapt<ComentarioReadDto>());
        }
    }
}