using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.Utils
{
    public static class BookingStatusHelper
    {
        private static readonly Dictionary<BookingStatusTypes, string> LocalizedStatuses = new()
        {
            { BookingStatusTypes.Pending, "Függőben" },
            { BookingStatusTypes.Paid, "Kifizetve" },
            { BookingStatusTypes.Cancelled, "Törölve" },
            { BookingStatusTypes.Returned, "Visszahozva" }
        };

        private static readonly Dictionary<string, BookingStatusTypes> StatusFromString = new()
        {
            { "Függőben", BookingStatusTypes.Pending },
            { "Kifizetve", BookingStatusTypes.Paid },
            { "Törölve", BookingStatusTypes.Cancelled },
            { "Visszahozva", BookingStatusTypes.Returned }
        };

        public static string GetLocalizedString(BookingStatusTypes status)
        {
            return LocalizedStatuses.TryGetValue(status, out var localizedString)
                ? localizedString
                : status.ToString();
        }

        public static BookingStatusTypes GetStatusFromLocalizedString(string localizedStatus)
        {
            return StatusFromString.TryGetValue(localizedStatus, out var status)
                ? status
                : throw new ArgumentException("Invalid localized status string", nameof(localizedStatus));
        }

        public static IEnumerable<string> GetAllLocalizedStatuses()
        {
            return LocalizedStatuses.Values;
        }
    }
}
