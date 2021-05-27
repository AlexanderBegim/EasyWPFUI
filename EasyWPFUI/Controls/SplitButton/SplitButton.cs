// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.ComponentModel;
using EasyWPFUI.Automation.Peers;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls
{
    public class SplitButton : ContentControl
    {
        protected bool m_hasLoaded = false;
        private Button m_primaryButton;
        private Button m_secondaryButton;
        private bool m_isKeyDown = false;
        private bool m_isFlyoutOpen = false;

        private DependencyPropertyDescriptor m_pressedPrimary = DependencyPropertyDescriptor.FromProperty(ButtonBase.IsPressedProperty, typeof(Button));
        private DependencyPropertyDescriptor m_pointerOverPrimary = DependencyPropertyDescriptor.FromProperty(ButtonBase.IsMouseOverProperty, typeof(Button));
        private DependencyPropertyDescriptor m_pressedSecondary = DependencyPropertyDescriptor.FromProperty(ButtonBase.IsPressedProperty, typeof(Button));
        private DependencyPropertyDescriptor m_pointerOverSecondary = DependencyPropertyDescriptor.FromProperty(ButtonBase.IsMouseOverProperty, typeof(Button));
        private DependencyPropertyDescriptor m_flyoutPlacement = DependencyPropertyDescriptor.FromProperty(FlyoutBase.PlacementProperty, typeof(FlyoutBase));

        #region Events

        public event TypedEventHandler<SplitButton, SplitButtonClickEventArgs> Click;

        #endregion

        #region Flyout Property

        public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register("Flyout", typeof(FlyoutBase), typeof(SplitButton), new FrameworkPropertyMetadata(null, OnFlyoutPropertyChanged));

        public FlyoutBase Flyout
        {
            get
            {
                return (FlyoutBase)GetValue(FlyoutProperty);
            }
            set
            {
                SetValue(FlyoutProperty, value);
            }
        }

        private static void OnFlyoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton splitButton = d as SplitButton;

            if (splitButton == null)
                return;

            if (e.OldValue is FlyoutBase flyoutBase)
            {
                splitButton.UnregisterFlyoutEvents(flyoutBase);
            }

            if (e.NewValue is FlyoutBase)
            {
                splitButton.OnFlyoutChanged();
            }
        }

        #endregion

        #region Command Property

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new FrameworkPropertyMetadata(null, OnCommandPropertyChanged));

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CommandParameter Property

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new FrameworkPropertyMetadata(null, OnCommandParameterPropertyChanged));

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion


        #region Methods

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        public SplitButton()
        {
            KeyDown += OnSplitButtonKeyDown;
            KeyUp += OnSplitButtonKeyUp;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UnregisterEvents();

            m_primaryButton = GetTemplateChild("PrimaryButton") as Button;
            m_secondaryButton = GetTemplateChild("SecondaryButton") as Button;

            if (m_primaryButton != null)
            {
                m_primaryButton.Click += OnClickPrimary;

                m_pressedPrimary.AddValueChanged(m_primaryButton, OnVisualPropertyChanged);
                m_pointerOverPrimary.AddValueChanged(m_primaryButton, OnVisualPropertyChanged);
            }

            if (m_secondaryButton != null)
            {
                // Do localization for the secondary button
                string secondaryName = "";
                System.Windows.Automation.AutomationProperties.SetName(m_secondaryButton, secondaryName);

                m_secondaryButton.Click += OnClickSecondary;

                m_pressedSecondary.AddValueChanged(m_secondaryButton, OnVisualPropertyChanged);
                m_pointerOverSecondary.AddValueChanged(m_secondaryButton, OnVisualPropertyChanged);
            }

            // Register events on flyout
            RegisterFlyoutEvents();

            UpdateVisualStates();

            m_hasLoaded = true;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SplitButtonAutomationPeer(this);
        }

        internal void OpenFlyout()
        {
            if (Flyout != null)
            {
                Flyout.ShowAt(this);
            }
        }

        internal void CloseFlyout()
        {
            if (Flyout != null)
            {
                Flyout.Hide();
            }
        }

        internal virtual void OnClickPrimary(object sender, RoutedEventArgs e)
        {
            SplitButtonClickEventArgs eventArgs = new SplitButtonClickEventArgs();

            Click?.Invoke(this, eventArgs);

            AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);

            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
            }
        }

        internal bool IsFlyoutOpen()
        {
            return m_isFlyoutOpen;
        }

        internal virtual bool InternalIsChecked()
        {
            return false;
        }

        private void OnFlyoutChanged()
        {
            if(Flyout != null)
            {
                RegisterFlyoutEvents();

                UpdateVisualStates();
            }
        }

        private void RegisterFlyoutEvents()
        {
            if(Flyout != null)
            {
                Flyout.Opened += OnFlyoutOpened;
                Flyout.Closed += OnFlyoutClosed;

                m_flyoutPlacement.AddValueChanged(Flyout, OnFlyoutPlacementChanged);
            }
        }

        private void UnregisterFlyoutEvents(FlyoutBase flyout)
        {
            if (flyout != null)
            {
                flyout.Opened -= OnFlyoutOpened;
                flyout.Closed -= OnFlyoutClosed;

                m_flyoutPlacement.RemoveValueChanged(Flyout, OnFlyoutPlacementChanged);
            }
        }

        private void OnVisualPropertyChanged(object sender, EventArgs e)
        {
            UpdateVisualStates();
        }

        private void OnFlyoutPlacementChanged(object sender, EventArgs e)
        {
            UpdateVisualStates();
        }

        protected void UpdateVisualStates(bool useTransitions = true)
        {
            // place the secondary button
            if (m_isKeyDown)
            {
                VisualStateManager.GoToState(this, "SecondaryButtonSpan", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "SecondaryButtonRight", useTransitions);
            }

            if (m_primaryButton != null && m_secondaryButton != null)
            {
                if (m_isFlyoutOpen)
                {
                    VisualStateManager.GoToState(this, "FlyoutOpen", useTransitions);
                }
                // SplitButton and ToggleSplitButton share a template -- this section is driving the checked states for ToggleSplitButton.
                else if (InternalIsChecked())
                {
                    if (m_isKeyDown)
                    {
                        if (m_primaryButton.IsPressed || m_secondaryButton.IsPressed || m_isKeyDown)
                        {
                            VisualStateManager.GoToState(this, "CheckedTouchPressed", useTransitions);
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, "Checked", useTransitions);
                        }
                    }
                    else if (m_primaryButton.IsPressed)
                    {
                        VisualStateManager.GoToState(this, "CheckedPrimaryPressed", useTransitions);
                    }
                    else if (m_primaryButton.IsMouseOver)
                    {
                        VisualStateManager.GoToState(this, "CheckedPrimaryPointerOver", useTransitions);
                    }
                    else if (m_secondaryButton.IsPressed)
                    {
                        VisualStateManager.GoToState(this, "CheckedSecondaryPressed", useTransitions);
                    }
                    else if (m_secondaryButton.IsMouseOver)
                    {
                        VisualStateManager.GoToState(this, "CheckedSecondaryPointerOver", useTransitions);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Checked", useTransitions);
                    }
                }
                else
                {
                    if (m_isKeyDown)
                    {
                        if (m_primaryButton.IsPressed || m_secondaryButton.IsPressed || m_isKeyDown)
                        {
                            VisualStateManager.GoToState(this, "TouchPressed", useTransitions);
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, "Normal", useTransitions);
                        }
                    }
                    else if (m_primaryButton.IsPressed)
                    {
                        VisualStateManager.GoToState(this, "PrimaryPressed", useTransitions);
                    }
                    else if (m_primaryButton.IsMouseOver)
                    {
                        VisualStateManager.GoToState(this, "PrimaryPointerOver", useTransitions);
                    }
                    else if (m_secondaryButton.IsPressed)
                    {
                        VisualStateManager.GoToState(this, "SecondaryPressed", useTransitions);
                    }
                    else if (m_secondaryButton.IsMouseOver)
                    {
                        VisualStateManager.GoToState(this, "SecondaryPointerOver", useTransitions);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Normal", useTransitions);
                    }
                }
            }
        }

        private void UnregisterEvents()
        {
            // This explicitly unregisters all events related to the two buttons in OnApplyTemplate
            // in case the new template doesn't have all the expected elements.

            if (m_primaryButton != null)
            {
                m_primaryButton.Click += OnClickPrimary;
                m_pressedPrimary.RemoveValueChanged(m_primaryButton, OnVisualPropertyChanged);
                m_pointerOverPrimary.RemoveValueChanged(m_primaryButton, OnVisualPropertyChanged);
            }

            if (m_secondaryButton != null)
            {
                m_secondaryButton.Click += OnClickSecondary;
                m_pressedSecondary.RemoveValueChanged(m_secondaryButton, OnVisualPropertyChanged);
                m_pointerOverSecondary.RemoveValueChanged(m_secondaryButton, OnVisualPropertyChanged);
            }
        }

        private void OnSplitButtonKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space || e.Key == Key.Enter)
            {
                m_isKeyDown = true;
                UpdateVisualStates();
            }
        }

        private void OnSplitButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                m_isKeyDown = false;
                UpdateVisualStates();

                // Consider this a click on the primary button
                if (IsEnabled)
                {
                    OnClickPrimary(null, null);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Down)
            {
                bool menuKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

                if(IsEnabled && menuKeyDown)
                {
                    // Open the menu on alt-down
                    OpenFlyout();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.F4 && IsEnabled)
            {
                OpenFlyout();
                e.Handled = true;
            }
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            m_isFlyoutOpen = true;

            UpdateVisualStates();

            SharedHelpers.RaiseAutomationPropertyChangedEvent(this, System.Windows.Automation.ExpandCollapseState.Collapsed, System.Windows.Automation.ExpandCollapseState.Expanded);
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            m_isFlyoutOpen = false;

            UpdateVisualStates();

            SharedHelpers.RaiseAutomationPropertyChangedEvent(this, System.Windows.Automation.ExpandCollapseState.Expanded, System.Windows.Automation.ExpandCollapseState.Collapsed);
        }

        private void OnClickSecondary(object sender, RoutedEventArgs e)
        {
            OpenFlyout();
        }

        #endregion
    }
}
