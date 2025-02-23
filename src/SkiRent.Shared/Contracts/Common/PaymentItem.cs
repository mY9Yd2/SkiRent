namespace SkiRent.Shared.Contracts.Common
{
    public record PaymentItem
    {
        public required string Name { get; init; }

        public required decimal Price { get; init; }

        public required int Quantity { get; init; }
    }
}
