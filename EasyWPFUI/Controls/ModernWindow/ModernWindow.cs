using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Controls.Primitives;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls
{
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Border))]
    [TemplatePart(Name = "PART_LayoutGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_TitleBar", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_TitleBarControlButtons", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    public class ModernWindow : Window
    {
        #region Events

        public event EventHandler<BackRequestedEventArgs> BackRequested;

        #endregion

        #region BorderBrushInactive Property

        public static readonly DependencyProperty BorderBrushInactiveProperty = DependencyProperty.Register("BorderBrushInactive", typeof(Brush), typeof(ModernWindow), new FrameworkPropertyMetadata(SystemColors.WindowFrameBrush));

        public Brush BorderBrushInactive
        {
            get
            {
                return (Brush)GetValue(BorderBrushInactiveProperty);
            }
            set
            {
                SetValue(BorderBrushInactiveProperty, value);
            }
        }

        #endregion

        #region IsWindowAccent Property

        public static readonly DependencyProperty IsWindowAccentProperty = DependencyProperty.Register("IsWindowAccent", typeof(bool), typeof(ModernWindow), new FrameworkPropertyMetadata(false));

        public bool IsWindowAccent
        {
            get
            {
                return (bool)GetValue(IsWindowAccentProperty);
            }
            set
            {
                SetValue(IsWindowAccentProperty, value);
            }
        }

        #endregion

        #region ExtendViewIntoTitleBar Property

        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty = DependencyProperty.Register("ExtendViewIntoTitleBar", typeof(bool), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public bool ExtendViewIntoTitleBar
        {
            get
            {
                return (bool)GetValue(ExtendViewIntoTitleBarProperty);
            }
            set
            {
                SetValue(ExtendViewIntoTitleBarProperty, value);
            }
        }

        #endregion

        #region TitleBarBackground Property

        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(ModernWindow), new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush TitleBarBackground
        {
            get
            {
                return (Brush)GetValue(TitleBarBackgroundProperty);
            }
            set
            {
                SetValue(TitleBarBackgroundProperty, value);
            }
        }

        #endregion

        #region TitleBarInactiveBackground Property

        public static readonly DependencyProperty TitleBarInactiveBackgroundProperty = DependencyProperty.Register("TitleBarInactiveBackground", typeof(Brush), typeof(ModernWindow), new FrameworkPropertyMetadata(Brushes.Transparent));

        public Brush TitleBarInactiveBackground
        {
            get
            {
                return (Brush)GetValue(TitleBarInactiveBackgroundProperty);
            }
            set
            {
                SetValue(TitleBarInactiveBackgroundProperty, value);
            }
        }

        #endregion

        #region TitleBarHeight Property

        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(ModernWindow), new FrameworkPropertyMetadata(30d));

        public double TitleBarHeight
        {
            get
            {
                return (double)GetValue(TitleBarHeightProperty);
            }
            set
            {
                SetValue(TitleBarHeightProperty, value);
            }
        }

        #endregion

        #region TitleBarForeground Property

        public static readonly DependencyProperty TitleBarForegroundProperty = DependencyProperty.Register("TitleBarForeground", typeof(Brush), typeof(ModernWindow), new FrameworkPropertyMetadata(SystemColors.WindowTextBrush));

        public Brush TitleBarForeground
        {
            get
            {
                return (Brush)GetValue(TitleBarForegroundProperty);
            }
            set
            {
                SetValue(TitleBarForegroundProperty, value);
            }
        }

        #endregion

        #region TitleBarInactiveForeground Property

        public static readonly DependencyProperty TitleBarInactiveForegroundProperty = DependencyProperty.Register("TitleBarInactiveForeground", typeof(Brush), typeof(ModernWindow), new FrameworkPropertyMetadata(SystemColors.WindowTextBrush));

        public Brush TitleBarInactiveForeground
        {
            get
            {
                return (Brush)GetValue(TitleBarInactiveForegroundProperty);
            }
            set
            {
                SetValue(TitleBarInactiveForegroundProperty, value);
            }
        }

        #endregion

        #region TitleTextAlign Property

        public static readonly DependencyProperty TitleTextAlignProperty = DependencyProperty.Register("TitleTextAlign", typeof(HorizontalAlignment), typeof(ModernWindow), new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment TitleTextAlign
        {
            get
            {
                return (HorizontalAlignment)GetValue(TitleTextAlignProperty);
            }
            set
            {
                SetValue(TitleTextAlignProperty, value);
            }
        }

        #endregion

        #region TitleBarContent Property

        public static readonly DependencyProperty TitleBarContentProperty = DependencyProperty.Register("TitleBarContent", typeof(object), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public object TitleBarContent
        {
            get
            {
                return GetValue(TitleBarContentProperty);
            }
            set
            {
                SetValue(TitleBarContentProperty, value);
            }
        }

        #endregion

        #region TitleBarContentTemplate Property

        public static readonly DependencyProperty TitleBarContentTemplateProperty = DependencyProperty.Register("TitleBarContentTemplate", typeof(DataTemplate), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public DataTemplate TitleBarContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TitleBarContentTemplateProperty);
            }
            set
            {
                SetValue(TitleBarContentTemplateProperty, value);
            }
        }

        #endregion

        #region IsBackButtonEnabled Property

        public static readonly DependencyProperty IsBackButtonEnabledProperty = DependencyProperty.Register("IsBackButtonEnabled", typeof(bool), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public bool IsBackButtonEnabled
        {
            get
            {
                return (bool)GetValue(IsBackButtonEnabledProperty);
            }
            set
            {
                SetValue(IsBackButtonEnabledProperty, value);
            }
        }

        #endregion

        #region IsBackButtonVisible Property

        public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public bool IsBackButtonVisible
        {
            get
            {
                return (bool)GetValue(IsBackButtonVisibleProperty);
            }
            set
            {
                SetValue(IsBackButtonVisibleProperty, value);
            }
        }

        #endregion

        #region BackButtonStyle Property

        public static readonly DependencyProperty BackButtonStyleProperty = DependencyProperty.Register("BackButtonStyle", typeof(Style), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public Style BackButtonStyle
        {
            get
            {
                return (Style)GetValue(BackButtonStyleProperty);
            }
            set
            {
                SetValue(BackButtonStyleProperty, value);
            }
        }

        #endregion

        #region BackButtonCommand Property

        public static readonly DependencyProperty BackButtonCommandProperty = DependencyProperty.Register("BackButtonCommand", typeof(ICommand), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public ICommand BackButtonCommand
        {
            get
            {
                return (ICommand)GetValue(BackButtonCommandProperty);
            }
            set
            {
                SetValue(BackButtonCommandProperty, value);
            }
        }

        #endregion

        #region BackButtonCommandParameter Property

        public static readonly DependencyProperty BackButtonCommandParameterProperty = DependencyProperty.Register("BackButtonCommandParameter", typeof(object), typeof(ModernWindow), new FrameworkPropertyMetadata());

        public object BackButtonCommandParameter
        {
            get
            {
                return GetValue(BackButtonCommandParameterProperty);
            }
            set
            {
                SetValue(BackButtonCommandParameterProperty, value);
            }
        }

        #endregion

        #region Methods

        static ModernWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernWindow), new FrameworkPropertyMetadata(typeof(ModernWindow)));
        }

        public ModernWindow()
        {
            SetResourceReference(StyleProperty, typeof(ModernWindow));

            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("BackButton") is Button backButton)
            {
                backButton.Click += OnBackButtonClick;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                int borderPadding = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);
                Thickness curr = SystemParameters.WindowResizeBorderThickness;
                Padding = new Thickness(curr.Left + borderPadding, curr.Top + borderPadding, curr.Right + borderPadding, curr.Top + borderPadding);
            }
            else
            {
                Padding = new Thickness(0);
            }

            base.OnStateChanged(e);
        }

        public CoreApplicationViewTitleBar GetTitleBar()
        {
            if (m_coreAppTitleBar != null && m_coreAppTitleBar.TryGetTarget(out CoreApplicationViewTitleBar target))
            {
                return target;
            }

            CoreApplicationViewTitleBar titleBar = new CoreApplicationViewTitleBar(this);

            m_coreAppTitleBar = new WeakReference<CoreApplicationViewTitleBar>(titleBar);

            return titleBar;
        }

        public void SetTitleBar(FrameworkElement element)
        {
            if (!ExtendViewIntoTitleBar)
                return;

            WindowChrome.SetIsHitTestVisibleInChrome(element, false);

            TitleBarHeight = element.ActualHeight > 0 ? element.ActualHeight : (element.Height > 0 ? element.Height : TitleBarHeight);

            WindowChrome.GetWindowChrome(this).CaptionHeight = TitleBarHeight;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new BackRequestedEventArgs());
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        internal T GetTemplateChild<T>(string childName) where T : FrameworkElement
        {
            return GetTemplateChild(childName) as T;
        }

        #endregion

        private WeakReference<CoreApplicationViewTitleBar> m_coreAppTitleBar = null;
    }
}
