using Mapster;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Core.Dto.Proyecto;
using TaskFlow.Core.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Core.Validations;

// Notas para un posible reclutador:
//
// Controlador encargado de gestionar los proyectos.
//
// Funcionalidades:
//  - Obtener un proyecto por su ID.
//  - Obtener los proyectos a los que pertenece un usuario.
//  - Obtener los proyectos creados por un usuario.
//  - Obtener los proyectos del usuario autenticado mediante su JWT.
//  - Crear un proyecto utilizando automáticamente el usuario autenticado como propietario.
//  - Modificar la información básica de un proyecto, comprobando los permisos correspondientes.
//  - Transferir la propiedad de un proyecto, realizando las comprobaciones de negocio necesarias.
//
// El controlador se encarga de recibir las peticiones, validar los datos de entrada
// y delegar la lógica de negocio en los servicios correspondientes.
//
// Los permisos y reglas de negocio se gestionan fuera del controlador para mantener
// separadas las responsabilidades.
//
// Este controlador existe porque los proyectos son una funcionalidad principal de TaskFlow,
// permitiendo a los usuarios crearlos, consultarlos, modificarlos y participar en ellos.

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProyectoController : ControllerBase
    {
        // Inyección de servicio y validadores.
        private readonly IProyectoService _proyectoService;
        private readonly ProyectoValidator _validator;
        private readonly ProyectoPatchValidator _validatorPatch;
        private readonly ProyectoPatchDueñoValidator _validatorDueño;

        public ProyectoController(
            IProyectoService proyectoService,
            ProyectoValidator validator,
            ProyectoPatchValidator validatorPatch,
            ProyectoPatchDueñoValidator validatorDueño
        )
        {
            _proyectoService = proyectoService;
            _validator = validator;
            _validatorPatch = validatorPatch;
            _validatorDueño = validatorDueño;
        }

        // Obtener un solo proyecto por su ID.
        [HttpGet("{proyectoId}")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> GetProyecto(int proyectoId)
        {
            var proyecto = await _proyectoService.GetProyectoPorIdAsync(proyectoId);
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
        [HttpGet("mis-proyectos")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProyectoReadDto>>> GetProyectosPropios()
        {
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var proyectos = await _proyectoService.GetProyectosDeUnaPersonaAsync(idJWT);
            if (!proyectos.EsCorrecto || proyectos.Valor is null) return NotFound(proyectos.MensajeError);

            return Ok(proyectos.Valor.Adapt<IEnumerable<ProyectoReadDto>>());
        }

        // Crear un proyecto.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostProyecto([FromBody] ProyectoWriteDto proyectoDto)
        {
            var validationResult = await _validator.ValidateAsync(proyectoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            int idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var proyectoNuevo = await _proyectoService.PostProyectoAsync(
                proyectoDto.Nombre,
                proyectoDto.Descripcion,
                idJWT
            );
            if (!proyectoNuevo.EsCorrecto || proyectoNuevo.Valor is null) return BadRequest(proyectoNuevo.MensajeError);

            var proyectoNuevoDto = proyectoNuevo.Valor.Adapt<ProyectoReadDto>();
            return CreatedAtAction(nameof(GetProyecto), new {proyectoId = proyectoNuevoDto.Id}, proyectoNuevoDto);
        }

        // Modificar un proyecto
        [HttpPatch("{proyectoId}")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> PatchProyecto(int proyectoId, [FromBody] ProyectoPatchDto proyectoDto)
        {
            var validationResult = await _validatorPatch.ValidateAsync(proyectoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cambios = await _proyectoService.PatchProyectoAsync(
                idJWT,
                proyectoId,
                proyectoDto.Nombre,
                proyectoDto.Descripcion
            );
            if (!cambios.EsCorrecto || cambios.Valor is null) return BadRequest(cambios.MensajeError);

            return Ok(cambios.Valor.Adapt<ProyectoReadDto>());
        }

        // Cambiar propietario de un proyecto
        [HttpPatch("cambio-propietario/{proyectoId}")]
        [Authorize]
        public async Task<ActionResult<ProyectoReadDto>> TransferirPropiedadProyecto(int proyectoId, [FromBody] ProyectoPatchDueñoDto proyectoPatchDueñoDto)
        {
            var validationResult = await _validatorDueño.ValidateAsync(proyectoPatchDueñoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Campo = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
    
            var idJWT = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cambioPropietario = await _proyectoService.PatchDueñoProyectoAsync(
                idJWT,
                proyectoId,
                proyectoPatchDueñoDto.NuevoPropietarioId
            );
            if (!cambioPropietario.EsCorrecto || cambioPropietario.Valor is null) return BadRequest(cambioPropietario.MensajeError);

            return Ok(cambioPropietario.Valor.Adapt<ProyectoReadDto>());
        }
    }
}