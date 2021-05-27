// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI;
using EasyWPFUI.Common;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public enum InfoBarCloseReason
    {
        CloseButton = 0,
        Programmatic = 1
    }

    public enum InfoBarSeverity
    {
        Informational = 0,
        Success = 1,
        Warning = 2,
        Error = 3,
    }

    public class InfoBarClosedEventArgs : EventArgs
    {
        public InfoBarCloseReason Reason { get; internal set; }
    }

    public class InfoBarClosingEventArgs : EventArgs
    {
        public InfoBarCloseReason Reason { get; internal set; }
        public bool Cancel { get; set; }
    }

    [ContentProperty("Content")]
    public class InfoBar : Control
    {
        private const string c_closeButtonName = "CloseButton";
        private const string c_contentRootName = "ContentRoot";

        #region Fields

        private InfoBarCloseReason m_lastCloseReason = InfoBarCloseReason.Programmatic;

        private bool m_applyTemplateCalled = false;
        private bool m_notifyOpen = false;
        private bool m_isVisible = false;

        #endregion

        #region Events

        public event TypedEventHandler<InfoBar, object> CloseButtonClick;
        public event TypedEventHandler<InfoBar, InfoBarClosingEventArgs> Closing;
        public event TypedEventHandler<InfoBar, InfoBarClosedEventArgs> Closed;

        #endregion


        #region IsOpen Property

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(InfoBar), new FrameworkPropertyMetadata(false, OnIsOpenPropertyChanged));

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.OnIsOpenPropertyChanged(e);
        }

        #endregion

        #region Title Property

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(InfoBar), new FrameworkPropertyMetadata());

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        #endregion

        #region Message Property

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(InfoBar), new FrameworkPropertyMetadata());

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        #endregion

        #region Severity Property

        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register("Severity", typeof(InfoBarSeverity), typeof(InfoBar), new FrameworkPropertyMetadata(InfoBarSeverity.Informational, OnSeverityPropertyChanged));

        public InfoBarSeverity Severity
        {
            get
            {
                return (InfoBarSeverity)GetValue(SeverityProperty);
            }
            set
            {
                SetValue(SeverityProperty, value);
            }
        }

        private static void OnSeverityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.OnSeverityPropertyChanged(e);
        }

        #endregion

        #region IconSource Property

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(IconSource), typeof(InfoBar), new FrameworkPropertyMetadata(OnIconSourcePropertyChanged));

        public IconSource IconSource
        {
            get
            {
                return (IconSource)GetValue(IconSourceProperty);
            }
            set
            {
                SetValue(IconSourceProperty, value);
            }
        }

        private static void OnIconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.OnIconSourcePropertyChanged(e);
        }

        #endregion

        #region IsIconVisible Property

        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(InfoBar), new FrameworkPropertyMetadata(true, OnIsIconVisiblePropertyChanged));

        public bool IsIconVisible
        {
            get
            {
                return (bool)GetValue(IsIconVisibleProperty);
            }
            set
            {
                SetValue(IsIconVisibleProperty, value);
            }
        }

        private static void OnIsIconVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.OnIsIconVisiblePropertyChanged(e);
        }

        #endregion

        #region IsClosable Property

        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(bool), typeof(InfoBar), new FrameworkPropertyMetadata(true, OnIsClosablePropertyChanged));

        public bool IsClosable
        {
            get
            {
                return (bool)GetValue(IsClosableProperty);
            }
            set
            {
                SetValue(IsClosableProperty, value);
            }
        }

        private static void OnIsClosablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.OnIsClosablePropertyChanged(e);
        }

        #endregion

        #region CloseButtonStyle Property

        public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(InfoBar), new FrameworkPropertyMetadata());

        public Style CloseButtonStyle
        {
            get
            {
                return (Style)GetValue(CloseButtonStyleProperty);
            }
            set
            {
                SetValue(CloseButtonStyleProperty, value);
            }
        }

        #endregion

        #region CloseButtonCommand Property

        public static readonly DependencyProperty CloseButtonCommandProperty = DependencyProperty.Register("CloseButtonCommand", typeof(ICommand), typeof(InfoBar), new FrameworkPropertyMetadata());

        public ICommand CloseButtonCommand
        {
            get
            {
                return (ICommand)GetValue(CloseButtonCommandProperty);
            }
            set
            {
                SetValue(CloseButtonCommandProperty, value);
            }
        }

        #endregion

        #region CloseButtonCommandParameter Property

        public static readonly DependencyProperty CloseButtonCommandParameterProperty = DependencyProperty.Register("CloseButtonCommandParameter", typeof(object), typeof(InfoBar), new FrameworkPropertyMetadata());

        public object CloseButtonCommandParameter
        {
            get
            {
                return GetValue(CloseButtonCommandParameterProperty);
            }
            set
            {
                SetValue(CloseButtonCommandParameterProperty, value);
            }
        }

        #endregion

        #region ActionButton Property

        public static readonly DependencyProperty ActionButtonProperty = DependencyProperty.Register("ActionButton", typeof(ButtonBase), typeof(InfoBar), new FrameworkPropertyMetadata());

        public ButtonBase ActionButton
        {
            get
            {
                return (ButtonBase)GetValue(ActionButtonProperty);
            }
            set
            {
                SetValue(ActionButtonProperty, value);
            }
        }

        #endregion

        #region Content Property

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(InfoBar), new FrameworkPropertyMetadata());

        public object Content
        {
            get
            {
                return (object)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        #endregion

        #region ContentTemplate Property

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(InfoBar), new FrameworkPropertyMetadata());

        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        #endregion

        #region TemplateSettings Property

        public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register("TemplateSettings", typeof(InfoBarTemplateSettings), typeof(InfoBar), new FrameworkPropertyMetadata());

        public InfoBarTemplateSettings TemplateSettings
        {
            get
            {
                return (InfoBarTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            set
            {
                SetValue(TemplateSettingsProperty, value);
            }
        }

        #endregion

        #region Methods

        static InfoBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoBar), new FrameworkPropertyMetadata(typeof(InfoBar)));
            ForegroundProperty.OverrideMetadata(typeof(InfoBar), new FrameworkPropertyMetadata(OnForegroundPropertyChanged));
        }

        public InfoBar()
        {
            TemplateSettings = new InfoBarTemplateSettings();
        }

        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InfoBar infoBar = d as InfoBar;

            if (infoBar == null)
                return;

            infoBar.UpdateForeground();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InfoBarAutomationPeer(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_applyTemplateCalled = true;

            if (GetTemplateChild(c_closeButtonName) is Button closeButton)
            {
                closeButton.Click += OnCloseButtonClick;

                // Do localization for the close button
                if (string.IsNullOrEmpty(AutomationProperties.GetName(closeButton)))
                {
                    string closeButtonName = Properties.Resources.Strings.Resources.InfoBarCloseButtonName;

                    AutomationProperties.SetName(closeButton, closeButtonName);
                }

                // Setup the tooltip for the close button
                ToolTip tooltip = new ToolTip();
                string closeButtonTooltipText = Properties.Resources.Strings.Resources.InfoBarCloseButtonTooltip;
                tooltip.Content = closeButtonTooltipText;
                ToolTipService.SetToolTip(closeButton, tooltip);
            }

            UpdateVisibility(m_notifyOpen, true);
            m_notifyOpen = false;

            UpdateSeverity();
            UpdateIcon();
            UpdateIconVisibility();
            UpdateCloseButton();
            UpdateForeground();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            m_lastCloseReason = InfoBarCloseReason.CloseButton;

            CloseButtonClick?.Invoke(this, EventArgs.Empty);

            IsOpen = false;
        }

        private void RaiseClosingEvent()
        {
            InfoBarClosingEventArgs args = new InfoBarClosingEventArgs();
            args.Reason = m_lastCloseReason;

            Closing?.Invoke(this, args);

            if (!args.Cancel)
            {
                UpdateVisibility();
                RaiseClosedEvent();
            }
            else
            {
                // The developer has changed the Cancel property to true,
                // so we need to revert the IsOpen property to true.
                IsOpen = false;
            }
        }

        private void RaiseClosedEvent()
        {
            InfoBarClosedEventArgs args = new InfoBarClosedEventArgs();
            args.Reason = m_lastCloseReason;

            Closed?.Invoke(this, args);
        }

        private void UpdateVisibility(bool notify = true, bool force = false)
        {
            InfoBarAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as InfoBarAutomationPeer;

            if (!m_applyTemplateCalled)
            {
                // ApplyTemplate() hasn't been called yet but IsOpen has already been set.
                // Since this method will be called again shortly from ApplyTemplate, we'll just wait and send a notification then.
                m_notifyOpen = true;
            }
            else
            {
                // Don't do any work if nothing has changed (unless we are forcing a update)
                if (force || IsOpen != m_isVisible)
                {
                    if (IsOpen)
                    {
                        VisualStateManager.GoToState(this, "InfoBarVisible", false);
                        m_isVisible = true;
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "InfoBarCollapsed", false);
                        m_isVisible = false;
                    }
                }
            }
        }

        private void UpdateSeverity()
        {
            string severityState = "Informational";

            switch (Severity)
            {
                case InfoBarSeverity.Success: severityState = "Success"; break;
                case InfoBarSeverity.Warning: severityState = "Warning"; break;
                case InfoBarSeverity.Error: severityState = "Error"; break;
            }

            VisualStateManager.GoToState(this, severityState, false);
        }

        private void UpdateIcon()
        {
            if (IconSource != null)
            {
                TemplateSettings.IconElement = SharedHelpers.MakeIconElementFrom(IconSource);
            }
            else
            {
                TemplateSettings.IconElement = null;
            }
        }

        private void UpdateIconVisibility()
        {
            VisualStateManager.GoToState(this, IsIconVisible ? (IconSource != null ? "UserIconVisible" : "StandardIconVisible") : "NoIconVisible", false);
        }

        private void UpdateCloseButton()
        {
            VisualStateManager.GoToState(this, IsClosable ? "CloseButtonVisible" : "CloseButtonCollapsed", false);
        }

        private void UpdateForeground()
        {
            // If Foreground is set, then change Title and Message Foreground to match.
            VisualStateManager.GoToState(this, ReadLocalValue(Control.ForegroundProperty) == DependencyProperty.UnsetValue ? "ForegroundNotSet" : "ForegroundSet", false);
        }


        // Property change handlers
        private void OnIsOpenPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsOpen)
            {
                //Reset the close reason to the default value of programmatic.
                m_lastCloseReason = InfoBarCloseReason.Programmatic;

                UpdateVisibility();
            }
            else
            {
                RaiseClosingEvent();
            }
        }

        private void OnSeverityPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateSeverity();
        }

        private void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateIcon();
            UpdateIconVisibility();
        }

        private void OnIsIconVisiblePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateIconVisibility();
        }

        private void OnIsClosablePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateCloseButton();
        }

        #endregion
    }
}
