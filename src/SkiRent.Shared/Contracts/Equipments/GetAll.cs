namespace SkiRent.Shared.Contracts.Equipments
{
    public record GetAllEquipmentResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string? Description { get; init; }

        public required int CategoryId { get; init; }

        public required string CategoryName { get; init; }

        public required decimal PricePerDay { get; init; }

        public required int AvailableQuantity { get; init; }

        public required Guid? MainImageId { get; init; }

        public string MainImageUrl { get; init; } = string.Empty;

        public bool IsAvailable => AvailableQuantity > 0;
    }
}
