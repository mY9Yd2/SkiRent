namespace SkiRent.Shared.Contracts.Equipments
{
    public record GetAllEquipmentResponse
    {
        public required string Name { get; init; }

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }
    }
}
