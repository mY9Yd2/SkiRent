using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Equipments
{
    public class PricePerDayValidator : AbstractValidator<decimal>
    {
        public PricePerDayValidator()
        {
            RuleFor(pricePerDay => pricePerDay)
                .GreaterThanOrEqualTo(0);
        }
    }
}
