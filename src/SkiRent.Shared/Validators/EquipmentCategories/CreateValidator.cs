using FluentValidation;

using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Validators.Common.EquipmentCategories;

namespace SkiRent.Shared.Validators.EquipmentCategories
{
    public class CreateEquipmentCategoryValidator : AbstractValidator<CreateEquipmentCategoryRequest>
    {
        public CreateEquipmentCategoryValidator()
        {
            RuleFor(request => request.Name).SetValidator(new EquipmentCategoryNameValidator());
        }
    }
}
