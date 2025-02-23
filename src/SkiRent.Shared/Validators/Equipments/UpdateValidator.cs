using FluentValidation;

using SkiRent.Shared.Contracts.Equipments;
using SkiRent.Shared.Validators.Common.Equipments;

namespace SkiRent.Shared.Validators.Equipments
{
    public class UpdateEquipmentRequestValidator : AbstractValidator<UpdateEquipmentRequest>
    {
        public UpdateEquipmentRequestValidator()
        {
            RuleFor(request => request.NameAsNonNull).SetValidator(new NameValidator())
                .When(request => request.Name is not null);

            RuleFor(request => request.DescriptionAsNonNull).SetValidator(new DescriptionValidator())
                .When(request => request.Description is not null);

            RuleFor(request => request.CategoryIdAsNonNull).GreaterThanOrEqualTo(1)
                .When(request => request.CategoryId is not null);

            RuleFor(request => request.PricePerDayAsNonNull).SetValidator(new PricePerDayValidator())
                .When(request => request.PricePerDay is not null);

            RuleFor(request => request.AvailableQuantityAsNonNull).SetValidator(new AvailableQuantityValidator())
                .When(request => request.AvailableQuantity is not null);
        }
    }
}
