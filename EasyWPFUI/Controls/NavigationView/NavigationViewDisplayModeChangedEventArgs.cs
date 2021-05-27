using System;

namespace EasyWPFUI.Controls
{
    public sealed class NavigationViewDisplayModeChangedEventArgs : EventArgs
    {
        public NavigationViewDisplayMode DisplayMode { get; internal set; }
    }
}
