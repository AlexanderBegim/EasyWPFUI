using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public sealed class CoreApplicationViewTitleBar
    {
        public event TypedEventHandler<CoreApplicationViewTitleBar, object> IsVisibleChanged;
        public event TypedEventHandler<CoreApplicationViewTitleBar, object> LayoutMetricsChanged;

        public bool ExtendViewIntoTitleBar
        {
            get
            {
                return m_window.ExtendViewIntoTitleBar;
            }
            set
            {
                m_window.ExtendViewIntoTitleBar = value;
            }
        }

        public double Height
        {
            get
            {
                return m_window.TitleBarHeight;
            }
        }

        public bool IsVisible
        {
            get
            {
                return m_window.WindowStyle != WindowStyle.None;
            }
        }

        public double SystemOverlayLeftInset { get; private set; }
        public double SystemOverlayRightInset { get; private set; }

        internal CoreApplicationViewTitleBar(ModernWindow window)
        {
            if(window == null)
            {
                throw new ArgumentNullException("window");
            }

            m_window = window;

            if(m_window.IsLoaded)
            {
                InitElements();

                UpdateLayoutMetrics();
            }
            else
            {
                m_window.Loaded += OnWindowLoaded;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            m_window.Loaded -= OnWindowLoaded;

            InitElements();

            UpdateLayoutMetrics();
        }

        private void InitElements()
        {
            if (m_window.GetTemplateChild<Grid>("PART_TitleBar") is Grid titleBarGrid)
            {
                titleBarGrid.IsVisibleChanged += OnTitleBarGridIsVisibleChanged;
            }

            if (m_window.GetTemplateChild<Button>("BackButton") is Button backButton)
            {
                m_backButton = backButton;
                backButton.IsVisibleChanged += OnBackButtonIsVisibleChanged;
            }

            if (m_window.GetTemplateChild<FrameworkElement>("PART_TitleBarControlButtons") is FrameworkElement windowControlButtonsPanel)
            {
                m_windowControlButtonsPanel = windowControlButtonsPanel;
                windowControlButtonsPanel.SizeChanged += OnWindowControlButtonsPanelSizeChanged;
            }
        }

        private void OnTitleBarGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateLayoutMetrics();

            IsVisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnBackButtonIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateLayoutMetrics();
        }

        private void OnWindowControlButtonsPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutMetrics();
        }

        private void UpdateLayoutMetrics()
        {
            SystemOverlayLeftInset = m_backButton != null ? m_backButton.ActualWidth : 0;
            SystemOverlayRightInset = m_windowControlButtonsPanel != null ? m_windowControlButtonsPanel.ActualWidth : 0;

            LayoutMetricsChanged?.Invoke(this, EventArgs.Empty);
        }

        private ModernWindow m_window;
        private Button m_backButton;
        private FrameworkElement m_windowControlButtonsPanel;
    }
}
