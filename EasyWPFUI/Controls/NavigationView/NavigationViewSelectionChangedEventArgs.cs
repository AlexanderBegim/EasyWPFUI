using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWPFUI.Media.Animations;

namespace EasyWPFUI.Controls
{
    public class NavigationViewSelectionChangedEventArgs : EventArgs
    {
        public object SelectedItem { get; internal set; }

        public bool IsSettingsSelected { get; internal set; }

        public NavigationViewItemBase SelectedItemContainer { get; internal set; }

        public NavigationTransitionInfo RecommendedNavigationTransitionInfo { get; internal set; }
    }
}
