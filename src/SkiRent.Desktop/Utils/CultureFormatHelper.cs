using System.Globalization;

namespace SkiRent.Desktop.Utils
{
    public static class CultureFormatHelper
    {
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("hu-HU");
        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public static string FormatCurrency(decimal value)
        {
            return value.ToString("C0", Culture);
        }

        public static string FormatCurrency(int value)
        {
            return value.ToString("C0", Culture);
        }

        public static string FormatShortDate(DateOnly value)
        {
            return value.ToString("yyyy. MM. dd.", Culture);
        }

        public static string FormatDateTime(DateTimeOffset value)
        {
            return TimeZoneInfo.ConvertTime(value, TimeZone).ToString("yyyy. MM. dd. HH:mm", Culture);
        }
    }
}
