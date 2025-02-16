using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Equipments
{
    public class DescriptionValidator : AbstractValidator<string>
    {
        public DescriptionValidator()
        {
            RuleFor(description => description)
                .NotNull()
                .MaximumLength(4000);
        }
    }
}
