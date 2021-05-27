using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI
{
    public class ElementThemeChangedEventArgs : EventArgs
    {
        public FrameworkElement Element { get; internal set; }

        public ElementTheme Theme { get; internal set; }

        internal ElementThemeChangedEventArgs()
        {

        }

        internal ElementThemeChangedEventArgs(FrameworkElement element, ElementTheme theme)
        {
            Element = element;

            Theme = theme;
        }
    }
}
