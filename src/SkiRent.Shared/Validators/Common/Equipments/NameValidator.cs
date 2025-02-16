using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Equipments
{
    public class NameValidator : AbstractValidator<string>
    {
        public NameValidator()
        {
            RuleFor(name => name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
