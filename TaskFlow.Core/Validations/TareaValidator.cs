using FluentValidation;
using TaskFlow.Core.Dto.Tarea;

namespace TaskFlow.Core.Validations
{
    public class TareaValidator : AbstractValidator<TareaWriteDto>
    {
        public TareaValidator()
        {
            RuleFor(x => x.Titulo)
                .MinimumLength(2).WithMessage("El título no puede tener menos de dos caracteres.")
                .MaximumLength(20).WithMessage("El título no puede tener más de veinte caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Descripcion)
                .MinimumLength(1).WithMessage("La descripción no puede tener menos de un caracter.")
                .MaximumLength(300).WithMessage("La descripción no puede tener más de trescientos caracteres.");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Prioridad)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.FechaLimite)
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("La fecha debe ser en el pasado.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.ProyectoId)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.AsignadoId)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }

    public class TareaPatchValidator : AbstractValidator<TareaPatchDto>
    {
        public TareaPatchValidator()
        {
            RuleFor(x => x.Titulo)
                .MinimumLength(2).WithMessage("El título no puede tener menos de dos caracteres.")
                .MaximumLength(20).WithMessage("El título no puede tener más de veinte caracteres.")
                .When(x => x.Titulo != null);

            RuleFor(x => x.Descripcion)
                .MinimumLength(1).WithMessage("La descripción no puede tener menos de un caracter.")
                .MaximumLength(300).WithMessage("La descripción no puede tener más de trescientos caracteres.")
                .When(x => x.Descripcion != null);

            RuleFor(x => x.FechaLimite)
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("La fecha debe ser en el pasado.")
                .When(x => x.FechaLimite != null);
        }
    }

    public class TareaEstadoPatchValidator : AbstractValidator<TareaEstadoPatchDto>
    {
        public TareaEstadoPatchValidator()
        {
            RuleFor(x => x.IdTarea)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
}