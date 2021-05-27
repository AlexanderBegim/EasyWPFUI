// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    [TemplatePart(Name = s_LayoutRootName, Type = typeof(Grid))]
    [TemplatePart(Name = s_DeterminateProgressBarIndicatorName, Type = typeof(Rectangle))]
    [TemplatePart(Name = s_IndeterminateProgressBarIndicatorName, Type = typeof(Rectangle))]
    [TemplatePart(Name = s_IndeterminateProgressBarIndicator2Name, Type = typeof(Rectangle))]
    [TemplateVisualState(GroupName = "CommonStates", Name = s_DeterminateStateName)]
    [TemplateVisualState(GroupName = "CommonStates", Name = s_IndeterminateStateName)]
    public class ProgressBar : RangeBase
    {
        private const string s_LayoutRootName = "LayoutRoot";
        private const string s_DeterminateProgressBarIndicatorName = "DeterminateProgressBarIndicator";
        private const string s_IndeterminateProgressBarIndicatorName = "IndeterminateProgressBarIndicator";
        private const string s_IndeterminateProgressBarIndicator2Name = "IndeterminateProgressBarIndicator2";
        private const string s_ErrorStateName = "Error";
        private const string s_PausedStateName = "Paused";
        private const string s_IndeterminateStateName = "Indeterminate";
        private const string s_IndeterminateErrorStateName = "IndeterminateError";
        private const string s_IndeterminatePausedStateName = "IndeterminatePaused";
        private const string s_DeterminateStateName = "Determinate";
        private const string s_UpdatingStateName = "Updating";

        private Grid m_layoutRoot;
        private Rectangle m_determinateProgressBarIndicator;
        private Rectangle m_indeterminateProgressBarIndicator;
        private Rectangle m_indeterminateProgressBarIndicator2;

        #region Orientation Property

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ProgressBar), new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.RefreshAnimation();
        }

        #endregion

        #region IsIndeterminate Property

        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressBar), new FrameworkPropertyMetadata(false, OnIsindeterminatePropertyChanged));

        public bool IsIndeterminate
        {
            get
            {
                return (bool)GetValue(IsIndeterminateProperty);
            }
            set
            {
                SetValue(IsIndeterminateProperty, value);
            }
        }

        private static void OnIsindeterminatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.SetProgressBarIndicatorWidth();
            progressBar.UpdateStates();
            progressBar.RefreshAnimation();
        }

        #endregion

        #region UseMetroIndeterminateStyle Property

        public static readonly DependencyProperty UseMetroIndeterminateStyleProperty = DependencyProperty.Register("UseMetroIndeterminateStyle", typeof(bool), typeof(ProgressBar), new FrameworkPropertyMetadata(false, OnUseMetroIndeterminateStylePropertyChanged));

        public bool UseMetroIndeterminateStyle
        {
            get
            {
                return (bool)GetValue(UseMetroIndeterminateStyleProperty);
            }
            set
            {
                SetValue(UseMetroIndeterminateStyleProperty, value);
            }
        }

        private static void OnUseMetroIndeterminateStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ShowError Property

        public static readonly DependencyProperty ShowErrorProperty = DependencyProperty.Register("ShowError", typeof(bool), typeof(ProgressBar), new FrameworkPropertyMetadata(false, OnShowErrorPropertyChanged));

        public bool ShowError
        {
            get
            {
                return (bool)GetValue(ShowErrorProperty);
            }
            set
            {
                SetValue(ShowErrorProperty, value);
            }
        }

        private static void OnShowErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.UpdateStates();
        }

        #endregion

        #region ShowPaused Property

        public static readonly DependencyProperty ShowPausedProperty = DependencyProperty.Register("ShowPaused", typeof(bool), typeof(ProgressBar), new FrameworkPropertyMetadata(false, OnShowPausedPropertyChanged));

        public bool ShowPaused
        {
            get
            {
                return (bool)GetValue(ShowPausedProperty);
            }
            set
            {
                SetValue(ShowPausedProperty, value);
            }
        }

        private static void OnShowPausedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.UpdateStates();
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ProgressBar), new FrameworkPropertyMetadata(new CornerRadius(2)));

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

        #endregion

        #region TemplateSettings Property

        private static readonly DependencyPropertyKey TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(ProgressBarTemplateSettings), typeof(ProgressBar), null);

        public static readonly DependencyProperty TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;

        public ProgressBarTemplateSettings TemplateSettings
        {
            get
            {
                return (ProgressBarTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            private set
            {
                SetValue(TemplateSettingsPropertyKey, value);
            }
        }

        #endregion

        #region Methods

        static ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(typeof(ProgressBar)));
            PaddingProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(OnPaddingChanged));
            TemplateProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(OnTemplateChanged));
        }

        public ProgressBar()
        {
            SizeChanged += OnSizeChanged;

            SetValue(TemplateSettingsPropertyKey, new ProgressBarTemplateSettings());
        }

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.SetProgressBarIndicatorWidth();
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProgressBar progressBar = (ProgressBar)d;

            progressBar.RefreshAnimation();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetProgressBarIndicatorWidth();
            UpdateWidthBasedTemplateSettings();

            if (IsIndeterminate)
                RefreshAnimation();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_layoutRoot = (Grid)GetTemplateChild(s_LayoutRootName);
            m_determinateProgressBarIndicator = (Rectangle)GetTemplateChild(s_DeterminateProgressBarIndicatorName);
            m_indeterminateProgressBarIndicator = (Rectangle)GetTemplateChild(s_IndeterminateProgressBarIndicatorName);
            m_indeterminateProgressBarIndicator2 = (Rectangle)GetTemplateChild(s_IndeterminateProgressBarIndicator2Name);

            UpdateStates();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            SetProgressBarIndicatorWidth();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            SetProgressBarIndicatorWidth();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SetProgressBarIndicatorWidth();
        }

        private void UpdateStates(bool useTransition = true)
        {
            if (ShowError && IsIndeterminate)
            {
                VisualStateManager.GoToState(this, s_IndeterminateErrorStateName, useTransition);
            }
            else if (ShowError)
            {
                VisualStateManager.GoToState(this, s_ErrorStateName, useTransition);
            }
            else if (ShowPaused && IsIndeterminate)
            {
                VisualStateManager.GoToState(this, s_IndeterminatePausedStateName, useTransition);
            }
            else if (ShowPaused)
            {
                VisualStateManager.GoToState(this, s_PausedStateName, useTransition);
            }
            else if (IsIndeterminate)
            {
                UpdateWidthBasedTemplateSettings();
                VisualStateManager.GoToState(this, s_IndeterminateStateName, useTransition);
            }
            else if (!IsIndeterminate)
            {
                VisualStateManager.GoToState(this, s_DeterminateStateName, useTransition);
            }
        }

        private void RefreshAnimation()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                VisualStateManager.GoToState(this, s_UpdatingStateName, false);
                UpdateStates(false);
            }), DispatcherPriority.Render);
        }

        private void SetProgressBarIndicatorWidth()
        {
            Grid progressBar = m_layoutRoot;
            Rectangle determinateProgressBarIndicator = m_determinateProgressBarIndicator;

            if (progressBar != null && determinateProgressBarIndicator != null)
            {
                double progressBarWidth = progressBar.ActualWidth;
                double prevIndicatorWidth = determinateProgressBarIndicator.ActualWidth;
                double maximum = Maximum;
                double minimum = Minimum;
                Thickness padding = Padding;

                // Adds "Updating" state in between to trigger RepositionThemeAnimation Visual Transition
                // in ProgressBar.xaml when reverting back to previous state
                VisualStateManager.GoToState(this, s_UpdatingStateName, true);

                if (IsIndeterminate)
                {
                    determinateProgressBarIndicator.Width = 0;

                    if (m_indeterminateProgressBarIndicator != null)
                    {
                        m_indeterminateProgressBarIndicator.Width = progressBarWidth * 0.4; // 40% of ProgressBar Width
                    }

                    if (m_indeterminateProgressBarIndicator2 != null)
                    {
                        m_indeterminateProgressBarIndicator2.Width = progressBarWidth * 0.6; // 60% of ProgressBar Width
                    }
                }
                else if (Math.Abs(maximum - minimum) > double.Epsilon)
                {
                    double maxIndicatorWidth = progressBarWidth - (padding.Left + Padding.Right);
                    double increment = maxIndicatorWidth / (maximum - minimum);
                    double indicatorWidth = increment * (Value - minimum);
                    double widthDelta = indicatorWidth - prevIndicatorWidth;
                    TemplateSettings.IndicatorLengthDelta = -widthDelta;
                    m_determinateProgressBarIndicator.Width = indicatorWidth;
                }
                else
                {
                    determinateProgressBarIndicator.Width = 0; //Error;
                }

                UpdateStates();
            }
        }

        private void UpdateWidthBasedTemplateSettings()
        {
            double width = 0;
            double height = 0;

            if (m_layoutRoot != null)
            {
                width = m_layoutRoot.ActualWidth;
                height = m_layoutRoot.ActualHeight;
            }

            double indeterminateProgressBarIndicatorWidth = width * 0.4; // Indicator width at 40% of ProgressBar
            double indeterminateProgressBarIndicatorWidth2 = width * 0.6; // Indicator width at 60% of ProgressBar

            TemplateSettings.ContainerAnimationStartPosition = indeterminateProgressBarIndicatorWidth * -1.0; // Position at -100%
            TemplateSettings.ContainerAnimationEndPosition = indeterminateProgressBarIndicatorWidth * 3.0; // Position at 300%

            TemplateSettings.Container2AnimationStartPosition = indeterminateProgressBarIndicatorWidth2 * -1.5; // Position at -150%
            TemplateSettings.Container2AnimationEndPosition = indeterminateProgressBarIndicatorWidth2 * 1.66; // Position at 166%

            TemplateSettings.ContainerAnimationMidPosition = width * 0.2;

            RectangleGeometry rectangle = new RectangleGeometry(new Rect(Padding.Left, Padding.Top, width - (Padding.Right + Padding.Left), height - (Padding.Bottom + Padding.Top)));

            TemplateSettings.ClipRect = rectangle;

            // TemplateSetting properties from WUXC for backwards compatibility.
            TemplateSettings.EllipseAnimationEndPosition = width * 2.0 / 3.0;
            TemplateSettings.EllipseAnimationWellPosition = width * 1.0 / 3.0;

            if (width <= 180.0)
            {
                // Small ellipse diameter and offset.
                TemplateSettings.EllipseDiameter = 4.0;
                TemplateSettings.EllipseOffset = 4.0;

                if(UseMetroIndeterminateStyle)
                {
                    TemplateSettings.ContainerAnimationStartPosition = -34;
                    TemplateSettings.ContainerAnimationEndPosition = (0.4352 * width) - 25.731;
                }
            }
            else if (width <= 280)
            {
                // Medium ellipse diameter and offset.
                TemplateSettings.EllipseDiameter = 5.0;
                TemplateSettings.EllipseOffset = 7.0;

                if(UseMetroIndeterminateStyle)
                {
                    TemplateSettings.ContainerAnimationStartPosition = 50.5;
                    TemplateSettings.ContainerAnimationEndPosition = (0.4352 * width) + 27.84;
                }
            }
            else
            {
                // Large ellipse diameter and offset.
                TemplateSettings.EllipseDiameter = 6.0;
                TemplateSettings.EllipseOffset = 9.0;

                if(UseMetroIndeterminateStyle)
                {
                    TemplateSettings.ContainerAnimationStartPosition = -63;
                    TemplateSettings.ContainerAnimationEndPosition = (0.4352 * width) - 58.862;
                }
            }
        }

        #endregion
    }
}
