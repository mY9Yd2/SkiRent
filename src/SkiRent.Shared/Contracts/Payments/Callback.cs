namespace SkiRent.Shared.Contracts.Payments
{
    public record PaymentResult
    {
        public required Guid PaymentId { get; init; }

        public required bool IsSuccessful { get; init; }

        public required string Message { get; init; }
    }
}
