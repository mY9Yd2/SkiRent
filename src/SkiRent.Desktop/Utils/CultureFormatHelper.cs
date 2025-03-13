using System.Globalization;

namespace SkiRent.Desktop.Utils
{
    public static class CultureFormatHelper
    {
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("hu-HU");

        public static string FormatCurrency(decimal value)
        {
            return value.ToString("C0", Culture);
        }

        public static string FormatCurrency(int value)
        {
            return value.ToString("C0", Culture);
        }
    }
}
