using FluentValidation;

namespace SkiRent.Shared.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<string, string> NoLeadingOrTrailingWhitespace(this IRuleBuilderOptions<string, string> builder)
        {
            return builder
                .NoLeadingWhitespace()
                .NoTrailingWhitespace();
        }

        public static IRuleBuilderOptions<string, string> NoLeadingWhitespace(this IRuleBuilderOptions<string, string> builder)
        {
            return builder
                .Must(str => str is null || str.Length == 0 || !char.IsWhiteSpace(str.First()))
                .WithMessage("'{PropertyName}' must not start with a whitespace character.");
        }

        public static IRuleBuilderOptions<string, string> NoTrailingWhitespace(this IRuleBuilderOptions<string, string> builder)
        {
            return builder
                .Must(str => str is null || str.Length == 0 || !char.IsWhiteSpace(str.Last()))
                .WithMessage("'{PropertyName}' must not end with a whitespace character.");
        }

        public static IRuleBuilderOptions<string, string> NoWhitespace(this IRuleBuilderOptions<string, string> builder)
        {
            return builder
                .Must(str => string.IsNullOrWhiteSpace(str) || !str.Any(char.IsWhiteSpace))
                .WithMessage("'{PropertyName}' must not contain any whitespace characters.");
        }
    }
}
