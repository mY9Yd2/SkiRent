using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Equipments
{
    public class AvailableQuantityValidator : AbstractValidator<int>
    {
        public AvailableQuantityValidator()
        {
            RuleFor(availableQuantity => availableQuantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}
