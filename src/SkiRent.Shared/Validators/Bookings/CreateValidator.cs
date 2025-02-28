using FluentValidation;

using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Validators.Common.Bookings;

namespace SkiRent.Shared.Validators.Bookings
{
    public class CreateValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateValidator()
        {
            RuleFor(request => request.PersonalDetails).SetValidator(new PersonalDetailsValidator());

            RuleFor(request => request.Equipments)
                .NotEmpty()
                .Must(equipments => equipments.Select(
                    equipment => equipment.EquipmentId).Distinct().Count() == equipments.Count())
                .WithMessage("Equipment Ids must be unique.");

            RuleForEach(request => request.Equipments)
                .SetValidator(new EquipmentBookingValidator());

            RuleFor(request => request.StartDate)
                .LessThanOrEqualTo(request => request.EndDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime));

            RuleFor(request => request.EndDate)
                .LessThanOrEqualTo(DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime.AddMonths(3)))
                .GreaterThanOrEqualTo(request => request.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime));

            RuleFor(request => request.SuccessUrl).SetValidator(new UrlValidator());

            RuleFor(request => request.CancelUrl).SetValidator(new UrlValidator());
        }
    }
}
