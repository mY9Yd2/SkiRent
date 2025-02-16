using FluentValidation;

using SkiRent.Shared.Contracts.Users;
using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.Shared.Validators.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(request => request.EmailAsNonNull).SetValidator(new EmailValidator())
                .When(request => request.Email is not null);

            RuleFor(request => request.PasswordAsNonNull).SetValidator(new PasswordValidator())
                .When(request => request.Password is not null);

            RuleFor(request => request.RoleAsNonNull).SetValidator(new RoleValidator())
                .When(request => request.Role is not null);
        }
    }
}
