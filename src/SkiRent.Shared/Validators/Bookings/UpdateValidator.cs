using FluentValidation;

using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Bookings
{
    public class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequest>
    {
        public UpdateBookingRequestValidator()
        {
            RuleFor(request => request.Status)
                .IsInEnum()
                .Must(status => status == BookingStatusTypes.InDelivery
                    || status == BookingStatusTypes.Received
                    || status == BookingStatusTypes.Returned)
                .WithMessage("Invalid status. Only 'InDelivery', 'Received', or 'Returned' are allowed.");
        }
    }
}
