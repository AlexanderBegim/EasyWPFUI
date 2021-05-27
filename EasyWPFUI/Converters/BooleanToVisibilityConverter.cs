using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace EasyWPFUI.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInvert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!IsInvert)
            {
                return (bool)value == true ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return (bool)value == true ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
