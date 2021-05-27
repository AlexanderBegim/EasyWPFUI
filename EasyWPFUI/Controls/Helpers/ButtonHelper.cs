using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls.Helpers
{
    public class ButtonHelper
    {
        #region Flyout Property

        public static readonly DependencyProperty FlyoutProperty = DependencyProperty.RegisterAttached("Flyout", typeof(FlyoutBase), typeof(ButtonHelper), new FrameworkPropertyMetadata(OnFlyoutPropertyChanged));

        public static FlyoutBase GetFlyout(UIElement ui)
        {
            return (FlyoutBase)ui.GetValue(FlyoutProperty);
        }

        public static void SetFlyout(UIElement ui, FlyoutBase value)
        {
            ui.SetValue(FlyoutProperty, value);
        }

        private static void OnFlyoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;

            if (button == null)
                return;

            button.Click -= OnButtonClick;

            if (e.NewValue is FlyoutBase)
            {
                button.Click += OnButtonClick;
            }
        }

        private static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            FlyoutBase flyoutBase = GetFlyout(button);

            if(flyoutBase != null)
            {
                flyoutBase.ShowAt(button);
            }
        }

        #endregion
    }
}
