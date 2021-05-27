using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public enum NavigationViewVisualStateDisplayMode
    {
        Compact,
        Expanded,
        Minimal,
        MinimalWithBackButton
    }

    public enum NavigationViewRepeaterPosition
    {
        LeftNav,
        TopPrimary,
        TopOverflow,
        LeftFooter,
        TopFooter
    }

    public enum NavigationViewPropagateTarget
    {
        LeftListView,
        TopListView,
        OverflowListView,
        All
    }

    public class NavigationViewItemHelper
    {
        public const string c_OnLeftNavigationReveal = "OnLeftNavigationReveal";
        public const string c_OnLeftNavigation = "OnLeftNavigation";
        public const string c_OnTopNavigationPrimary = "OnTopNavigationPrimary";
        public const string c_OnTopNavigationPrimaryReveal = "OnTopNavigationPrimaryReveal";
        public const string c_OnTopNavigationOverflow = "OnTopNavigationOverflow";
    }

    public class NavigationViewItemHelper<T> where T : Control
    {
        public NavigationViewItemHelper()
        {

        }

        public UIElement GetSelectionIndicator()
        {
            return m_selectionIndicator;
        }

        public void Init(T control)
        {
            m_selectionIndicator = control.Template?.FindName(c_selectionIndicatorName, control) as UIElement; // TODO Проверить работоспособность этого кода.
        }


        private const string c_selectionIndicatorName = "SelectionIndicator";

        private UIElement m_selectionIndicator;
    }
}
