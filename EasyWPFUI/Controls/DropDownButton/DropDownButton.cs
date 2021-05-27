// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Automation.Peers;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls
{
    public class DropDownButton : Button
    {
        private bool m_isFlyoutOpen = false;

        #region Flyout Property

        public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register("Flyout", typeof(FlyoutBase), typeof(DropDownButton), new FrameworkPropertyMetadata(null, OnFlyoutPropertyChanged));

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
            DropDownButton dropDownButton = d as DropDownButton;

            if (dropDownButton == null)
                return;

            if (e.OldValue is FlyoutBase oldFlyout)
            {
                dropDownButton.UnregisterFlyoutEvents(oldFlyout);
            }

            if (e.NewValue is FlyoutBase newFlyout)
            {
                dropDownButton.RegisterFlyoutEvents(newFlyout);
            }
        }

        #endregion

        #region IsFlyoutOpen Property

        public bool IsFlyoutOpen
        {
            get
            {
                return m_isFlyoutOpen;
            }
            private set
            {
                m_isFlyoutOpen = value;
            }
        }

        #endregion

        #region Methods

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public DropDownButton()
        {
            Click += OnClick;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DropDownButtonAutomationPeer(this);
        }

        public void OpenFlyout()
        {
            if (Flyout != null)
            {
                Flyout.ShowAt(this);
            }
        }

        public void CloseFlyout()
        {
            if (Flyout != null)
            {
                Flyout.Hide();
            }
        }

        private void UnregisterFlyoutEvents(FlyoutBase flyout)
        {
            flyout.Opened -= OnFlyoutOpened;
            flyout.Closed -= OnFlyoutClosed;
        }

        private void RegisterFlyoutEvents(FlyoutBase flyout)
        {
            flyout.Opened += OnFlyoutOpened;
            flyout.Closed += OnFlyoutClosed;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (Flyout != null)
            {
                Button button = (Button)sender;

                Flyout.ShowAt(button);
            }
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            m_isFlyoutOpen = false;
            SharedHelpers.RaiseAutomationPropertyChangedEvent(this, ExpandCollapseState.Expanded, ExpandCollapseState.Collapsed);
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            m_isFlyoutOpen = true;
            SharedHelpers.RaiseAutomationPropertyChangedEvent(this, ExpandCollapseState.Collapsed, ExpandCollapseState.Expanded);
        }

        #endregion
    }
}
