using System;

namespace EasyWPFUI.Controls
{
    public sealed class NavigationViewPaneClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }

        internal void SplitViewClosingArgs(SplitViewPaneClosingEventArgs value)
        {

        }
    }
}
