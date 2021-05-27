using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace EasyWPFUI.Controls.Helpers
{
    public class RichTextBoxHelper
    {
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(RichTextBoxHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

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
            RichTextBox textBox = d as RichTextBox;

            if (textBox == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.Loaded += OnLoaded; ;
                textBox.TextChanged += OnTextChanged; ;
            }
            else
            {
                textBox.Loaded -= OnLoaded;
                textBox.TextChanged -= OnTextChanged;
            }
        }

        #endregion

        #region HasText Proeprty

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasText", typeof(bool), typeof(RichTextBoxHelper), new FrameworkPropertyMetadata(false));

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

        #region Methods

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateHasText((RichTextBox)sender);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHasText((RichTextBox)sender);
        }

        private static void UpdateHasText(RichTextBox rtb)
        {
            bool hasText = false;

            if (rtb.Document != null || rtb.Document.Blocks.Count > 0)
            {
                TextPointer startPointer = rtb.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
                TextPointer endPointer = rtb.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);

                if (startPointer != null && endPointer != null && !(rtb.Document.Blocks.FirstBlock is List))
                {
                    hasText = startPointer.CompareTo(endPointer) != 0;
                }
                else if (rtb.Document.Blocks.FirstBlock is List)
                {
                    hasText = true;
                }
            }

            if (GetHasText(rtb) != hasText)
            {
                SetHasText(rtb, hasText);
            }
        }

        #endregion
    }
}
