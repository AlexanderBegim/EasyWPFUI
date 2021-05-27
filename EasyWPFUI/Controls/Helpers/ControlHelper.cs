using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

namespace EasyWPFUI.Controls.Helpers
{
    public static class ControlHelper
    {
        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper), new PropertyMetadata(new CornerRadius(2)));

        public static CornerRadius GetCornerRadius(UIElement ui)
        {
            return (CornerRadius)ui.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(UIElement ui, CornerRadius value)
        {
            ui.SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region Header Property

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(object), typeof(ControlHelper), new FrameworkPropertyMetadata(null));

        public static object GetHeader(UIElement ui)
        {
            return ui.GetValue(HeaderProperty);
        }

        public static void SetHeader(UIElement ui, object value)
        {
            ui.SetValue(HeaderProperty, value);
        }

        #endregion

        #region HeaderTemplate Property

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(ControlHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetHeaderTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(HeaderTemplateProperty);
        }

        public static void SetHeaderTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region PlaceholderText Property

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(ControlHelper), new FrameworkPropertyMetadata(string.Empty));

        public static string GetPlaceholderText(UIElement ui)
        {
            return (string)ui.GetValue(PlaceholderTextProperty);
        }

        public static void SetPlaceholderText(UIElement ui, string value)
        {
            ui.SetValue(PlaceholderTextProperty, value);
        }

        #endregion

        #region PlaceholderForeground Property

        public static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.RegisterAttached("PlaceholderForeground", typeof(Brush), typeof(ControlHelper), new FrameworkPropertyMetadata(Brushes.White));

        public static Brush GetPlaceholderForeground(UIElement ui)
        {
            return (Brush)ui.GetValue(PlaceholderForegroundProperty);
        }

        public static void SetPlaceholderForeground(UIElement ui, Brush value)
        {
            ui.SetValue(PlaceholderForegroundProperty, value);
        }

        #endregion

        #region Description Property

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached("Description", typeof(object), typeof(ControlHelper), new FrameworkPropertyMetadata(null));

        public static object GetDescription(UIElement ui)
        {
            return ui.GetValue(DescriptionProperty);
        }

        public static void SetDescription(UIElement ui, object value)
        {
            ui.SetValue(DescriptionProperty, value);
        }

        #endregion

        /* System Visual Focus */

        private static readonly DependencyProperty ActualItemForFocusProperty = DependencyProperty.Register("ActualItemForFocus", typeof(FrameworkElement), typeof(ControlHelper));

        #region IsSystemFocusVisuals Property

        public static readonly DependencyProperty IsSystemFocusVisualsProperty = DependencyProperty.RegisterAttached("IsSystemFocusVisuals", typeof(bool), typeof(ControlHelper), new FrameworkPropertyMetadata(OnIsSystemFocusVisualsPropertyChanged));

        public static bool GetIsSystemFocusVisuals(FrameworkElement ui)
        {
            return (bool)ui.GetValue(IsSystemFocusVisualsProperty);
        }

        public static void SetIsSystemFocusVisuals(FrameworkElement ui, bool value)
        {
            ui.SetValue(IsSystemFocusVisualsProperty, value);
        }

        private static void OnIsSystemFocusVisualsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;

            control.Loaded += IsSystemFocusVisualsControlLoaded;
        }

        private static void IsSystemFocusVisualsControlLoaded(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            DependencyObject parent = VisualTreeHelper.GetParent(control) as Adorner;

            if (control != null && parent is Adorner adorner && adorner.AdornedElement is FrameworkElement element)
            {
                bool useSystemFocusVisuals = (bool)element.GetValue(ControlHelper.UseSystemFocusVisualsProperty);

                if (element.GetValue(ActualItemForFocusProperty) is FrameworkElement target)
                {
                    if (target.FocusVisualStyle is Style fvs && fvs.Setters.Count == 0 && fvs.BasedOn == null)
                    {
                        target.FocusVisualStyle = element.FocusVisualStyle;
                    }

                    target.Focusable = true;
                    Keyboard.Focus(target);
                }

                if (useSystemFocusVisuals)
                {
                    if((bool)element.GetValue(IsTemplateFocusTargetProperty))
                    {
                        if(element.TemplatedParent is FrameworkElement el)
                        {
                            element = el;
                        }
                    }

                    if (element.GetValue(ControlHelper.FocusVisualMarginProperty) is Thickness margin)
                    {
                        control.Margin = margin;
                    }
                    control.CopyDependencyValue(element, FocusVisualPrimaryBrushProperty);
                    control.CopyDependencyValue(element, FocusVisualPrimaryThicknessProperty);
                    control.CopyDependencyValue(element, FocusVisualSecondaryBrushProperty);
                    control.CopyDependencyValue(element, FocusVisualSecondaryThicknessProperty);
                }
                else
                {
                    control.Template = null;
                }
            }
        }

        private static void CopyDependencyValue(this Control target, DependencyObject source, DependencyProperty property)
        {
            if (source.GetValue(property) is object value)
            {
                target.SetValue(property, value);
            }
        }

        #endregion

        #region UseSystemFocusVisuals Property

        public static readonly DependencyProperty UseSystemFocusVisualsProperty = DependencyProperty.RegisterAttached("UseSystemFocusVisuals", typeof(bool), typeof(ControlHelper), new FrameworkPropertyMetadata(true, OnUseSystemFocusVisualsPropertyChanged));

        public static bool GetUseSystemFocusVisuals(FrameworkElement ui)
        {
            return (bool)ui.GetValue(FocusVisualPrimaryThicknessProperty);
        }

        public static void SetUseSystemFocusVisuals(FrameworkElement ui, bool value)
        {
            ui.SetValue(UseSystemFocusVisualsProperty, value);
        }

        private static void OnUseSystemFocusVisualsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IsTemplateFocusTarget Property

        public static readonly DependencyProperty IsTemplateFocusTargetProperty = DependencyProperty.RegisterAttached("IsTemplateFocusTarget", typeof(bool), typeof(ControlHelper), new FrameworkPropertyMetadata(OnIsTemplateFocusTargetPropertyChanged));

        public static bool GetIsTemplateFocusTarget(FrameworkElement ui)
        {
            return (bool)ui.GetValue(FocusVisualPrimaryThicknessProperty);
        }

        public static void SetIsTemplateFocusTarget(FrameworkElement ui, bool value)
        {
            ui.SetValue(UseSystemFocusVisualsProperty, value);
        }

        private static void OnIsTemplateFocusTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && element.TemplatedParent is Control control)
            {
                if((bool)e.NewValue)
                {
                    control.SetValue(ActualItemForFocusProperty, element);
                }
                else
                {
                    control.ClearValue(ActualItemForFocusProperty);
                }
            }
        }

        #endregion

        #region FocusVisualPrimaryThickness Property

        public static readonly DependencyProperty FocusVisualPrimaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryThickness", typeof(Thickness), typeof(ControlHelper), new FrameworkPropertyMetadata(new Thickness(2), OnFocusVisualPrimaryThicknessPropertyChanged));

        public static Thickness GetFocusVisualPrimaryThickness(FrameworkElement ui)
        {
            return (Thickness)ui.GetValue(FocusVisualPrimaryThicknessProperty);
        }

        public static void SetFocusVisualPrimaryThickness(FrameworkElement ui, Thickness value)
        {
            ui.SetValue(FocusVisualPrimaryThicknessProperty, value);
        }

        private static void OnFocusVisualPrimaryThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region FocusVisualSecondaryThickness Property

        public static readonly DependencyProperty FocusVisualSecondaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryThickness", typeof(Thickness), typeof(ControlHelper), new FrameworkPropertyMetadata(new Thickness(0), OnFocusVisualSecondaryThicknessPropertyChanged));

        public static Thickness GetFocusVisualSecondaryThickness(FrameworkElement ui)
        {
            return (Thickness)ui.GetValue(FocusVisualSecondaryThicknessProperty);
        }

        public static void SetFocusVisualSecondaryThickness(FrameworkElement ui, Thickness value)
        {
            ui.SetValue(FocusVisualSecondaryThicknessProperty, value);
        }

        private static void OnFocusVisualSecondaryThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region FocusVisualPrimaryBrush Property

        public static readonly DependencyProperty FocusVisualPrimaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryBrush", typeof(Brush), typeof(ControlHelper), new FrameworkPropertyMetadata(OnFocusVisualPrimaryBrushPropertyChanged));

        public static Brush GetFocusVisualPrimaryBrush(FrameworkElement ui)
        {
            return (Brush)ui.GetValue(FocusVisualPrimaryBrushProperty);
        }

        public static void SetFocusVisualPrimaryBrush(FrameworkElement ui, Brush value)
        {
            ui.SetValue(FocusVisualPrimaryBrushProperty, value);
        }

        private static void OnFocusVisualPrimaryBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region FocusVisualSecondaryBrush Property

        public static readonly DependencyProperty FocusVisualSecondaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryBrush", typeof(Brush), typeof(ControlHelper), new FrameworkPropertyMetadata(OnFocusVisualSecondaryBrushPropertyChanged));

        public static Brush GetFocusVisualSecondaryBrush(FrameworkElement ui)
        {
            return (Brush)ui.GetValue(FocusVisualSecondaryBrushProperty);
        }

        public static void SetFocusVisualSecondaryBrush(FrameworkElement ui, Brush value)
        {
            ui.SetValue(FocusVisualSecondaryBrushProperty, value);
        }

        private static void OnFocusVisualSecondaryBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region FocusVisualMargin Property

        public static readonly DependencyProperty FocusVisualMarginProperty = DependencyProperty.RegisterAttached("FocusVisualMargin", typeof(Thickness), typeof(ControlHelper), new FrameworkPropertyMetadata(new Thickness(0), OnFocusVisualMarginPropertyChanged));

        public static Thickness GetFocusVisualMargin(FrameworkElement ui)
        {
            return (Thickness)ui.GetValue(FocusVisualMarginProperty);
        }

        public static void SetFocusVisualMargin(FrameworkElement ui, Thickness value)
        {
            ui.SetValue(FocusVisualMarginProperty, value);
        }

        private static void OnFocusVisualMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion
    }
}
