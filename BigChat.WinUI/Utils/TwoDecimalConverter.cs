using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace BigChat.Converters;

internal sealed partial class TwoDecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double d)
        {
            // Format to 2 decimal places
            return d.ToString("F2", CultureInfo.InvariantCulture);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is string s && double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : value;
    }
}
