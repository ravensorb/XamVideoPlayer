using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xam.Plugins.VideoPlayer
{
    public class IntToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return new TimeSpan(0, 0, (int) value);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                return ((TimeSpan)value).TotalSeconds;
            }

            return null;
        }
    }
}
