using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EquipmentAvailableQuantityValidator : AbstractValidator<int>
    {
        public EquipmentAvailableQuantityValidator()
        {
            RuleFor(availableQuantity => availableQuantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}
