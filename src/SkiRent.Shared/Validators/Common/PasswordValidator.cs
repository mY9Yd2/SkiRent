using FluentValidation;

using SkiRent.Shared.Extensions;

namespace SkiRent.Shared.Validators.Common
{
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(password => password)
                .NotEmpty()
                .NoWhitespace()
                .Length(6, 16);
        }
    }
}
