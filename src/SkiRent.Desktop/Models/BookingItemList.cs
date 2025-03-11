namespace SkiRent.Desktop.Models
{
    public record BookingItemList
    {
        public required string Name { get; init; }

        public required int Quantity { get; init; }

        public required string PricePerDay { get; init; }

        public required string TotalPrice { get; init; }
    }
}
