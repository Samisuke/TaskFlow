using FluentValidation;
using GimnasioFit.Api.Dtos;

namespace GimnasioFit.Api.Validators
{
    public class ClaseValidator : AbstractValidator<ClaseCreateDto>
    {
        public ClaseValidator()
        {
            RuleFor (x => x.Nombre)
                .MinimumLength(3).WithMessage("El nombre debe tener mas de tres caracteres.")
                .MaximumLength(15).WithMessage("El nombre debe tener menos de quince caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacio.");

            RuleFor (x => x.Descripcion)
                .MaximumLength(100).WithMessage("La descripcion debe tener menos de cien caracteres.")
                .NotEmpty().WithMessage("Este campo no puede estar vacio.");

            RuleFor (x => x.CapacidadMaxima)
                .GreaterThan(0).WithMessage("La capacidad maxima no puede ser cero o menor.")
                .LessThanOrEqualTo(40).WithMessage("La capacidad maxima no puede ser mayor de 40.")
                .NotEmpty().WithMessage("Este campo no puede estar vacio.");
            
            RuleFor(x => x.ProfesorId)
                .NotEmpty().WithMessage("Este campo no puede estar vacio.");
        }
    }
}