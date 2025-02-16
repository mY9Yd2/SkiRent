using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EquipmentNameValidator : AbstractValidator<string>
    {
        public EquipmentNameValidator()
        {
            RuleFor(name => name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
