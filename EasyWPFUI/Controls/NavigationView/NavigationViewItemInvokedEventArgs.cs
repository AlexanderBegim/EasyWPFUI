using EasyWPFUI.Media.Animations;
using System;

namespace EasyWPFUI.Controls
{
    public sealed class NavigationViewItemInvokedEventArgs : EventArgs
    {
        public object InvokedItem { get; internal set; }

        public NavigationViewItemBase InvokedItemContainer { get; internal set; }

        public bool IsSettingsInvoked { get; internal set; }

        public NavigationTransitionInfo RecommendedNavigationTransitionInfo { get; internal set; }
    }
}
