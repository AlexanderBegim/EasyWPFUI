using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI
{
    public class ApplicationAccentColorChangedEventArgs : EventArgs
    {
        public Color OldValue { get; internal set; }

        public Color NewValue { get; internal set; }

        internal ApplicationAccentColorChangedEventArgs()
        {

        }

        internal ApplicationAccentColorChangedEventArgs(Color oldValue, Color newValue)
        {
            OldValue = oldValue;

            NewValue = newValue;
        }
    }
}
