using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls.Helpers
{
    public class ScrollViewerHelper
    {
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(ScrollViewerHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

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
            ScrollViewer scrollViewer = d as ScrollViewer;

            if (scrollViewer == null)
                return;

            if((bool)e.NewValue)
            {
                scrollViewer.Loaded += OnLoaded;
            }
            else
            {
                scrollViewer.Loaded -= OnLoaded;
            }
        }

        #endregion

        #region AllowAutoHide Property

        public static readonly DependencyProperty AllowAutoHideProperty = DependencyProperty.RegisterAttached("AllowAutoHide", typeof(bool), typeof(ScrollViewerHelper), new FrameworkPropertyMetadata(OnAllowAutoHidePropertyChanged));

        public static bool GetAllowAutoHide(UIElement ui)
        {
            return (bool)ui.GetValue(AllowAutoHideProperty);
        }

        public static void SetAllowAutoHide(UIElement ui, bool value)
        {
            ui.SetValue(AllowAutoHideProperty, value);
        }

        private static void OnAllowAutoHidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;

            if (scrollViewer == null)
                return;

            UpdateVisualState(scrollViewer);
        }

        #endregion

        #region Methods

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            scrollViewer.ApplyTemplate();
            UpdateVisualState(scrollViewer, false);
        }

        private static void UpdateVisualState(ScrollViewer scrollViewer, bool useTransitions = true)
        {
            string stateName;

            if (GetAllowAutoHide(scrollViewer))
            {
                stateName = "NoIndicator";
            }
            else
            {
                stateName = "MouseIndicator";
            }

            VisualStateManager.GoToState(scrollViewer, stateName, useTransitions);
        }

        #endregion
    }
}
