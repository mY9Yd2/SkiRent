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
                .Equal(BookingStatusTypes.Returned);
        }
    }
}
