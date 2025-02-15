namespace SkiRent.Shared.Contracts.Equipments
{
    public record GetAllEquipmentResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }

        public required bool IsAvaiable { get; init; }
    }
}
