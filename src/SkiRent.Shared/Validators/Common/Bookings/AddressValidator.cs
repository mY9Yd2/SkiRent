using FluentValidation;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Common.Bookings
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(address => address.Country)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(address => address.PostalCode)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(address => address.City)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(address => address.StreetAddress)
                .NotEmpty()
                .MaximumLength(60);
        }
    }
}
