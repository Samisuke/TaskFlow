using FluentValidation;
using TaskFlow.Core.Dto.Usuario;

// Validaciones relacionadas con los usuarios, tanto creación como edición.

namespace TaskFlow.Core.Validations
{
    public class UsuarioValidator : AbstractValidator<UsuarioWriteDto>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Nombre)
                .MinimumLength(3).WithMessage("El nombre no puede tener menos de tres caracteres.")
                .MaximumLength(20).WithMessage("El nombre no puede tener más de veinte caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Apellidos)
                .MinimumLength(3).WithMessage("Los apellidos no pueden tener menos de tres caracteres.")
                .MaximumLength(100).WithMessage("Los apellidos no pueden tener más de cien caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("El correo no puede tener más de cien caracteres.")
                .EmailAddress().WithMessage("El formato del correo no es válido.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("La contraseña tiene que tener al menos 6 caracteres")
                .MaximumLength(100).WithMessage("La contraseña no puede tener más de 100 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe tener al menos una letra mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe tener al menos una letra minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe tener al menos un número.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    }
    public class UsuarioPatchValidator : AbstractValidator<UsuarioPatchDto>
    {   
        public UsuarioPatchValidator()
        {
            RuleFor(x => x.Nombre)
                .MinimumLength(3).WithMessage("El nombre no puede tener menos de tres caracteres.")
                .MaximumLength(20).WithMessage("El nombre no puede tener más de veinte caracteres.")
                .When(x => x.Nombre != null);

            RuleFor(x => x.Apellidos)
                .MinimumLength(3).WithMessage("Los apellidos no pueden tener menos de tres caracteres.")
                .MaximumLength(100).WithMessage("Los apellidos no pueden tener más de cien caracteres.")
                .When(x => x.Apellidos != null);

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("El correo no puede tener más de cien caracteres.")
                .EmailAddress().WithMessage("El formato del correo no es válido.")
                .When(x => x.Email != null);
        }
    }

    public class UsuarioPatchPassValidator : AbstractValidator<UsuarioPassDto>
    {   
        public UsuarioPatchPassValidator()
        {
            RuleFor(x => x.PassNueva)
            .MinimumLength(6).WithMessage("La contraseña tiene que tener al menos 6 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede tener más de 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe tener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe tener al menos una letra minúscula.")
            .Matches(@"[0-9]").WithMessage("La contraseña debe tener al menos un número.")
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");

            RuleFor(x => x.PassAntigua)
                .NotEmpty().WithMessage("Este campo no puede estar vacío.");
        }
    } 
}
