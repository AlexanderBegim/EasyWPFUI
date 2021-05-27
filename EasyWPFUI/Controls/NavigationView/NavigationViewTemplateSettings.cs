using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class NavigationViewTemplateSettings : DependencyObject
    {
        #region TopPadding Property

        public static readonly DependencyProperty TopPaddingProperty = DependencyProperty.Register("TopPadding", typeof(double), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(0d));

        public double TopPadding
        {
            get
            {
                return (double)GetValue(TopPaddingProperty);
            }
            set
            {
                SetValue(TopPaddingProperty, value);
            }
        }

        #endregion

        #region OverflowButtonVisibility Property

        public static readonly DependencyProperty OverflowButtonVisibilityProperty = DependencyProperty.Register("OverflowButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(Visibility.Collapsed));

        public Visibility OverflowButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(OverflowButtonVisibilityProperty);
            }
            set
            {
                SetValue(OverflowButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region PaneToggleButtonVisibility Property

        public static readonly DependencyProperty PaneToggleButtonVisibilityProperty = DependencyProperty.Register("PaneToggleButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(Visibility.Visible));

        public Visibility PaneToggleButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(PaneToggleButtonVisibilityProperty);
            }
            set
            {
                SetValue(PaneToggleButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region BackButtonVisibility Property

        public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(Visibility.Collapsed));

        public Visibility BackButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(BackButtonVisibilityProperty);
            }
            set
            {
                SetValue(BackButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region TopPaneVisibility Property

        public static readonly DependencyProperty TopPaneVisibilityProperty = DependencyProperty.Register("TopPaneVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(Visibility.Collapsed));

        public Visibility TopPaneVisibility
        {
            get
            {
                return (Visibility)GetValue(TopPaneVisibilityProperty);
            }
            set
            {
                SetValue(TopPaneVisibilityProperty, value);
            }
        }

        #endregion

        #region LeftPaneVisibility Property

        public static readonly DependencyProperty LeftPaneVisibilityProperty = DependencyProperty.Register("LeftPaneVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(Visibility.Visible));

        public Visibility LeftPaneVisibility
        {
            get
            {
                return (Visibility)GetValue(LeftPaneVisibilityProperty);
            }
            set
            {
                SetValue(LeftPaneVisibilityProperty, value);
            }
        }

        #endregion

        #region SingleSelectionFollowsFocus Property

        public static readonly DependencyProperty SingleSelectionFollowsFocusProperty = DependencyProperty.Register("SingleSelectionFollowsFocus", typeof(bool), typeof(NavigationViewTemplateSettings), new FrameworkPropertyMetadata(false));

        public bool SingleSelectionFollowsFocus
        {
            get
            {
                return (bool)GetValue(SingleSelectionFollowsFocusProperty);
            }
            set
            {
                SetValue(SingleSelectionFollowsFocusProperty, value);
            }
        }

        #endregion
    }
}
