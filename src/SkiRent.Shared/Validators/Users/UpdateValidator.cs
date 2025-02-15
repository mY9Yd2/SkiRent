using FluentValidation;

using SkiRent.Shared.Contracts.Users;
using SkiRent.Shared.Validators.Common;

namespace SkiRent.Shared.Validators.Users
{
    public class UpdateValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateValidator()
        {
            RuleFor(request => request.Email).SetValidator(new EmailValidator());
            RuleFor(request => request.Password).SetValidator(new PasswordValidator());
            RuleFor(request => request.Role)
                .IsInEnum()
                .When(request => request.Role is not null);
        }
    }
}
