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
using EasyWPFUI.Automation.Peers;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    public class ProgressRing : Control
    {
        private const string s_ActiveStateName = "Active";
        private const string s_InactiveStateName = "Inactive";
        private const string s_SmallStateName = "Small";
        private const string s_LargeStateName = "Large";

        #region IsActive Property

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new FrameworkPropertyMetadata(false, OnIsActiveChanged));

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressRing progressRing = (ProgressRing)d;

            progressRing.UpdateStates();
        }

        #endregion

        #region TemplateSettings Property

        public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register("TemplateSettings", typeof(ProgressRingTemplateSettings), typeof(ProgressRing), new FrameworkPropertyMetadata(null));

        public ProgressRingTemplateSettings TemplateSettings
        {
            get
            {
                return (ProgressRingTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            private set
            {
                SetValue(TemplateSettingsProperty, value);
            }
        }

        #endregion

        #region Methods

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
        }

        public ProgressRing()
        {
            TemplateSettings = new ProgressRingTemplateSettings();

            SizeChanged += OnSizeChanged;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ProgressRingAutomationPeer(this);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplytemplateSettings();
            UpdateStates();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateStates();
        }

        private void UpdateStates()
        {
            VisualStateManager.GoToState(this, (IsActive) ? s_ActiveStateName : s_InactiveStateName, true);
            VisualStateManager.GoToState(this, (TemplateSettings.MaxSideLength < 60) ? s_SmallStateName : s_LargeStateName, true);
        }

        private void ApplytemplateSettings()
        {
            double width = ActualWidth;
            double diameterValue = 0;
            double anchorPoint = 0;

            if (ActualWidth != 0)
            {
                double diameterAdditive = 0;
                diameterAdditive = (width <= 40.0) ? 1.0 : 0.0;
                diameterValue = (width * 0.1d) + diameterAdditive;
                anchorPoint = (width * 0.5d) - diameterValue;
            }

            TemplateSettings.EllipseDiameter = diameterValue;
            TemplateSettings.EllipseOffset = new Thickness(0, anchorPoint, 0, 0);
            TemplateSettings.MaxSideLength = width;
        }

        #endregion
    }
}
