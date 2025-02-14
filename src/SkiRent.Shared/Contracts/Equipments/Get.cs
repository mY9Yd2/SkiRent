namespace SkiRent.Shared.Contracts.Equipments
{
    public record GetEquipmentResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required string? Description { get; init; }

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }

        public required int AvailableQuantity { get; init; }
    }
}
