using FluentValidation;

namespace SkiRent.Shared.Validators.Common.EquipmentCategories
{
    public class EquipmentCategoryNameValidator : AbstractValidator<string>
    {
        public EquipmentCategoryNameValidator()
        {
            RuleFor(name => name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
