using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class FlyoutPresenter : ContentControl
    {
        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FlyoutPresenter), new FrameworkPropertyMetadata(OnCornerRadiusPropertyChanged));

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        static FlyoutPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutPresenter), new FrameworkPropertyMetadata(typeof(FlyoutPresenter)));
        }
    }
}
