using FluentValidation;

using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Validators.Common.EquipmentCategories;

namespace SkiRent.Shared.Validators.EquipmentCategories
{
    public class UpdateEquipmentCategoryRequestValidator : AbstractValidator<UpdateEquipmentCategoryRequest>
    {
        public UpdateEquipmentCategoryRequestValidator()
        {
            RuleFor(request => request.NameAsNonNull).SetValidator(new EquipmentCategoryNameValidator())
                .When(request => request.Name is not null);
        }
    }
}
