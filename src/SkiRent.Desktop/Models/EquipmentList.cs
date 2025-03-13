namespace SkiRent.Desktop.Models
{
    public record EquipmentList
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required int CategoryId { get; init; }

        public required string CategoryName { get; init; }

        public required decimal PricePerDay { get; init; }

        public required int AvailableQuantity { get; init; }

        public bool IsAvailable => AvailableQuantity > 0;
    }
}
