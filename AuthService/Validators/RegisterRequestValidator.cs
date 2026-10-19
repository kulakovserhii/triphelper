using AuthService.Contracts;
using FluentValidation;

namespace AuthService.Validators
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Incorrect email format");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 symbols length")
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number");
        }
    }
}
