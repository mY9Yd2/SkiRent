using System.Globalization;
using System.Windows.Data;

using SkiRent.Desktop.Utils;

namespace SkiRent.Desktop.Converters
{
    public class CurrencyFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                decimal currencyDecimal => CultureFormatHelper.FormatCurrency(currencyDecimal),
                int currencyInt => CultureFormatHelper.FormatCurrency(currencyInt),
                _ => value
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
