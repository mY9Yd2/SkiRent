using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EquipmentPricePerDayValidator : AbstractValidator<decimal>
    {
        public EquipmentPricePerDayValidator()
        {
            RuleFor(pricePerDay => pricePerDay)
                .GreaterThanOrEqualTo(0);
        }
    }
}
