using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Bookings
{
    public class UrlValidator : AbstractValidator<Uri>
    {
        public UrlValidator()
        {
            RuleFor(url => url)
                .Must(url => url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);
        }
    }
}
