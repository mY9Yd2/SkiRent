using System.Globalization;
using System.Windows.Data;

using SkiRent.Desktop.Utils;

namespace SkiRent.Desktop.Converters
{
    public class DateFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                return CultureFormatHelper.FormatShortDate(date);
            }
            else if (value is DateTimeOffset dateTime)
            {
                return CultureFormatHelper.FormatDateTime(dateTime);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
