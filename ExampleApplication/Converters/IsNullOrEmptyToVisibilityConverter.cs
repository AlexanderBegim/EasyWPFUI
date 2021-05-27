using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExampleApplication.Converters
{
    public class IsNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public bool IsInvert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = false;

            if (value != null)
            {
                if (value is string str)
                {
                    isVisible = !string.IsNullOrEmpty(str);
                }
                else
                {
                    isVisible = true;
                }
            }
            else
            {
                isVisible = false;
            }

            if (IsInvert)
            {
                isVisible = !isVisible;
            }

            return (isVisible) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
