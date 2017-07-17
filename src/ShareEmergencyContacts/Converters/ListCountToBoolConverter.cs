using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Converters
{
    /// <summary>
    /// Converts to true if the provided object is a <see cref="ICollection"/> with a count greator 0, false if count is 0 or value is null.
    /// Using the parameter "invert" the returned values are flipped.
    /// Otherwise throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class ListCountToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "invert")
                return !(bool)Convert(value, targetType, null, culture);

            if (value == null)
                return false;
            var collection = value as ICollection;
            if (collection != null)
                return collection.Count > 0;

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}