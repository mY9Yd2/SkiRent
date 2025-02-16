using FluentValidation;

using SkiRent.Shared.Contracts.Equipments;
using SkiRent.Shared.Validators.Common;

namespace SkiRent.Shared.Validators.Equipments
{
    public class CreateEquipmentRequestValidator : AbstractValidator<CreateEquipmentRequest>
    {
        public CreateEquipmentRequestValidator()
        {
            RuleFor(request => request.Name).SetValidator(new EquipmentNameValidator());
            RuleFor(request => request.Description)
                .SetValidator(new EquipmentDescriptionValidator())
                .When(request => request.Description is not null);
            RuleFor(request => request.CategoryId).SetValidator(new EquipmentCategoryIdValidator());
            RuleFor(request => request.PricePerDay).SetValidator(new EquipmentPricePerDayValidator());
            RuleFor(request => request.AvailableQuantity).SetValidator(new EquipmentAvailableQuantityValidator());
        }
    }
}
