using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls.Primitives
{
    public class NavigationViewItemPresenterTemplateSettings : DependencyObject
    {
        #region IconWidth Property

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(NavigationViewItemPresenterTemplateSettings));

        public double IconWidth
        {
            get
            {
                return (double)GetValue(IconWidthProperty);
            }
            set
            {
                SetValue(IconWidthProperty, value);
            }
        }

        #endregion

        #region SmallerIconWidth Property

        public static readonly DependencyProperty SmallerIconWidthProperty = DependencyProperty.Register("SmallerIconWidth", typeof(double), typeof(NavigationViewItemPresenterTemplateSettings));

        public double SmallerIconWidth
        {
            get
            {
                return (double)GetValue(SmallerIconWidthProperty);
            }
            set
            {
                SetValue(SmallerIconWidthProperty, value);
            }
        }

        #endregion
    }
}
