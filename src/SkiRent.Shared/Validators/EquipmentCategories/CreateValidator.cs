﻿using FluentValidation;

using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Validators.Common.EquipmentCategories;

namespace SkiRent.Shared.Validators.EquipmentCategories
{
    public class CreateEquipmentCategoryRequestValidator : AbstractValidator<CreateEquipmentCategoryRequest>
    {
        public CreateEquipmentCategoryRequestValidator()
        {
            RuleFor(request => request.Name).SetValidator(new EquipmentCategoryNameValidator());
        }
    }
}
