using FluentValidation;

namespace SkiRent.Shared.Validators.Common.Users
{
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(password => password)
                .NotEmpty()
                .Length(8, 16)
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d\!\?\*\.\@\#\$%^&+=]*$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number. Special characters (!?*.@#$%^&+=) are optional.");
        }
    }
}
