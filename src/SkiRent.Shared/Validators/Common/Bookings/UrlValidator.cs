using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Bookings
{
    public class UrlValidator : AbstractValidator<Uri>
    {
        public UrlValidator()
        {
            RuleFor(url => url)
                .Must(url => url.IsAbsoluteUri
                    && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps))
                .WithMessage("URL must be an absolute HTTP or HTTPS URL.");
        }
    }
}
