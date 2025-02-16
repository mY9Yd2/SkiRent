using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Equipments
{
    public class CategoryIdValidator : AbstractValidator<int>
    {
        public CategoryIdValidator()
        {
            RuleFor(categoryId => categoryId)
                .GreaterThan(0);
        }
    }
}
