using Caliburn.Micro;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Converters
{
    /// <summary>
    /// Workaround for icons. Allows input both via binding or parameter, e.g. "{Binding Convert={StaticResource foo}, ConverterParameter=delete.png}" will insert delete.png and return "delete.png" for light theme and "delete-inverted.png" for dark theme.
    /// UWP provides magic black -> white conversion for icons when app theme changes.
    /// This allows creating black icon and automagically having black icon in light theme and white icon on dark theme.
    /// BUT IT ONLY WORKS IN APP BAR (doesn't work for buttons).
    /// Android does not allow this at all, so this helper will determine current theme and then return "icon-name" or "icon-name-inverted" for the alternative icon.
    /// </summary>
    public class IconNameColorByThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iconName;
            if (value is string a)
            {
                iconName = a;
            }
            else if (parameter is string b)
            {
                iconName = b;
            }
            else
            {
                throw new NotSupportedException();
            }
            var theme = IoC.Get<IThemeProvider>();
            // icons are black, so for dark theme we need to invert them to white icons which by convention are "<name>-inverted"
            if (theme.IsDarkTheme)
            {
                var fn = iconName;
                string ext = "";
                if (fn.Contains("."))
                {
                    var idx = iconName.LastIndexOf('.');
                    fn = iconName.Substring(0, idx);
                    ext = iconName.Substring(idx);
                }
                return $"{fn}_inverted{ext}";
            }
            return iconName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}