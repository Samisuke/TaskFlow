using FluentValidation;
using TaskFlow.Core.Dto.Proyecto;

namespace TaskFlow.Core.Validations
{
    public class ProyectoValidator : AbstractValidator<ProyectoWriteDto>
    {
        public ProyectoValidator()
        {
            RuleFor(x => x.Nombre)
                .MinimumLength(2).WithMessage("El título no puede tener menos de dos caracteres.")
                .MaximumLength(20).WithMessage("El título no puede tener más de veinte caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Descripcion)
                .MinimumLength(1).WithMessage("La descripción no puede tener menos de un caracter.")
                .MaximumLength(300).WithMessage("La descripción no puede tener más de trescientos caracteres.");
        }
    }

    public class ProyectoPatchValidator : AbstractValidator<ProyectoPatchDto>
    {
        public ProyectoPatchValidator()
        {
            RuleFor(x => x.Nombre)
                .MinimumLength(2).WithMessage("El título no puede tener menos de dos caracteres.")
                .MaximumLength(20).WithMessage("El título no puede tener más de veinte caracteres.")
                .When(x => x.Nombre != null);

            RuleFor(x => x.Descripcion)
                .MinimumLength(1).WithMessage("La descripción no puede tener menos de un caracter.")
                .MaximumLength(300).WithMessage("La descripción no puede tener más de trescientos caracteres.")
                .When(x => x.Descripcion != null);
        }
    }

    public class ProyectoPatchDueñoValidator : AbstractValidator<ProyectoPatchDueñoDto>
    {
        public ProyectoPatchDueñoValidator()
        {
            RuleFor(x => x.NuevoPropietarioId)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
}