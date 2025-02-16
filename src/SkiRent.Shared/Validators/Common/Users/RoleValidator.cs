using FluentValidation;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Common.Users
{
    public class RoleValidator : AbstractValidator<Roles>
    {
        public RoleValidator()
        {
            RuleFor(role => role)
                .IsInEnum()
                .NotEqual(Roles.Invalid);
        }
    }
}
