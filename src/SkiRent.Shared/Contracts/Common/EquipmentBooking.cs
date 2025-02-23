namespace SkiRent.Shared.Contracts.Common
{
    public record EquipmentBooking
    {
        public required int EquipmentId { get; init; }

        public required int Quantity { get; init; }
    }
}
