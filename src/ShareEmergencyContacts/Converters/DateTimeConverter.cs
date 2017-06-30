using System;
using System.Globalization;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is DateTime)
            {
                var d = (DateTime)value;
                if (parameter == null)
                    return d.ToString();
                var str = (string)parameter;
                return d.ToString(str);
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}