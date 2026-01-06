using ApiResponse.Poc.Models;
using FluentValidation;

namespace ApiResponse.Poc.Validators;

public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentDtoValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .NotNull()
            .WithMessage("Customer name is required.");

        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Appointment date must be in the future.");
    }
}

