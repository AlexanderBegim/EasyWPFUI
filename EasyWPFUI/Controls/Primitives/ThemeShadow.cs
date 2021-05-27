using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace EasyWPFUI.Controls.Primitives
{
    public class ThemeShadow : Decorator
    {
        #region IsShadowVisible

        public static readonly DependencyProperty IsShadowVisibleProperty = DependencyProperty.Register("IsShadowVisible", typeof(bool), typeof(ThemeShadow), new FrameworkPropertyMetadata(true, OnIsShadowVisiblePropertyChanged));

        public bool IsShadowVisible
        {
            get
            {
                return (bool)GetValue(IsShadowVisibleProperty);
            }
            set
            {
                SetValue(IsShadowVisibleProperty, value);
            }
        }

        private static void OnIsShadowVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ThemeShadow)d).ChangeIsShadowVisible();
        }

        #endregion

        #region ShadowBrush Property

        public static readonly DependencyProperty ShadowBrushProperty = DependencyProperty.Register("ShadowBrush", typeof(SolidColorBrush), typeof(ThemeShadow), new FrameworkPropertyMetadata(Brushes.Black, OnShadowBrushPropertyChanged));

        public SolidColorBrush ShadowBrush
        {
            get
            {
                return (SolidColorBrush)GetValue(ShadowBrushProperty);
            }
            set
            {
                SetValue(ShadowBrushProperty, value);
            }
        }

        private static void OnShadowBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ThemeShadow)d).ChangeShadowBrush();
        }

        #endregion

        #region ShadowDepth Property

        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register("ShadowDepth", typeof(double), typeof(ThemeShadow), new FrameworkPropertyMetadata(20d, OnShadowDepthPropertyChanged));

        public double ShadowDepth
        {
            get
            {
                return (double)GetValue(ShadowDepthProperty);
            }
            set
            {
                SetValue(ShadowDepthProperty, value);
            }
        }

        private static void OnShadowDepthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ThemeShadow)d).ChangeShadowDepth();
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ThemeShadow), new FrameworkPropertyMetadata(OnCornerRadiusPropertyChanged));

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
            ((ThemeShadow)d).ChangeCornerRadius();
        }

        #endregion

        #region ShadowOpacity Property

        public static readonly DependencyProperty ShadowOpacityProperty = DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(ThemeShadow), new FrameworkPropertyMetadata(0.2d, OnShadowOpacityPropertyChanged));

        public double ShadowOpacity
        {
            get
            {
                return (double)GetValue(ShadowOpacityProperty);
            }
            set
            {
                SetValue(ShadowOpacityProperty, value);
            }
        }

        private static void OnShadowOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ThemeShadow)d).ChangeShadowOpacity();
        }

        #endregion

        protected override int VisualChildrenCount => (Child != null) ? 2 : 1;

        #region Methods

        public ThemeShadow()
        {
            InitializeShadow();
        }

        private void InitializeShadow()
        {
            shadowEffect = new BlurEffect()
            {
                Radius = ShadowDepth,
                RenderingBias = RenderingBias.Performance
            };

            shadowTransform = new TranslateTransform(0, ShadowDepth / 3);

            shadowBorder = new Border()
            {
                Effect = shadowEffect,
                Background = ShadowBrush,
                Visibility = IsShadowVisible ? Visibility.Visible : Visibility.Collapsed,
                CornerRadius = CornerRadius,
                Opacity = ShadowOpacity,
                RenderTransform = shadowTransform,
            };


            AddVisualChild(shadowBorder);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            shadowBorder?.Arrange(new Rect(arrangeSize));

            return base.ArrangeOverride(arrangeSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            shadowBorder?.Measure(constraint);

            return base.MeasureOverride(constraint);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && shadowBorder != null)
            {
                return shadowBorder;
            }
            else if (index == 1 && Child != null)
            {
                return Child;
            }
            else
            {
                return base.GetVisualChild(index);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            Thickness margin = new Thickness(ShadowDepth);

            if (Parent is Popup popup && popup.Child is FrameworkElement popupChild)
            {
                popupChild.Margin = margin;
            }
            else if (VisualParent is ContextMenu contextMenu)
            {
                contextMenu.Margin = margin; ;
            }
            else if (VisualParent is ToolTip toolTip)
            {
                toolTip.Margin = margin;

                if (shadowTransform != null)
                {
                    shadowTransform.Y = 0;
                }
            }
            else if (VisualParent is FlyoutPresenter flyoutPresenter)
            {
                flyoutPresenter.Margin = margin;
            }
            else if (VisualParent is TeachingTip teachingTip)
            {

            }

            base.OnVisualParentChanged(oldParent);
        }

        private void ChangeIsShadowVisible()
        {
            if(shadowBorder != null)
            {
                shadowBorder.Visibility = IsShadowVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ChangeShadowBrush()
        {
            if(shadowBorder != null)
            {
                shadowBorder.Background = ShadowBrush;
            }
        }

        private void ChangeShadowDepth()
        {
            if(shadowEffect != null)
            {
                shadowEffect.Radius = ShadowDepth;
            }

            if(shadowTransform != null)
            {
                shadowTransform.Y = ShadowDepth / 3;
            }
        }

        private void ChangeCornerRadius()
        {
            if(shadowBorder != null)
            {
                shadowBorder.CornerRadius = CornerRadius;
            }
        }

        private void ChangeShadowOpacity()
        {
            if(shadowBorder != null)
            {
                shadowBorder.Opacity = ShadowOpacity;
            }
        }

        #endregion

        private Border shadowBorder;
        private BlurEffect shadowEffect;
        private TranslateTransform shadowTransform;
    }
}
