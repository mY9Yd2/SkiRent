using System.Globalization;
using System.Windows.Data;

using SkiRent.Desktop.Utils;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.Converters
{
    public class UserRoleFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RoleTypes userRole)
            {
                return UserRoleHelper.GetLocalizedString(userRole);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
