namespace SkiRent.Shared.Contracts.Common
{
    public record PaymentItem
    {
        public required string Name { get; init; }

        public required string SubText { get; init; }

        public required decimal TotalPrice { get; init; }
    }
}
