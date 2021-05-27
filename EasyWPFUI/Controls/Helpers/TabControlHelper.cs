using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls.Helpers
{
    public class TabItemTabCloseRequestedEventArgs : RoutedEventArgs
    {
        public object Item { get; internal set; }

        public TabItem Tab { get; internal set; }

        internal TabItemTabCloseRequestedEventArgs()
        {
            
        }

        internal TabItemTabCloseRequestedEventArgs(RoutedEvent routedEvent, TabItem tab, object item)
        {
            RoutedEvent = routedEvent;

            Tab = tab;

            Item = item;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            TypedEventHandler<TabControl, TabItemTabCloseRequestedEventArgs> handler = (TypedEventHandler<TabControl, TabItemTabCloseRequestedEventArgs>)genericHandler;

            handler((TabControl)genericTarget, this);
        }
    }

    public enum TabControlCloseButtonOverlayMode
    {
        Auto,
        OnPointerOver,
        Always
    }

    public static class TabControlHelper
    {
        #region Events

        public static RoutedEvent AddTabButtonClickEvent = EventManager.RegisterRoutedEvent("AddTabButtonClick", RoutingStrategy.Bubble, typeof(TypedEventHandler<TabControl, RoutedEventArgs>), typeof(TabControlHelper));

        public static void AddAddTabButtonClickHandler(DependencyObject d, TypedEventHandler<TabControl, RoutedEventArgs> handler)
        {
            UIElement element = d as UIElement;

            if(element != null)
            {
                element.AddHandler(TabControlHelper.AddTabButtonClickEvent, handler);
            }
        }

        public static void RemoveAddTabButtonClickHandler(DependencyObject d, TypedEventHandler<TabControl, RoutedEventArgs> handler)
        {
            UIElement element = d as UIElement;

            if (element != null)
            {
                element.RemoveHandler(TabControlHelper.AddTabButtonClickEvent, handler);
            }
        }

        public static RoutedEvent TabCloseRequestedEvent = EventManager.RegisterRoutedEvent("TabCloseRequested", RoutingStrategy.Bubble, typeof(TypedEventHandler<TabControl, TabItemTabCloseRequestedEventArgs>), typeof(TabControlHelper));

        public static void AddTabCloseRequestedHandler(DependencyObject d, TypedEventHandler<TabControl, TabItemTabCloseRequestedEventArgs> handler)
        {
            UIElement element = d as UIElement;

            if (element != null)
            {
                element.AddHandler(TabControlHelper.TabCloseRequestedEvent, handler);
            }
        }

        public static void RemoveTabCloseRequestedHandler(DependencyObject d, TypedEventHandler<TabControl, TabItemTabCloseRequestedEventArgs> handler)
        {
            UIElement element = d as UIElement;

            if (element != null)
            {
                element.RemoveHandler(TabControlHelper.TabCloseRequestedEvent, handler);
            }
        }

        #endregion

        #region TabStripHeader Property

        public static readonly DependencyProperty TabStripHeaderProperty = DependencyProperty.RegisterAttached("TabStripHeader", typeof(object), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static object GetTabStripHeader(UIElement ui)
        {
            return ui.GetValue(TabStripHeaderProperty);
        }

        public static void SetTabStripHeader(UIElement ui, object value)
        {
            ui.SetValue(TabStripHeaderProperty, value);
        }

        #endregion

        #region TabStripHeaderTemplate Property

        public static readonly DependencyProperty TabStripHeaderTemplateProperty = DependencyProperty.RegisterAttached("TabStripHeaderTemplate", typeof(DataTemplate), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetTabStripHeaderTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(TabStripHeaderTemplateProperty);
        }

        public static void SetTabStripHeaderTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(TabStripHeaderTemplateProperty, value);
        }

        #endregion

        #region TabStripFooter Property

        public static readonly DependencyProperty TabStripFooterProperty = DependencyProperty.RegisterAttached("TabStripFooter", typeof(object), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static object GetTabStripFooter(UIElement ui)
        {
            return ui.GetValue(TabStripFooterProperty);
        }

        public static void SetTabStripFooter(UIElement ui, object value)
        {
            ui.SetValue(TabStripFooterProperty, value);
        }

        #endregion

        #region TabStripFooterTemplate Property

        public static readonly DependencyProperty TabStripFooterTemplateProperty = DependencyProperty.RegisterAttached("TabStripFooterTemplate", typeof(DataTemplate), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetTabStripFooterTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(TabStripFooterTemplateProperty);
        }

        public static void SetTabStripFooterTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(TabStripFooterTemplateProperty, value);
        }

        #endregion

        /*
         * Удалить код ниже если функция создания вкладок не будет реализована
         */

        #region IsAddTabButtonVisible Property

        private static readonly DependencyProperty IsAddTabButtonVisibleProperty = DependencyProperty.RegisterAttached("IsAddTabButtonVisible", typeof(bool), typeof(TabControlHelper), new FrameworkPropertyMetadata(OnIsAddTabButtonVisiblePropertyChanged));

        public static bool GetIsAddTabButtonVisible(UIElement ui)
        {
            return (bool)ui.GetValue(IsAddTabButtonVisibleProperty);
        }

        public static void SetIsAddTabButtonVisible(UIElement ui, bool value)
        {
            ui.SetValue(IsAddTabButtonVisibleProperty, value);
        }

        private static void OnIsAddTabButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tabControl = d as TabControl;

            if (tabControl == null)
                return;

            if (tabControl.IsLoaded)
            {
                AddButtonClickSubscribe(tabControl, (bool)e.NewValue);
            }
            else
            {
                tabControl.Loaded += OnTabControlLoaded;
            }
        }

        #endregion

        #region IsCloseTabButtonVisible Property

        public static readonly DependencyProperty IsCloseTabButtonVisibleProperty = DependencyProperty.RegisterAttached("IsCloseTabButtonVisible", typeof(bool), typeof(TabControlHelper), new FrameworkPropertyMetadata(OnIsCloseTabButtonVisiblePropertyChanged));

        public static bool GetIsCloseTabButtonVisible(UIElement ui)
        {
            return (bool)ui.GetValue(IsCloseTabButtonVisibleProperty);
        }

        public static void SetIsCloseTabButtonVisible(UIElement ui, bool value)
        {
            ui.SetValue(IsCloseTabButtonVisibleProperty, value);
        }

        private static void OnIsCloseTabButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region AddTabButtonCommand Property

        public static readonly DependencyProperty AddTabButtonCommandProperty = DependencyProperty.RegisterAttached("AddTabButtonCommand", typeof(ICommand), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static ICommand GetAddTabButtonCommand(UIElement ui)
        {
            return (ICommand)ui.GetValue(AddTabButtonCommandProperty);
        }

        public static void SetAddTabButtonCommand(UIElement ui, ICommand value)
        {
            ui.SetValue(AddTabButtonCommandProperty, value);
        }

        #endregion

        #region AddTabButtonCommandParameter Property

        public static readonly DependencyProperty AddTabButtonCommandParameterProperty = DependencyProperty.RegisterAttached("AddTabButtonCommandParameter", typeof(object), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static object GetAddTabButtonCommandParameter(UIElement ui)
        {
            return ui.GetValue(AddTabButtonCommandParameterProperty);
        }

        public static void SetAddTabButtonCommandParameter(UIElement ui, object value)
        {
            ui.SetValue(AddTabButtonCommandParameterProperty, value);
        }

        #endregion

        #region CloseButtonOverlayMode Property

        public static readonly DependencyProperty CloseButtonOverlayModeProperty = DependencyProperty.RegisterAttached("CloseButtonOverlayMode", typeof(TabControlCloseButtonOverlayMode), typeof(TabControlHelper), new FrameworkPropertyMetadata(null));

        public static TabControlCloseButtonOverlayMode GetCloseButtonOverlayMode(UIElement ui)
        {
            return (TabControlCloseButtonOverlayMode)ui.GetValue(CloseButtonOverlayModeProperty);
        }

        public static void SetCloseButtonOverlayMode(UIElement ui, TabControlCloseButtonOverlayMode value)
        {
            ui.SetValue(CloseButtonOverlayModeProperty, value);
        }

        #endregion

        #region Methods

        private static void OnTabControlLoaded(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;

            AddButtonClickSubscribe(tabControl, (bool)tabControl.GetValue(IsAddTabButtonVisibleProperty));

            ((INotifyCollectionChanged)tabControl.Items).CollectionChanged += (s, args) =>
            {
                if(args.Action == NotifyCollectionChangedAction.Add && tabControl.Items.Count == 1)
                {
                    tabControl.SelectedIndex = 0;
                }
            };
        }

        private static void AddButtonClickSubscribe(TabControl tabControl, bool isAddButtonVisible)
        {
            Button addButton = tabControl.Template?.FindName("AddButton", tabControl) as Button;

            if (addButton != null && isAddButtonVisible)
            {
                addButton.Click += OnAddButtonClick;
            }
            else
            {
                addButton.Click -= OnAddButtonClick;
            }
        }

        private static void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            TabControl tabControl = ((Button)sender).FindAscendant<TabControl>();

            tabControl.RaiseEvent(new RoutedEventArgs(AddTabButtonClickEvent));
        }

        internal static void OnTabCloseRequested(TabControl tabControl, TabItem tabItem)
        {
            TabItemTabCloseRequestedEventArgs args = new TabItemTabCloseRequestedEventArgs(TabCloseRequestedEvent, tabItem, tabItem.DataContext);

            tabControl?.RaiseEvent(args);
        }

        #endregion
    }
}
