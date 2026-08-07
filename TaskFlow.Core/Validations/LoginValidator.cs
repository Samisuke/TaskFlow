using FluentValidation;
using TaskFlow.Core.Dto.Login;

namespace TaskFlow.Core.Validations
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Pass)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
}