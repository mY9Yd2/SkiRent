using System.Globalization;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Invoices
{
    public record CreateInvoiceRequest
    {
        public required Guid PaymentId { get; init; }

        public required PersonalDetails PersonalDetails { get; init; }

        public required IEnumerable<PaymentItem> Items { get; init; }

        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

        public required DateOnly StartDate { get; init; }

        public required DateOnly EndDate { get; init; }

        public required string MerchantName { get; init; }

        public required CultureInfo Culture { get; init; }
    }
}
