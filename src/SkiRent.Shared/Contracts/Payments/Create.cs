using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Payments
{
    public record CreatePaymentRequest
    {
        public required string MerchantName { get; init; }

        public required IEnumerable<PaymentItem> Items { get; init; }

        public required decimal TotalPrice { get; init; }

        public required string TwoLetterISORegionName { get; init; }

        public required Uri CallbackUrl { get; init; }

        public required Uri SuccessUrl { get; init; }

        public required Uri CancelUrl { get; init; }
    }
}
