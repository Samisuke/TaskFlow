using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.ProyectoUsuario;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
//
// Controlador encargado de gestionar la relación entre usuarios y proyectos.
//
// Funcionalidades:
//  - Obtener la información de un usuario dentro de un proyecto.
//  - Obtener todos los usuarios pertenecientes a un proyecto.
//  - Añadir un usuario a un proyecto.
//  - Modificar el rol o el estado de un usuario dentro de un proyecto.
//
// La relación ProyectoUsuario permite gestionar la pertenencia, el rol y el
// estado de cada usuario dentro de un proyecto.
//
// Las operaciones de modificación están sujetas a las reglas de permisos
// definidas por el modelo de negocio y gestionadas desde los servicios.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProyectoUsuarioController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly IProyectoUsuarioService _pUsuarioService;
        private readonly ProyectoUsuarioValidator _validator;

        public ProyectoUsuarioController(
            IProyectoUsuarioService pUsuarioService,
            ProyectoUsuarioValidator validator
        )
        {
            _pUsuarioService = pUsuarioService;
            _validator = validator;
        }

        // Permite ver el perfil de un usuario dentro de un proyecto.
        [HttpGet("proyecto/{proyectoId}/usuario/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<ProyectoUsuarioReadDto>> GetPerfil(int proyectoId, int usuarioId)
        {
            var usuario = await _pUsuarioService.GetUsuarioDeUnProyectoAsync(proyectoId, usuarioId);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<ProyectoUsuarioReadDto>());
        }

        // Permite ver tu perfil de un usuario dentro de un proyecto.
        [HttpGet("proyecto/{proyectoId}/mi-perfil")]
        [Authorize]
        public async Task<ActionResult<ProyectoUsuarioReadDto>> GetPerfilPropio(int proyectoId)
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _pUsuarioService.GetUsuarioDeUnProyectoAsync(proyectoId, idJWT);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<ProyectoUsuarioReadDto>());
        }

        // Permite ver todos los perfiles de usuario dentro de un proyecto.
        [HttpGet("proyecto/{proyectoId}/usuarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoUsuarioReadDto>>> GetPerfiles(int proyectoId)
        {
            var usuario = await _pUsuarioService.GetTodosLosUsuariosDeUnProyectoAsync(proyectoId);
            if (!usuario.EsCorrecto || usuario.Valor is null) return NotFound(usuario.MensajeError);

            return Ok(usuario.Valor.Adapt<IEnumerable<ProyectoUsuarioReadDto>>());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostPerfil([FromBody] ProyectoUsuarioWriteDto proyetoUsuarioWriteDto)
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

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var miembroProyecto = await _pUsuarioService.PostUsuarioAsync(
                idJWT,
                proyetoUsuarioWriteDto.UsuarioId,
                proyetoUsuarioWriteDto.ProyectoId,
                proyetoUsuarioWriteDto.Rol
            );
            if (!miembroProyecto.EsCorrecto ||miembroProyecto.Valor is null) return BadRequest(miembroProyecto.MensajeError);

            var usuarioDto = miembroProyecto.Valor.Adapt<ProyectoUsuarioReadDto>();
            return Ok(usuarioDto);
        }

        [HttpPatch("proyecto/{proyectoId}/usuario/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult> PatchPerfil(int proyectoId,int usuarioId, [FromBody] ProyectoUsuarioPatchDto proyetoUsuarioPathcDto)
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _pUsuarioService.PatchUsuarioAsync(
                idJWT,
                usuarioId,
                proyectoId,
                proyetoUsuarioPathcDto.Activo,
                proyetoUsuarioPathcDto.Rol
            );
            if (!usuario.EsCorrecto || usuario.Valor is null) return BadRequest(usuario.MensajeError);

            var usuarioDto = usuario.Adapt<ProyectoUsuarioReadDto>();
            return Ok(usuarioDto);
        }
    }
}