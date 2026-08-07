using FluentValidation;
using TaskFlow.Core.Dto.Tarea;

// Validaciones relacionadas con las tareas, tanto creación como edición.

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
                .IsInEnum()
                .WithMessage("El estado indicado no es válido.");

            RuleFor(x => x.Prioridad)
                .IsInEnum()
                .WithMessage("La prioridad indicada no es válido.");

            RuleFor(x => x.FechaLimite)
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("La fecha debe ser en el futuro.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.ProyectoId)
                .GreaterThan(0)
                .WithMessage("El usuario indicado no es válido.");

            RuleFor(x => x.AsignadoId)
                .GreaterThan(0)
                .WithMessage("El usuario indicado no es válido.");
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
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("La fecha debe ser en el futuro.")
                .When(x => x.FechaLimite != null);
        }
    }

    public class TareaEstadoPatchValidator : AbstractValidator<TareaEstadoPatchDto>
    {
        public TareaEstadoPatchValidator()
        {
            RuleFor(x => x.Estado)
                .IsInEnum()
                .WithMessage("El estado indicado no es válido.");
        }
    }
}