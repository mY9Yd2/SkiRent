using FluentValidation;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.Shared.Validators.Auth
{
    public class SignInRequestValidator : AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(request => request.Email).SetValidator(new EmailValidator());

            RuleFor(request => request.Password).SetValidator(new PasswordValidator());
        }
    }
}
