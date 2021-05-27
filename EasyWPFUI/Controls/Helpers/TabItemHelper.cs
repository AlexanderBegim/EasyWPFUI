using System;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls.Helpers
{
    public class TabItemHelper
    {
        private static readonly DependencyProperty IsButtonClickSubscribedProperty = DependencyProperty.RegisterAttached("IsButtonClickSubscribed", typeof(bool), typeof(TabItemHelper), new FrameworkPropertyMetadata());
        
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(TabItemHelper), new FrameworkPropertyMetadata(OnIsHelperEnabledPropertyChanged));

        public static bool GetIsHelperEnabled(UIElement ui)
        {
            return (bool)ui.GetValue(IsHelperEnabledProperty);
        }

        public static void SetIsHelperEnabled(UIElement ui, bool value)
        {
            ui.SetValue(IsHelperEnabledProperty, value);
        }

        private static void OnIsHelperEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;

            if (tabItem == null)
                return;

            if (tabItem.IsLoaded)
            {
                CloseButtonClickSubscribe(tabItem, (bool)e.NewValue);
            }
            else
            {
                tabItem.Loaded += OnTabItemLoaded;
            }
        }

        #endregion

        #region IconElement Property

        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(IconElement), typeof(TabItemHelper), new FrameworkPropertyMetadata());

        public static IconElement GetIcon(UIElement ui)
        {
            return (IconElement)ui.GetValue(IconProperty);
        }

        public static void SetIcon(UIElement ui, IconElement value)
        {
            ui.SetValue(IconProperty, value);
        }

        #endregion

        #region IsClosable Property

        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.RegisterAttached("IsClosable", typeof(bool), typeof(TabItemHelper), new FrameworkPropertyMetadata(OnIsClosablePropertyChanged));

        public static bool GetIsClosable(UIElement ui)
        {
            return (bool)ui.GetValue(IsClosableProperty);
        }

        public static void SetIsClosable(UIElement ui, bool value)
        {
            ui.SetValue(IsClosableProperty, value);
        }

        private static void OnIsClosablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;
            if(tabItem == null)
            {
                return;
            }

            if((bool)e.NewValue && !(bool)tabItem.GetValue(IsButtonClickSubscribedProperty))
            {
                CloseButtonClickSubscribe((TabItem)d, true);
            }
        }

        #endregion


        #region Methods

        private static void OnTabItemLoaded(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)sender;
            TabControl control = tabItem.FindAscendant<TabControl>();

            bool isTabsClosable = (bool)control.GetValue(TabControlHelper.IsCloseTabButtonVisibleProperty);

            if(!isTabsClosable)
            {
                isTabsClosable = (bool)tabItem.GetValue(IsClosableProperty);
            }

            if(isTabsClosable)
            {
                CloseButtonClickSubscribe(tabItem, isTabsClosable);
            }
        }

        private static void CloseButtonClickSubscribe(TabItem tabItem, bool isClosable)
        {
            Button closeButton = tabItem.Template?.FindName("CloseButton", tabItem) as Button;

            if (closeButton != null && isClosable)
            {
                closeButton.Click += OnCloseButtonClick;
                tabItem.SetValue(IsButtonClickSubscribedProperty, true);
            }
            else if (closeButton != null)
            {
                closeButton.Click -= OnCloseButtonClick;
                tabItem.SetValue(IsButtonClickSubscribedProperty, false);
            }

            tabItem.SetValue(IsClosableProperty, isClosable);
        }

        private static void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = ((Button)sender).FindAscendant<TabItem>();

            TabControl tabControl = tabItem?.FindAscendant<TabControl>();

            TabControlHelper.OnTabCloseRequested(tabControl, tabItem);
        }

        #endregion
    }
}
