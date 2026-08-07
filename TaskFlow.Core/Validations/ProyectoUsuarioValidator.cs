using FluentValidation;
using TaskFlow.Core.Dto.ProyectoUsuario;

namespace TaskFlow.Core.Validations
{
    public class ProyectoUsuarioValidator : AbstractValidator<ProyectoUsuarioWriteDto>
    {
        public ProyectoUsuarioValidator()
        {
            RuleFor(x => x.UsuarioId)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.ProyectoId)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Rol)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
}