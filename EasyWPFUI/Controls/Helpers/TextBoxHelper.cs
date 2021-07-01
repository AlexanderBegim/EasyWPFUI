using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace EasyWPFUI.Controls.Helpers
{
    public class TextBoxHelper
    {
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

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
            TextBox textBox = d as TextBox;

            if (textBox == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.Loaded += OnLoaded;
                textBox.TextChanged += OnTextChanged;
            }
            else
            {
                textBox.Loaded -= OnLoaded;
                textBox.TextChanged -= OnTextChanged;
            }
        }

        #endregion

        #region HasText Proeprty

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasText", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public static bool GetHasText(UIElement ui)
        {
            return (bool)ui.GetValue(HasTextProperty);
        }

        public static void SetHasText(UIElement ui, bool value)
        {
            ui.SetValue(HasTextPropertyKey, value);
        }

        #endregion

        #region DeleteButtonEnabled Property

        public static readonly DependencyProperty IsDeleteButtonProperty = DependencyProperty.RegisterAttached("IsDeleteButton", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, OnIsDeleteButtonPropertyChanged));

        public static bool GetIsDeleteButton(UIElement ui)
        {
            return (bool)ui.GetValue(IsDeleteButtonProperty);
        }

        public static void SetIsDeleteButton(UIElement ui, bool value)
        {
            ui.SetValue(IsDeleteButtonProperty, value);
        }

        private static void OnIsDeleteButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;

            if (button == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                button.Click += DeleteButtonClicked;
            }
            else
            {
                button.Click -= DeleteButtonClicked;
            }
        }

        #endregion

        #region Methods

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(textBox.Text))
            {
                SetHasText(textBox, true);
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(textBox.Text))
            {
                SetHasText(textBox, true);
            }
            else
            {
                SetHasText(textBox, false);
            }
        }

        private static void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if(button.TemplatedParent is TextBox)
            {
                TextBox textBox = (TextBox)button.TemplatedParent;
                textBox.Clear();
            }
        }

        #endregion
    }
}
