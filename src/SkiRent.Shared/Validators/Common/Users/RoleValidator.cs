using FluentValidation;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Common.Users
{
    public class RoleValidator : AbstractValidator<RoleTypes>
    {
        public RoleValidator()
        {
            RuleFor(role => role)
                .IsInEnum()
                .NotEqual(RoleTypes.Invalid);
        }
    }
}
