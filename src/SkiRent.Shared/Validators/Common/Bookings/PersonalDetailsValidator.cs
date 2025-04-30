using FluentValidation;

using PhoneNumbers;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Validators.Common.Bookings
{
    public class PersonalDetailsValidator : AbstractValidator<PersonalDetails>
    {
        private readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();

        public PersonalDetailsValidator()
        {
            RuleFor(details => details.FullName)
                .NotEmpty()
                .Length(2, 60);

            RuleFor(details => details)
                .Must(details => details.PhoneNumber is not null || details.MobilePhoneNumber is not null)
                .WithMessage("At least one phone number (Phone or Mobile) must be provided.");

            RuleFor(details => details.PhoneNumber)
                .Must(IsValidPhoneNumber)
                .When(details => details.PhoneNumber is not null)
                .WithMessage("Invalid phone number format.");

            RuleFor(details => details.MobilePhoneNumber)
                .Must(IsValidPhoneNumber)
                .When(details => details.MobilePhoneNumber is not null)
                .WithMessage("Invalid mobile phone number format.");

            RuleFor(details => details.Address).SetValidator(new AddressValidator());
        }

        private bool IsValidPhoneNumber(string? phoneNumber)
        {
            return IsValidPhoneNumber(phoneNumber ?? string.Empty, null);
        }

        private bool IsValidPhoneNumber(string phoneNumber, string? region)
        {
            try
            {
                var parsedNumber = _phoneNumberUtil.Parse(phoneNumber, region);
                return _phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                if (region is null)
                {
                    return IsValidPhoneNumber(phoneNumber, "HU");
                }
                return false;
            }
        }
    }
}
