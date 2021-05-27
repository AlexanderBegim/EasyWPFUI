using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    [ContentProperty(nameof(Content))]
    [StyleTypedProperty(Property = nameof(FlyoutPresenterStyle), StyleTargetType = typeof(FlyoutPresenter))]
    public class Flyout : FlyoutBase
    {
        #region Content Property

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(Flyout), new FrameworkPropertyMetadata(null));

        public UIElement Content
        {
            get
            {
                return (UIElement)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        #endregion

        #region FlyoutPresenterStyle Property

        public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register("FlyoutPresenterStyle", typeof(Style), typeof(Flyout), new FrameworkPropertyMetadata(null));

        public Style FlyoutPresenterStyle
        {
            get
            {
                return (Style)GetValue(FlyoutPresenterStyleProperty);
            }
            set
            {
                SetValue(FlyoutPresenterStyleProperty, value);
            }
        }

        #endregion


        #region Methods

        public Flyout()
        {

        }

        protected override Control CreatePresenter()
        {
            FlyoutPresenter flyoutPresenter = new FlyoutPresenter();

            flyoutPresenter.SetBinding(FlyoutPresenter.ContentProperty,
                new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(ContentProperty)
                });

            flyoutPresenter.SetBinding(FlyoutPresenter.StyleProperty,
                new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(FlyoutPresenterStyleProperty)
                });

            return flyoutPresenter;
        }

        #endregion
    }
}
