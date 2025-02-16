using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EquipmentDescriptionValidator : AbstractValidator<string?>
    {
        public EquipmentDescriptionValidator()
        {
            RuleFor(description => description)
                .NotEmpty()
                .MaximumLength(4000);
        }
    }
}
