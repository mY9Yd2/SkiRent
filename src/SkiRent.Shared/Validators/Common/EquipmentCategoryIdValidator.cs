using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EquipmentCategoryIdValidator : AbstractValidator<int>
    {
        public EquipmentCategoryIdValidator()
        {
            RuleFor(categoryId => categoryId)
                .GreaterThan(0);
        }
    }
}
