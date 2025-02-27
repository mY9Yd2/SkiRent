namespace SkiRent.Shared.Contracts.Common
{
    public record Address
    {
        public required string Country { get; init; }

        public required string PostalCode { get; init; }

        public required string City { get; init; }

        public required string StreetAddress { get; init; }
    }
}
