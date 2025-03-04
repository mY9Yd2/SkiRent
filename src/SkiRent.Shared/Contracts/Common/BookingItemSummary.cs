namespace SkiRent.Shared.Contracts.Common
{
    public record BookingItemSummary
    {
        public required string Name { get; init; }

        public required int Quantity { get; init; }

        public required decimal PricePerDay { get; init; }

        public required decimal TotalPrice { get; init; }
    }
}
