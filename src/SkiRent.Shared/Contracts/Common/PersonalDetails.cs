namespace SkiRent.Shared.Contracts.Common
{
    public record PersonalDetails
    {
        public required string FullName { get; init; }

        public string? PhoneNumber { get; init; }

        public string? MobilePhoneNumber { get; init; }

        public required Address Address { get; init; }
    }
}
