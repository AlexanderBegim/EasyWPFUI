// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using EasyWPFUI.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateClosed)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateClosedCompactLeft)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateClosedCompactRight)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenOverlayLeft)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenOverlayRight)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenInlineLeft)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenInlineRight)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenCompactOverlayLeft)]
    [TemplateVisualState(GroupName = DisplayModeGroup, Name = StateOpenCompactOverlayRight)]
    [TemplatePart(Name = c_templatePaneClipRectangleName, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = c_templateLightDismissLayerName, Type = typeof(Rectangle))]
    [ContentProperty("Content")]
    [DefaultProperty("Content")]
    public class SplitView : Control
    {
        private const string DisplayModeGroup = "DisplayModeStates";
        private const string StateClosed = "Closed";
        private const string StateClosedCompactLeft = "ClosedCompactLeft";
        private const string StateClosedCompactRight = "ClosedCompactRight";
        private const string StateOpenOverlayLeft = "OpenOverlayLeft";
        private const string StateOpenOverlayRight = "OpenOverlayRight";
        private const string StateOpenInlineLeft = "OpenInlineLeft";
        private const string StateOpenInlineRight = "OpenInlineRight";
        private const string StateOpenCompactOverlayLeft = "OpenCompactOverlayLeft";
        private const string StateOpenCompactOverlayRight = "OpenCompactOverlayRight";
        private const string StateOverlayVisible = "OverlayVisible";
        private const string StateOverlayNotVisible = "OverlayNotVisible";

        private const string c_templateRootGridName = "RootGrid";
        private const string c_templatePaneClipRectangleName = "PaneClipRectangle";
        private const string c_templateLightDismissLayerName = "LightDismissLayer";
        private const string c_displayModeStates = "DisplayModeStates";

        private RectangleGeometry m_paneClipRectangle;
        private Rectangle m_lightDismissLayer;

        #region Pane Property

        public static readonly DependencyProperty PaneProperty = DependencyProperty.Register("Pane", typeof(UIElement), typeof(SplitView), new FrameworkPropertyMetadata(null));

        public UIElement Pane
        {
            get
            {
                return (UIElement)GetValue(PaneProperty);
            }
            set
            {
                SetValue(PaneProperty, value);
            }
        }

        #endregion

        #region IsPaneOpen Property

        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(SplitView), new FrameworkPropertyMetadata(false, OnIsPaneOpenChanged));

        public bool IsPaneOpen
        {
            get
            {
                return (bool)GetValue(IsPaneOpenProperty);
            }
            set
            {
                SetValue(IsPaneOpenProperty, value);
            }
        }

        private static void OnIsPaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            if ((bool)e.OldValue != (bool)e.NewValue)
            {
                splitView.OnIsPaneOpenChanged((bool)e.NewValue);
            }
        }

        #endregion

        #region CompactPaneLength Property

        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(SplitView), new FrameworkPropertyMetadata(48d, OnCompactPaneLengthChanged));

        public double CompactPaneLength
        {
            get
            {
                return (double)GetValue(CompactPaneLengthProperty);
            }
            set
            {
                SetValue(CompactPaneLengthProperty, value);
            }
        }

        private static void OnCompactPaneLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            splitView.ChangeTemplateSettings();

            splitView.CompactPaneLengthChanged?.Invoke(splitView, CompactPaneLengthProperty);
        }

        #endregion

        #region OpenPaneLength Property

        public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(SplitView), new FrameworkPropertyMetadata(320d, OnOpenPaneLengthChanged));

        public double OpenPaneLength
        {
            get
            {
                return (double)GetValue(OpenPaneLengthProperty);
            }
            set
            {
                SetValue(OpenPaneLengthProperty, value);
            }
        }

        private static void OnOpenPaneLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            splitView.ChangeTemplateSettings();
            splitView.ChangeVisualState();
        }

        #endregion

        #region PaneBackground Property

        public static readonly DependencyProperty PaneBackgroundProperty = DependencyProperty.Register("PaneBackground", typeof(Brush), typeof(SplitView), new FrameworkPropertyMetadata(null));

        public Brush PaneBackground
        {
            get
            {
                return (Brush)GetValue(PaneBackgroundProperty);
            }
            set
            {
                SetValue(PaneBackgroundProperty, value);
            }
        }

        #endregion

        #region PanePlacement Property

        public static readonly DependencyProperty PanePlacementProperty = DependencyProperty.Register("PanePlacement", typeof(SplitViewPanePlacement), typeof(SplitView), new FrameworkPropertyMetadata(SplitViewPanePlacement.Left, OnPanePlacementChanged));

        public SplitViewPanePlacement PanePlacement
        {
            get
            {
                return (SplitViewPanePlacement)GetValue(PanePlacementProperty);
            }
            set
            {
                SetValue(PanePlacementProperty, value);
            }
        }

        private static void OnPanePlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            splitView.ChangeVisualState();
        }

        #endregion

        #region Content Property

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(SplitView), new FrameworkPropertyMetadata(null));

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

        #region DisplayMode Property

        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(SplitViewDisplayMode), typeof(SplitView), new FrameworkPropertyMetadata(SplitViewDisplayMode.Overlay, OnDisplayModeChanged));

        public SplitViewDisplayMode DisplayMode
        {
            get
            {
                return (SplitViewDisplayMode)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            splitView.ChangeVisualState();

            splitView.DisplayModeChanged?.Invoke(splitView, DisplayModeProperty);
        }

        #endregion

        #region LightDissmissOverlayMode Property

        public static readonly DependencyProperty LightDissmissOverlayModeProperty = DependencyProperty.Register("LightDissmissOverlayMode", typeof(LightDissmissOverlayMode), typeof(SplitView), new FrameworkPropertyMetadata(LightDissmissOverlayMode.Auto, OnLightDissmissOverlayModeChanged));

        public LightDissmissOverlayMode LightDissmissOverlayMode
        {
            get
            {
                return (LightDissmissOverlayMode)GetValue(LightDissmissOverlayModeProperty);
            }
            set
            {
                SetValue(LightDissmissOverlayModeProperty, value);
            }
        }

        private static void OnLightDissmissOverlayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitView splitView = (SplitView)d;

            splitView.ChangeVisualState();
        }

        #endregion

        #region TemplateSettings Property

        private static readonly DependencyPropertyKey TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(SplitViewTemplateSettings), typeof(SplitView), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;

        public SplitViewTemplateSettings TemplateSettings
        {
            get
            {
                return (SplitViewTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            private set
            {
                SetValue(TemplateSettingsPropertyKey, value);
            }
        }

        #endregion

        #region Methods

        static SplitView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitView), new FrameworkPropertyMetadata(typeof(SplitView)));
        }

        public SplitView()
        {
            TemplateSettings = new SplitViewTemplateSettings(this);
            TemplateSettings.Update();
        }

        public override void OnApplyTemplate()
        {
            if(m_lightDismissLayer != null)
            {
                m_lightDismissLayer.PreviewMouseDown -= OnLightDissmiss;
                m_lightDismissLayer.PreviewTouchDown -= OnLightDissmiss;
                m_lightDismissLayer.PreviewStylusDown -= OnLightDissmiss;
            }

            base.OnApplyTemplate();

            m_paneClipRectangle = GetTemplateChild(c_templatePaneClipRectangleName) as RectangleGeometry;

            if (GetTemplateChild(c_templateLightDismissLayerName) is Rectangle lightDismissLayer)
            {
                m_lightDismissLayer = lightDismissLayer;

                m_lightDismissLayer.PreviewMouseDown += OnLightDissmiss;
                m_lightDismissLayer.PreviewTouchDown += OnLightDissmiss;
                m_lightDismissLayer.PreviewStylusDown += OnLightDissmiss;
            }

            ChangeTemplateSettings();

            ChangeVisualState(false);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (GetTemplateChild(c_templateRootGridName) is Grid rootGrid && GetTemplateChild(c_displayModeStates) is VisualStateGroup displayModeStates)
                {
                    Storyboard storyboard = displayModeStates?.CurrentState?.Storyboard;
                    if (storyboard != null && rootGrid != null)
                    {
                        storyboard.Begin(rootGrid, true);
                    }
                }
            }), DispatcherPriority.Render);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);

            if(IsPaneOpen)
            {
                CoerceValue(OpenPaneLengthProperty);
            }

            if (m_paneClipRectangle != null)
            {
                m_paneClipRectangle.Rect = new Rect(0, 0, OpenPaneLength, ActualHeight);
            }
        }

        private void OnLightDissmiss(object sender, System.Windows.Input.StylusDownEventArgs e)
        {
            OnLightDissmiss();
        }

        private void OnLightDissmiss(object sender, System.Windows.Input.TouchEventArgs e)
        {
            OnLightDissmiss();
        }

        private void OnLightDissmiss(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnLightDissmiss();
        }

        private void OnLightDissmiss()
        {
            if(IsPaneOpen && DisplayMode == SplitViewDisplayMode.Overlay || DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                IsPaneOpen = false;
            }
        }

        private bool ChangeVisualState(bool useTransitions = true)
        {
            if (m_paneClipRectangle != null)
            {
                m_paneClipRectangle.Rect = new Rect(0, 0, OpenPaneLength, ActualHeight);
            }

            string stateName = string.Empty;

            if (IsPaneOpen)
            {
                switch (DisplayMode)
                {
                    case SplitViewDisplayMode.Inline:
                    case SplitViewDisplayMode.CompactInline:
                        {
                            stateName = (PanePlacement == SplitViewPanePlacement.Left) ? StateOpenInlineLeft : StateOpenInlineRight;
                            break;
                        }
                    case SplitViewDisplayMode.Overlay:
                        {
                            stateName = (PanePlacement == SplitViewPanePlacement.Left) ? StateOpenOverlayLeft : StateOpenOverlayRight;
                            break;
                        }
                    case SplitViewDisplayMode.CompactOverlay:
                        {
                            stateName = (PanePlacement == SplitViewPanePlacement.Left) ? StateOpenCompactOverlayLeft : StateOpenCompactOverlayRight;
                            break;
                        }
                }
            }
            else
            {
                if (DisplayMode == SplitViewDisplayMode.CompactInline || DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    stateName = (PanePlacement == SplitViewPanePlacement.Left) ? StateClosedCompactLeft : StateClosedCompactRight;
                }
                else
                {
                    stateName = StateClosed;
                }
            }

            bool result = VisualStateManager.GoToState(this, stateName, useTransitions);

            System.Threading.Thread.Sleep(30); // TODO

            if(IsPaneOpen && (DisplayMode == SplitViewDisplayMode.Overlay || DisplayMode == SplitViewDisplayMode.CompactOverlay) && LightDissmissOverlayMode == LightDissmissOverlayMode.On)
            {
                stateName = StateOverlayVisible;
            }
            else
            {
                stateName = StateOverlayNotVisible;
            }

            VisualStateManager.GoToState(this, stateName, useTransitions);
            
            return result;
        }

        private void ChangeTemplateSettings()
        {
            TemplateSettings?.Update();
        }

        private void OnIsPaneOpenChanged(bool value)
        {
            if (value)
            {
                PaneOpening?.Invoke(this, new EventArgs());

                ChangeVisualState();

                PaneOpened?.Invoke(this, new EventArgs());
            }
            else
            {
                if (PaneClosing != null)
                {
                    SplitViewPaneClosingEventArgs args = new SplitViewPaneClosingEventArgs();

                    PaneClosing.Invoke(this, args);

                    if (args.Cancel && IsPaneOpen)
                        return;
                }

                ChangeVisualState();

                PaneClosed?.Invoke(this, new EventArgs());
            }

            IsPaneOpenChanged?.Invoke(this, IsPaneOpenProperty);
        }

        #region Events

        public event TypedEventHandler<SplitView, object> PaneOpening;
        public event TypedEventHandler<SplitView, object> PaneOpened;
        public event TypedEventHandler<SplitView,SplitViewPaneClosingEventArgs> PaneClosing;
        public event TypedEventHandler<SplitView, object> PaneClosed;

        internal event TypedEventHandler<DependencyObject, DependencyProperty> IsPaneOpenChanged;
        internal event TypedEventHandler<DependencyObject, DependencyProperty> DisplayModeChanged;
        internal event TypedEventHandler<DependencyObject, DependencyProperty> CompactPaneLengthChanged;

        #endregion

        #endregion
    }
}
