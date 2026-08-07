using FluentValidation;
using TaskFlow.Core.Dto.Login;

// Validaciones relacionadas con el login.

namespace TaskFlow.Core.Validations
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Introduce un correo válido.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Pass)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
}