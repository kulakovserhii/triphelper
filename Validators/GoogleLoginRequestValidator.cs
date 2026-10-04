using AuthService.Contracts;
using FluentValidation;

namespace AuthService.Validators
{
    public class GoogleLoginRequestValidator: AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken).NotEmpty().WithMessage("ID token is required");
        }
    }
}