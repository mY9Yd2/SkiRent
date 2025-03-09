namespace SkiRent.Shared.Contracts.Equipments
{
    public record GetAllEquipmentResponse
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public string? Description { get; init; } // Hozzáadva (azért kérdőjel, mert lehet, hogy null az értéke)

        public required int CategoryId { get; init; }

        public required decimal PricePerDay { get; init; }

        public int AvailableQuantity { get; init; } // Hozzáadva a lekérdezés értékeinek visszaadására.

        public required bool IsAvailable { get; init; }
    }
}
