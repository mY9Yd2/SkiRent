using FluentValidation;

using SkiRent.Shared.Contracts.Equipments;
using SkiRent.Shared.Validators.Common.Equipments;

namespace SkiRent.Shared.Validators.Equipments
{
    public class CreateEquipmentRequestValidator : AbstractValidator<CreateEquipmentRequest>
    {
        public CreateEquipmentRequestValidator()
        {
            RuleFor(request => request.Name).SetValidator(new NameValidator());

            RuleFor(request => request.DescriptionAsNonNull).SetValidator(new DescriptionValidator())
                .When(request => request.Description is not null);

            RuleFor(request => request.CategoryId).GreaterThanOrEqualTo(1);

            RuleFor(request => request.PricePerDay).SetValidator(new PricePerDayValidator());

            RuleFor(request => request.AvailableQuantity).SetValidator(new AvailableQuantityValidator());

            RuleFor(request => request.MainImageId)
                .NotEqual(Guid.Empty)
                .When(request => request.MainImageId is not null);
        }
    }
}
