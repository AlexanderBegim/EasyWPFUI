using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls.Helpers
{
    public class ScrollBarHelper
    {
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(ScrollBarHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

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
            ScrollBar scrollBar = d as ScrollBar;

            if (scrollBar == null)
                return;

            if ((bool)e.NewValue)
            {
                scrollBar.Loaded += OnLoaded;
                scrollBar.IsVisibleChanged += OnIsVisibleChanged;
                scrollBar.IsEnabledChanged += OnIsEnabledChanged;
                scrollBar.MouseEnter += OnIsMouseOver;
                scrollBar.MouseLeave += OnIsMouseOver;
            }
            else
            {
                scrollBar.Loaded -= OnLoaded;
                scrollBar.IsVisibleChanged -= OnIsVisibleChanged;
                scrollBar.IsEnabledChanged -= OnIsEnabledChanged;
                scrollBar.MouseEnter -= OnIsMouseOver;
                scrollBar.MouseLeave -= OnIsMouseOver;
            }
        }

        #endregion

        #region IndicatorMode Property

        public static readonly DependencyProperty IndicatorModeProperty = DependencyProperty.RegisterAttached("IndicatorMode", typeof(ScrollingIndicatorMode), typeof(ScrollBarHelper), new FrameworkPropertyMetadata(ScrollingIndicatorMode.MouseIndicator, OnIndicatorModePropertyPropertyChanged));

        public static ScrollingIndicatorMode GetIndicatorMode(UIElement ui)
        {
            return (ScrollingIndicatorMode)ui.GetValue(IndicatorModeProperty);
        }

        public static void SetIndicatorMode(UIElement ui, ScrollingIndicatorMode value)
        {
            ui.SetValue(IndicatorModeProperty, value);
        }

        private static void OnIndicatorModePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar scrollBar = d as ScrollBar;

            if (scrollBar == null)
                return;

            UpdateVisualState(scrollBar);
        }

        #endregion

        #region ConsciousState Property

        private static readonly DependencyPropertyKey ConsciousStatePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ConsciousState", typeof(string), typeof(ScrollBarHelper), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ConsciousStateProperty = ConsciousStatePropertyKey.DependencyProperty;

        public static string GetConsciousState(UIElement ui)
        {
            return (string)ui.GetValue(ConsciousStateProperty);
        }

        public static void SetConsciousState(UIElement ui, string value)
        {
            ui.SetValue(ConsciousStatePropertyKey, value);
        }

        #endregion

        #region Methods

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            UpdateVisualState(scrollBar);
        }

        private static void OnIsMouseOver(object sender, MouseEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            UpdateVisualState(scrollBar);
        }

        private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            UpdateVisualState(scrollBar);
        }

        private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;

            if ((bool)e.NewValue)
            {
                UpdateVisualState(scrollBar, false);
            }
        }

        private static void UpdateVisualState(ScrollBar scrollBar, bool useTransitions = true)
        {
            string stateName;

            if(scrollBar.IsEnabled)
            {
                ScrollingIndicatorMode mode = GetIndicatorMode(scrollBar);
                if (mode == ScrollingIndicatorMode.None)
                {

                }
                if (mode == ScrollingIndicatorMode.None || mode == ScrollingIndicatorMode.TouchIndicator)
                {
                    stateName = scrollBar.IsMouseOver ? "Expanded" : "Collapsed";
                }
                else
                {
                    stateName = "Expanded";
                }
            }
            else
            {
                stateName = "Collapsed";
                useTransitions = false;
            }

            SetConsciousState(scrollBar, stateName);

            VisualStateManager.GoToState(scrollBar, stateName, useTransitions);
        }

        #endregion
    }
}
