using FluentValidation;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Common.Bookings
{
    public class EquipmentBookingValidator : AbstractValidator<EquipmentBooking>
    {
        public EquipmentBookingValidator()
        {
            RuleFor(equipmentBooking => equipmentBooking.EquipmentId)
                .GreaterThanOrEqualTo(1);

            RuleFor(equipmentBooking => equipmentBooking.Quantity)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(100);
        }
    }
}
