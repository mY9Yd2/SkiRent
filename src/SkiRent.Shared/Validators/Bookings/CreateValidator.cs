using FluentValidation;

using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Validators.Common.Bookings;

namespace SkiRent.Shared.Validators.Bookings
{
    public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator(TimeProvider timeProvider)
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
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));

            RuleFor(request => request.EndDate)
                .LessThanOrEqualTo(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime.AddMonths(3)))
                .GreaterThanOrEqualTo(request => request.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));

            RuleFor(request => request.SuccessUrl).SetValidator(new UrlValidator());

            RuleFor(request => request.CancelUrl).SetValidator(new UrlValidator());
        }
    }
}
