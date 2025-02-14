using FluentValidation;

using SkiRent.Shared.Contracts.Users;
using SkiRent.Shared.Validators.Common;

namespace SkiRent.Shared.Validators.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(request => request.Email).SetValidator(new EmailValidator());
            RuleFor(request => request.Password).SetValidator(new PasswordValidator());
        }
    }
}
