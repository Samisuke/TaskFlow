using FluentValidation;
using TaskFlow.Core.Dto.ProyectoUsuario;

namespace TaskFlow.Core.Validations
{
    public class ProyectoUsuarioValidator : AbstractValidator<ProyectoUsuarioWriteDto>
    {
        public ProyectoUsuarioValidator()
        {
            RuleFor(x => x.UsuarioId)
                .GreaterThan(0)
                .WithMessage("El usuario indicado no es válido.");

            RuleFor(x => x.ProyectoId)
                .GreaterThan(0)
                .WithMessage("El usuario indicado no es válido.");

            RuleFor(x => x.Rol)
                .IsInEnum()
                .WithMessage("El rol indicado no es válido.");
        }
    }
}