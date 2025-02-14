using FluentValidation;

namespace SkiRent.Shared.Validators.Common
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty()
                .MaximumLength(100)
                .EmailAddress();
        }
    }
}
