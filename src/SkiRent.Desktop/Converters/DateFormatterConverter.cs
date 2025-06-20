﻿using System.Globalization;
using System.Windows.Data;

using SkiRent.Desktop.Utils;

namespace SkiRent.Desktop.Converters
{
    public class DateFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                DateOnly date => CultureFormatHelper.FormatShortDate(date),
                DateTimeOffset dateTime => CultureFormatHelper.FormatDateTime(dateTime),
                _ => value
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
