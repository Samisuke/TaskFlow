using FluentValidation;
using TaskFlow.Core.Dto.Comentario;

namespace TaskFlow.Core.Validations
{
    public class ComentarioValidator : AbstractValidator<ComentarioWriteDto>
    {
        public ComentarioValidator()
        {
            RuleFor(x => x.Contenido)
                .MinimumLength(1).WithMessage("El comentario tiene que tener al menos un caracter.")
                .MaximumLength(500).WithMessage("El comentario no peude tener más de quinientos caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.TareaId)
                .GreaterThan(0)
                .WithMessage("El usuario indicado no es válido.");
        }
    }

    public class ComentarioPatchValidator : AbstractValidator<ComentarioPatchDto>
    {
        public ComentarioPatchValidator()
        {
            RuleFor(x => x.Contenido)
                .MinimumLength(1).WithMessage("El comentario tiene que tener al menos un caracter.")
                .MaximumLength(500).WithMessage("El comentario no peude tener más de quinientos caracteres.")
                .When(x => x.Contenido != null);
        }
    }
}