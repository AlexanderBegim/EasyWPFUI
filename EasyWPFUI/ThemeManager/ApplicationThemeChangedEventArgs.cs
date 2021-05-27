using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI
{
    public class ApplicationThemeChangedEventArgs : EventArgs
    {
        public ApplicationTheme NewTheme { get; internal set; }

        internal ApplicationThemeChangedEventArgs()
        {

        }

        internal ApplicationThemeChangedEventArgs(ApplicationTheme newTheme)
        {
            NewTheme = newTheme;
        }
    }
}
