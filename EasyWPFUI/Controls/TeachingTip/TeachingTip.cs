// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using EasyWPFUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public enum TeachingTipTailVisibility
    {
        Auto,
        Visible,
        Collapsed,
    }

    public enum TeachingTipCloseReason
    {
        CloseButton,
        LightDismiss,
        Programmatic,
    }

    public enum TeachingTipPlacementMode
    {
        Auto,
        Top,
        Bottom,
        Left,
        Right,
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft,
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom,
        Center
    }

    public enum TeachingTipHeroContentPlacementMode
    {
        Auto,
        Top,
        Bottom,
    }

    [ContentProperty(nameof(Content))]
    public class TeachingTip : ContentControl
    {
        #region Elements Names


        private const string s_containerName = "Container";
        private const string s_containerGridName = "ContainerGrid";
        private const string s_tailOcclusionGridName = "TailOcclusionGrid";
        private const string s_contentRootGridName = "ContentRootGrid";
        private const string s_nonHeroContentRootGridName = "NonHeroContentRootGrid";
        private const string s_heroContentBorderName = "HeroContentBorder";
        private const string s_alternateCloseButtonName = "AlternateCloseButton";
        private const string s_actionButtonName = "ActionButton";
        private const string s_closeButtonName = "CloseButton";
        private const string s_tailPolygonName = "TailPolygon";
        private const string s_tailEdgeBorderName = "TailEdgeBorder";

        private const string c_TitleTextBlockVisibleStateName = "ShowTitleTextBlock";
        private const string c_TitleTextBlockCollapsedStateName = "CollapseTitleTextBlock";
        private const string c_SubtitleTextBlockVisibleStateName = "ShowSubtitleTextBlock";
        private const string c_SubtitleTextBlockCollapsedStateName = "CollapseSubtitleTextBlock";

        #endregion

        #region Elements

        private FrameworkElement m_target;

        private Border m_container;
        private Grid m_containerGrid;

        private Popup m_popup;

        private UIElement m_rootElement;
        private Grid m_tailOcclusionGrid;
        private Grid m_contentRootGrid;
        private Grid m_nonHeroContentRootGrid;
        private Border m_heroContentBorder;
        private Button m_actionButton;
        private Button m_alternateCloseButton;
        private Button m_closeButton;
        private Polygon m_tailPolygon;
        private Grid m_tailEdgeBorder;

        private Window m_parentWindow;
        private Thickness m_parentWindowBorder;

        private Storyboard m_openAnimationStoryboard;
        private Storyboard m_closeAnimationStoryboard;
        private TimeSpan m_expandAnimationDuration = TimeSpan.FromMilliseconds(100);

        #endregion

        #region Fields

        private bool m_isTemplateApplied = false;
        private bool m_tipFollowsTarget = false;
        private bool m_returnTopForOutOfWindowPlacement = true;

        private Rect m_currentTargetBoundsInCoreWindowSpace = default;

        private TeachingTipCloseReason m_lastCloseReason = TeachingTipCloseReason.Programmatic;

        //Ideally this would be computed from layout but it is difficult to do.
        private const double s_tailOcclusionAmount = 2;

        //It is possible this should be exposed as a property, but you can adjust what it does with margin.
        private double s_untargetedTipWindowEdgeMargin; // = 24;

        private TeachingTipPlacementMode m_currentEffectiveTipPlacementMode = TeachingTipPlacementMode.Auto;
        private TeachingTipPlacementMode m_currentEffectiveTailPlacementMode = TeachingTipPlacementMode.Auto;
        private TeachingTipHeroContentPlacementMode m_currentHeroContentEffectivePlacementMode = TeachingTipHeroContentPlacementMode.Auto;

        #endregion

        #region Events

        public event TypedEventHandler<TeachingTip, object> ActionButtonClick;
        public event TypedEventHandler<TeachingTip, object> CloseButtonClick;
        public event TypedEventHandler<TeachingTip, TeachingTipClosingEventArgs> Closing;
        public event TypedEventHandler<TeachingTip, TeachingTipClosedEventArgs> Closed;

        #endregion


        #region IsOpen Property

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(TeachingTip), new FrameworkPropertyMetadata(false, OnIsOpenPropertyChanged));

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

        }

        #endregion

        #region Target Property

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(TeachingTip), new FrameworkPropertyMetadata(OnTargetPropertyChanged));

        public FrameworkElement Target
        {
            get
            {
                return (FrameworkElement)GetValue(TargetProperty);
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region TailVisibility Property

        public static readonly DependencyProperty TailVisibilityProperty = DependencyProperty.Register("TailVisibility", typeof(TeachingTipTailVisibility), typeof(TeachingTip), new FrameworkPropertyMetadata(TeachingTipTailVisibility.Auto, OnTailVisibilityPropertyChanged));

        public TeachingTipTailVisibility TailVisibility
        {
            get
            {
                return (TeachingTipTailVisibility)GetValue(TailVisibilityProperty);
            }
            set
            {
                SetValue(TailVisibilityProperty, value);
            }
        }

        private static void OnTailVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region Title Property

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TeachingTip), new FrameworkPropertyMetadata(OnTitlePropertyChanged));

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

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region Subtitle Property

        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register("Subtitle", typeof(string), typeof(TeachingTip), new FrameworkPropertyMetadata(OnSubtitlePropertyChanged));

        public string Subtitle
        {
            get
            {
                return (string)GetValue(SubtitleProperty);
            }
            set
            {
                SetValue(SubtitleProperty, value);
            }
        }

        private static void OnSubtitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ActionButtonContent Property

        public static readonly DependencyProperty ActionButtonContentProperty = DependencyProperty.Register("ActionButtonContent", typeof(object), typeof(TeachingTip), new FrameworkPropertyMetadata(OnActionButtonContentPropertyChanged));

        public object ActionButtonContent
        {
            get
            {
                return GetValue(ActionButtonContentProperty);
            }
            set
            {
                SetValue(ActionButtonContentProperty, value);
            }
        }

        private static void OnActionButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ActionButtonStyle Property

        public static readonly DependencyProperty ActionButtonStyleProperty = DependencyProperty.Register("ActionButtonStyle", typeof(Style), typeof(TeachingTip), new FrameworkPropertyMetadata(OnActionButtonStylePropertyChanged));

        public Style ActionButtonStyle
        {
            get
            {
                return (Style)GetValue(ActionButtonStyleProperty);
            }
            set
            {
                SetValue(ActionButtonStyleProperty, value);
            }
        }

        private static void OnActionButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ActionButtonCommand Property

        public static readonly DependencyProperty ActionButtonCommandProperty = DependencyProperty.Register("ActionButtonCommand", typeof(ICommand), typeof(TeachingTip), new FrameworkPropertyMetadata(OnActionButtonCommandPropertyChanged));

        public ICommand ActionButtonCommand
        {
            get
            {
                return (ICommand)GetValue(ActionButtonCommandProperty);
            }
            set
            {
                SetValue(ActionButtonCommandProperty, value);
            }
        }

        private static void OnActionButtonCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ActionButtonCommandParameter Property

        public static readonly DependencyProperty ActionButtonCommandParameterProperty = DependencyProperty.Register("ActionButtonCommandParameter", typeof(object), typeof(TeachingTip), new FrameworkPropertyMetadata(OnActionButtonCommandParameterPropertyChanged));

        public object ActionButtonCommandParameter
        {
            get
            {
                return GetValue(ActionButtonCommandParameterProperty);
            }
            set
            {
                SetValue(ActionButtonCommandParameterProperty, value);
            }
        }

        private static void OnActionButtonCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CloseButtonContent Property

        public static readonly DependencyProperty CloseButtonContentProperty = DependencyProperty.Register("CloseButtonContent", typeof(object), typeof(TeachingTip), new FrameworkPropertyMetadata(OnCloseButtonContentPropertyChanged));

        public object CloseButtonContent
        {
            get
            {
                return GetValue(CloseButtonContentProperty);
            }
            set
            {
                SetValue(CloseButtonContentProperty, value);
            }
        }

        private static void OnCloseButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CloseButtonStyle Property

        public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(TeachingTip), new FrameworkPropertyMetadata(OnCloseButtonStylePropertyChanged));

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

        private static void OnCloseButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ActionButtonCommand Property

        public static readonly DependencyProperty CloseButtonCommandProperty = DependencyProperty.Register("CloseButtonCommand", typeof(ICommand), typeof(TeachingTip), new FrameworkPropertyMetadata(OnCloseButtonCommandPropertyChanged));

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

        private static void OnCloseButtonCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CloseButtonCommandParameter Property

        public static readonly DependencyProperty CloseButtonCommandParameterProperty = DependencyProperty.Register("CloseButtonCommandParameter", typeof(object), typeof(TeachingTip), new FrameworkPropertyMetadata(OnCloseButtonCommandParameterPropertyChanged));

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

        private static void OnCloseButtonCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region PlacementMargin Property

        public static readonly DependencyProperty PlacementMarginProperty = DependencyProperty.Register("PlacementMargin", typeof(Thickness), typeof(TeachingTip), new FrameworkPropertyMetadata(OnPlacementMarginPropertyChanged));

        public Thickness PlacementMargin
        {
            get
            {
                return (Thickness)GetValue(PlacementMarginProperty);
            }
            set
            {
                SetValue(PlacementMarginProperty, value);
            }
        }

        private static void OnPlacementMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ShouldConstrainToRootBounds Property

        public static readonly DependencyProperty ShouldConstrainToRootBoundsProperty = DependencyProperty.Register("ShouldConstrainToRootBounds", typeof(bool), typeof(TeachingTip), new FrameworkPropertyMetadata(true, OnShouldConstrainToRootBoundsPropertyChanged));

        public bool ShouldConstrainToRootBounds
        {
            get
            {
                return (bool)GetValue(ShouldConstrainToRootBoundsProperty);
            }
            set
            {
                SetValue(ShouldConstrainToRootBoundsProperty, value);
            }
        }

        private static void OnShouldConstrainToRootBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IsLightDismissEnabled Property

        public static readonly DependencyProperty IsLightDismissEnabledProperty = DependencyProperty.Register("IsLightDismissEnabled", typeof(bool), typeof(TeachingTip), new FrameworkPropertyMetadata(false, OnIsLightDismissEnabledPropertyChanged));

        public bool IsLightDismissEnabled
        {
            get
            {
                return (bool)GetValue(IsLightDismissEnabledProperty);
            }
            set
            {
                SetValue(IsLightDismissEnabledProperty, value);
            }
        }

        private static void OnIsLightDismissEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region PreferredPlacement Property

        public static readonly DependencyProperty PreferredPlacementProperty = DependencyProperty.Register("PreferredPlacement", typeof(TeachingTipPlacementMode), typeof(TeachingTip), new FrameworkPropertyMetadata(TeachingTipPlacementMode.Auto, OnPreferredPlacementPropertyChanged));

        public TeachingTipPlacementMode PreferredPlacement
        {
            get
            {
                return (TeachingTipPlacementMode)GetValue(PreferredPlacementProperty);
            }
            set
            {
                SetValue(PreferredPlacementProperty, value);
            }
        }

        private static void OnPreferredPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region HeroContentPlacement Property

        public static readonly DependencyProperty HeroContentPlacementProperty = DependencyProperty.Register("HeroContentPlacement", typeof(TeachingTipHeroContentPlacementMode), typeof(TeachingTip), new FrameworkPropertyMetadata(TeachingTipHeroContentPlacementMode.Auto, OnHeroContentPlacementPropertyChanged));

        public TeachingTipHeroContentPlacementMode HeroContentPlacement
        {
            get
            {
                return (TeachingTipHeroContentPlacementMode)GetValue(HeroContentPlacementProperty);
            }
            set
            {
                SetValue(HeroContentPlacementProperty, value);
            }
        }

        private static void OnHeroContentPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region HeroContent Property

        public static readonly DependencyProperty HeroContentProperty = DependencyProperty.Register("HeroContent", typeof(UIElement), typeof(TeachingTip), new FrameworkPropertyMetadata(OnHeroContentPropertyChanged));

        public UIElement HeroContent
        {
            get
            {
                return (UIElement)GetValue(HeroContentProperty);
            }
            set
            {
                SetValue(HeroContentProperty, value);
            }
        }

        private static void OnHeroContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IconSource Property

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(IconSource), typeof(TeachingTip), new FrameworkPropertyMetadata(OnIconSourcePropertyChanged));

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

        }

        #endregion

        #region TemplateSettings Property

        public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register("TemplateSettings", typeof(TeachingTipTemplateSettings), typeof(TeachingTip), new FrameworkPropertyMetadata(OnTemplateSettingsPropertyChanged));

        public TeachingTipTemplateSettings TemplateSettings
        {
            get
            {
                return (TeachingTipTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            set
            {
                SetValue(TemplateSettingsProperty, value);
            }
        }

        private static void OnTemplateSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TeachingTip), new FrameworkPropertyMetadata());

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

        #region ActualPlacement Property

        public static readonly DependencyPropertyKey ActualPlacementPropertyKey = DependencyProperty.RegisterReadOnly("ActualPlacement", typeof(TeachingTipPlacementMode), typeof(TeachingTip), new FrameworkPropertyMetadata(TeachingTipPlacementMode.Auto));

        public static readonly DependencyProperty ActualPlacementProperty = ActualPlacementPropertyKey.DependencyProperty;

        public TeachingTipPlacementMode ActualPlacement
        {
            get
            {
                return (TeachingTipPlacementMode)GetValue(ActualPlacementProperty);
            }
            private set
            {
                SetValue(ActualPlacementPropertyKey, value);
            }
        }

        #endregion


        #region Methods

        static TeachingTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TeachingTip), new FrameworkPropertyMetadata(typeof(TeachingTip)));
        }

        public TeachingTip()
        {
            Unloaded += ClosePopupOnUnloadEvent;

            TemplateSettings = new TeachingTipTemplateSettings();

            ThemeManager.ApplicationThemeChanged += ThemeManagerApplicationThemeChanged;
        }

        ~TeachingTip()
        {
            ThemeManager.ApplicationThemeChanged -= ThemeManagerApplicationThemeChanged;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TeachingTipAutomationPeer(this);
        }

        public override void OnApplyTemplate()
        {
            if (m_tailOcclusionGrid != null)
            {
                m_tailOcclusionGrid.SizeChanged -= OnContentSizeChanged;
            }

            if (m_closeButton != null)
            {
                m_closeButton.Click -= OnCloseButtonClicked;
            }

            if (m_alternateCloseButton != null)
            {
                m_alternateCloseButton.Click -= OnCloseButtonClicked;
            }

            if (m_actionButton != null)
            {
                m_actionButton.Click -= OnActionButtonClicked;
            }

            if(m_target != null)
            {
                m_target.LayoutUpdated -= OnTargetLayoutUpdated;
            }

            base.OnApplyTemplate();

            m_container = GetTemplateChild(s_containerName) as Border;
            m_containerGrid = GetTemplateChild(s_containerGridName) as Grid;
            m_rootElement = m_container.Child;
            m_tailOcclusionGrid = GetTemplateChild(s_tailOcclusionGridName) as Grid;
            m_contentRootGrid = GetTemplateChild(s_contentRootGridName) as Grid;
            m_nonHeroContentRootGrid = GetTemplateChild(s_nonHeroContentRootGridName) as Grid;
            m_heroContentBorder = GetTemplateChild(s_heroContentBorderName) as Border;
            m_actionButton = GetTemplateChild(s_actionButtonName) as Button;
            m_alternateCloseButton = GetTemplateChild(s_alternateCloseButtonName) as Button;
            m_closeButton = GetTemplateChild(s_closeButtonName) as Button;
            m_tailEdgeBorder = GetTemplateChild(s_tailEdgeBorderName) as Grid;
            m_tailPolygon = GetTemplateChild(s_tailPolygonName) as Polygon;

            ToggleVisibilityForEmptyContent(c_TitleTextBlockVisibleStateName, c_TitleTextBlockCollapsedStateName, Title);
            ToggleVisibilityForEmptyContent(c_SubtitleTextBlockVisibleStateName, c_SubtitleTextBlockCollapsedStateName, Subtitle);

            m_parentWindow = Window.GetWindow(this);

            if (m_parentWindow == null && !DesignMode.IsInDesignMode)
            {
                throw new NullReferenceException("The Window cannot be empty.");
            }

            m_parentWindowBorder = GetParentWindowBorder();

            s_untargetedTipWindowEdgeMargin = (m_parentWindow is ModernWindow window) ? window.TitleBarHeight : SystemParameters.WindowCaptionHeight;

            if (m_container != null)
            {
                m_container.Child = null;
            }

            if(m_tailOcclusionGrid != null)
            {
                m_tailOcclusionGrid.SizeChanged += OnContentSizeChanged;
            }

            if(m_closeButton != null)
            {
                m_closeButton.Click += OnCloseButtonClicked;
            }

            if (m_alternateCloseButton != null)
            {
                AutomationProperties.SetName(m_alternateCloseButton, Properties.Resources.Strings.Resources.TeachingTipAlternateCloseButtonName);

                ToolTip toolTip = new ToolTip();
                toolTip.Content = Properties.Resources.Strings.Resources.TeachingTipAlternateCloseButtonTooltip;
                ToolTipService.SetToolTip(m_alternateCloseButton, toolTip);

                m_alternateCloseButton.Click += OnCloseButtonClicked;
            }

            if (m_actionButton != null)
            {
                m_actionButton.Click += OnActionButtonClicked;
            }

            UpdateButtonsState();
            OnIsLightDismissEnabledChanged();
            OnIconSourceChanged();
            OnHeroContentPlacementChanged();

            m_isTemplateApplied = true;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            DependencyProperty property = e.Property;

            if (property == IsOpenProperty)
            {
                OnIsOpenChanged();
            }
            else if (property == TargetProperty)
            {
                OnTargetChanged(e.OldValue as FrameworkElement, e.NewValue as FrameworkElement);
            }
            else if (property == ActionButtonContentProperty || property == CloseButtonContentProperty)
            {
                UpdateButtonsState();
            }
            else if (property == PlacementMarginProperty)
            {
                OnPlacementMarginChanged();
            }
            else if (property == IsLightDismissEnabledProperty)
            {
                OnIsLightDismissEnabledChanged();
            }
            else if (property == ShouldConstrainToRootBoundsProperty)
            {
                OnShouldConstrainToRootBoundsChanged();
            }
            else if (property == TailVisibilityProperty)
            {
                OnTailVisibilityChanged();
            }
            else if (property == PreferredPlacementProperty)
            {
                if (IsOpen)
                {
                    double contentWidth = (m_tailOcclusionGrid != null) ? m_tailOcclusionGrid.ActualWidth : 0;
                    double contentHeight = (m_tailOcclusionGrid != null) ? m_tailOcclusionGrid.ActualHeight : 0;

                    var (placement, ignored) = (m_target != null) ? DetermineEffectivePlacementTargeted(contentHeight, contentWidth) : DetermineEffectivePlacementUntargeted(contentHeight, contentWidth);

                    m_currentEffectiveTipPlacementMode = placement;

                    RepositionPopup();
                }
            }
            else if (property == HeroContentPlacementProperty)
            {
                OnHeroContentPlacementChanged();
            }
            else if (property == IconSourceProperty)
            {
                OnIconSourceChanged();
            }
            else if (property == TitleProperty)
            {
                SetPopupAutomationProperties();

                ToggleVisibilityForEmptyContent(c_TitleTextBlockVisibleStateName, c_TitleTextBlockCollapsedStateName, Title);
            }
            else if (property == SubtitleProperty)
            {
                ToggleVisibilityForEmptyContent(c_SubtitleTextBlockVisibleStateName, c_SubtitleTextBlockCollapsedStateName, Subtitle);
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (newContent != null)
            {
                VisualStateManager.GoToState(this, "Content", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoContent", false);
            }
        }

        /* Property Changed Methods */

        private void OnIsOpenChanged()
        {
            if (IsOpen)
            {
                IsOpenChangedToOpen();
            }
            else
            {
                IsOpenChangedToClose();
            }
        }

        private void OnTargetChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
            {
                oldValue.Unloaded -= ClosePopupOnUnloadEvent;
                oldValue.LayoutUpdated -= OnTargetLayoutUpdated;
                oldValue.Loaded -= OnTargetLoaded;
            }

            m_target = Target;

            m_target.Unloaded += ClosePopupOnUnloadEvent;
            m_target.Loaded += OnTargetLoaded;
            m_target.LayoutUpdated += OnTargetLayoutUpdated;

            if (IsOpen)
            {
                if (m_target != null)
                {
                    m_currentTargetBoundsInCoreWindowSpace = GetTargetBounds();

                    SetViewportChangedEvent(m_target);
                }

                PositionPopup();
            }
        }

        private void OnTailVisibilityChanged()
        {
            UpdateTail();
        }

        private void OnIconSourceChanged()
        {
            if (IconSource is IconSource source)
            {
                TemplateSettings.IconElement = SharedHelpers.MakeIconElementFrom(source);
                VisualStateManager.GoToState(this, "Icon", false);
            }
            else
            {
                TemplateSettings.IconElement = null;
                VisualStateManager.GoToState(this, "NoIcon", false);
            }
        }

        private void OnPlacementMarginChanged()
        {
            if (IsOpen)
            {
                RepositionPopup();
            }
        }

        private void OnIsLightDismissEnabledChanged()
        {
            if (IsLightDismissEnabled)
            {
                VisualStateManager.GoToState(this, "LightDismiss", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NormalDismiss", false);
            }

            UpdateButtonsState();
        }

        private void OnShouldConstrainToRootBoundsChanged()
        {

        }

        private void OnHeroContentPlacementChanged()
        {
            switch (HeroContentPlacement)
            {
                case TeachingTipHeroContentPlacementMode.Auto:
                    {
                        break;
                    }
                case TeachingTipHeroContentPlacementMode.Top:
                    {
                        UpdateDynamicHeroContentPlacementToTopImpl();
                        break;
                    }
                case TeachingTipHeroContentPlacementMode.Bottom:
                    {
                        UpdateDynamicHeroContentPlacementToBottomImpl();
                        break;
                    }
            }

            // Setting m_currentEffectiveTipPlacementMode to auto ensures that the next time position popup is called we'll rerun the DetermineEffectivePlacement
            // algorithm. If we did not do this and the popup was opened the algorithm would maintain the current effective placement mode, which we don't want
            // since the hero content placement contributes to the choice of tip placement mode.
            m_currentEffectiveTipPlacementMode = TeachingTipPlacementMode.Auto;

            if (IsOpen)
            {
                PositionPopup();
            }
        }

        /* Private Methods */

        private bool ToggleVisibilityForEmptyContent(string visibleStateName, string collapsedStateName, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                VisualStateManager.GoToState(this, visibleStateName, false);
                return true;
            }
            else
            {
                VisualStateManager.GoToState(this, collapsedStateName, false);
                return true;
            }
        }

        private void SetPopupAutomationProperties()
        {
            if (m_popup != null)
            {
                string name = AutomationProperties.GetName(this);

                if (string.IsNullOrEmpty(name))
                {
                    name = Title;
                }

                AutomationProperties.SetName(m_popup, name);

                AutomationProperties.SetAutomationId(m_popup, AutomationProperties.GetAutomationId(this));
            }
        }

        private void SetViewportChangedEvent(FrameworkElement target)
        {
            if (m_tipFollowsTarget)
            {
                target.LayoutUpdated += OnTargetLayoutUpdated;
            }
        }

        private void RaiseClosingEvent(bool attachDeferralCompletedHandler)
        {
            TeachingTipClosingEventArgs args = new TeachingTipClosingEventArgs();
            args.Reason = m_lastCloseReason;

            if (attachDeferralCompletedHandler)
            {
                if (!args.Cancel)
                {
                    // The developer has changed the Cancel property to true, indicating that they wish to Cancel the
                    // closing of this tip, so we need to revert the IsOpen property to true.
                    ClosePopupWithAnimationIfAvailable();
                }
                else
                {
                    IsOpen = true;
                }
            }
            else
            {
                Closing?.Invoke(this, args);
            }
        }

        private void ClosePopupWithAnimationIfAvailable()
        {
            if(m_popup != null && m_popup.IsOpen)
            {
                StartContractToClose();
            }
        }

        private void CreateOpenAnimation()
        {
            m_openAnimationStoryboard = new Storyboard();

            DoubleAnimation openOpacityAnimation = new DoubleAnimation(0, 1, m_expandAnimationDuration);

            m_openAnimationStoryboard.Children.Add(openOpacityAnimation);

            Storyboard.SetTargetProperty(openOpacityAnimation, new PropertyPath(Grid.OpacityProperty));
        }

        private void CreateCloseAnimation()
        {
            m_closeAnimationStoryboard = new Storyboard();

            DoubleAnimation closeOpacityAnimation = new DoubleAnimation(1, 0, m_expandAnimationDuration);

            m_closeAnimationStoryboard.Children.Add(closeOpacityAnimation);

            Storyboard.SetTargetProperty(closeOpacityAnimation, new PropertyPath(Grid.OpacityProperty));
        }

        private void StartOpenAnimation()
        {
            if (m_containerGrid != null && m_openAnimationStoryboard != null)
            {
                m_openAnimationStoryboard.Begin(m_containerGrid);
            }
        }

        private void StartContractToClose()
        {
            if (m_containerGrid != null && m_closeAnimationStoryboard != null)
            {
                m_closeAnimationStoryboard.Completed += OnCloseAnimationStoryboardCompleted;

                m_closeAnimationStoryboard.Begin(m_containerGrid);
            }
            else
            {
                ClosePopup();
            }
        }

        private void OnCloseAnimationStoryboardCompleted(object sender, EventArgs e)
        {
            m_closeAnimationStoryboard.Completed -= OnCloseAnimationStoryboardCompleted;

            ClosePopup();
        }

        /* Open And Close */

        private void IsOpenChangedToOpen()
        {
            //Reset the close reason to the default value of programmatic.
            m_lastCloseReason = TeachingTipCloseReason.Programmatic;

            if (m_target != null)
            {
                SetViewportChangedEvent(m_target);

                if(m_parentWindow == null)
                {
                    m_parentWindow = GetParentWindow();
                }

                m_currentTargetBoundsInCoreWindowSpace = GetTargetBounds();
            }
            else
            {
                m_currentTargetBoundsInCoreWindowSpace = default;
            }

            OnIsLightDismissEnabledChanged();

            if (m_openAnimationStoryboard == null)
            {
                CreateOpenAnimation();
            }

            if (m_closeAnimationStoryboard == null)
            {
                CreateCloseAnimation();
            }

            if (!m_isTemplateApplied)
            {
                this.ApplyTemplate();
            }

            if(m_popup == null)
            {
                CreateNewPopup();
            }

            // If the tip is not going to open because it does not fit we need to make sure that
            // the open, closing, closed life cycle still fires so that we don't cause apps to leak
            // that depend on this sequence.
            var (ignored, tipDoesNotFit) = DetermineEffectivePlacement();

            if (tipDoesNotFit)
            {
                RaiseClosingEvent(false);

                TeachingTipClosedEventArgs args = new TeachingTipClosedEventArgs();
                args.Reason = m_lastCloseReason;

                Closed?.Invoke(this, args);

                IsOpen = false;
            }
            else
            {
                if(m_popup != null)
                {
                    if(!m_popup.IsOpen)
                    {
                        m_popup.Child = m_rootElement;

                        m_popup.IsOpen = true;
                    }
                    else
                    {

                    }
                }
            }

            // Make sure we are in the correct VSM state after ApplyTemplate and moving the template content from the Control to the Popup:
            OnIsLightDismissEnabledChanged();
        }

        private void IsOpenChangedToClose()
        {
            if(m_popup != null)
            {
                if(m_popup.IsOpen)
                {
                    RaiseClosingEvent(true);
                }
            }

            m_currentEffectiveTipPlacementMode = TeachingTipPlacementMode.Auto;
        }

        /* Popup Methods */

        private void CreateNewPopup()
        {
            if (m_popup != null)
            {
                m_popup.Opened -= OnPopupOpened;
                m_popup.Closed -= OnPopupClosed;
            }

            Popup popup = new Popup();
            popup.AllowsTransparency = true;

            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;

            m_popup = popup;

            SetPopupAutomationProperties();
        }

        private void PositionPopup()
        {
            bool tipDoesNotFit = false;

            if (m_target != null)
            {
                tipDoesNotFit = PositionTargetedPopup();
            }
            else
            {
                tipDoesNotFit = PositionUntargetedPopup();
            }

            if (tipDoesNotFit)
            {
                IsOpen = false;
            }
        }

        private void RepositionPopup()
        {
            if (IsOpen)
            {
                Rect newTargetBounds;

                if (m_target != null)
                {
                    newTargetBounds = GetTargetBounds();
                }
                else
                {
                    newTargetBounds = default;
                }

                m_currentTargetBoundsInCoreWindowSpace = newTargetBounds;

                PositionPopup();
            }
        }

        private void ClosePopup()
        {
            if (m_popup != null)
            {
                m_popup.IsOpen = false;
            }
        }

        /* Update Visual States */

        private bool UpdateTail()
        {
            // An effective placement of auto indicates that no tail should be shown.
            var (placement, tipDoesNotFit) = DetermineEffectivePlacement();
            m_currentEffectiveTailPlacementMode = placement;
            TeachingTipTailVisibility tailVisibility = TailVisibility;

            if (tailVisibility == TeachingTipTailVisibility.Collapsed || (m_target == null && tailVisibility != TeachingTipTailVisibility.Visible))
            {
                m_currentEffectiveTailPlacementMode = TeachingTipPlacementMode.Auto;
            }

            if (placement != m_currentEffectiveTipPlacementMode)
            {
                m_currentEffectiveTipPlacementMode = placement;
            }

            UpdateSizeBasedTemplateSettings();

            switch (m_currentEffectiveTailPlacementMode)
            {
                // An effective placement of auto means the tip should not display a tail.
                case TeachingTipPlacementMode.Auto:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "Untargeted", false);
                    break;

                case TeachingTipPlacementMode.Top:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "Top", false);
                    break;

                case TeachingTipPlacementMode.Bottom:
                    UpdateDynamicHeroContentPlacementToBottom();
                    VisualStateManager.GoToState(this, "Bottom", false);
                    break;

                case TeachingTipPlacementMode.Left:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "Left", false);
                    break;

                case TeachingTipPlacementMode.Right:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "Right", false);
                    break;

                case TeachingTipPlacementMode.TopRight:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "TopRight", false);
                    break;

                case TeachingTipPlacementMode.TopLeft:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "TopLeft", false);
                    break;

                case TeachingTipPlacementMode.BottomRight:
                    UpdateDynamicHeroContentPlacementToBottom();
                    VisualStateManager.GoToState(this, "BottomRight", false);
                    break;

                case TeachingTipPlacementMode.BottomLeft:
                    UpdateDynamicHeroContentPlacementToBottom();
                    VisualStateManager.GoToState(this, "BottomLeft", false);
                    break;

                case TeachingTipPlacementMode.LeftTop:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "LeftTop", false);
                    break;

                case TeachingTipPlacementMode.LeftBottom:
                    UpdateDynamicHeroContentPlacementToBottom();
                    VisualStateManager.GoToState(this, "LeftBottom", false);
                    break;

                case TeachingTipPlacementMode.RightTop:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "RightTop", false);
                    break;

                case TeachingTipPlacementMode.RightBottom:
                    UpdateDynamicHeroContentPlacementToBottom();
                    VisualStateManager.GoToState(this, "RightBottom", false);
                    break;

                case TeachingTipPlacementMode.Center:
                    UpdateDynamicHeroContentPlacementToTop();
                    VisualStateManager.GoToState(this, "Center", false);
                    break;

                default:
                    break;
            }

            ActualPlacement = m_currentEffectiveTailPlacementMode;

            return tipDoesNotFit;
        }

        private void UpdateSizeBasedTemplateSettings()
        {
            double width;
            double height;

            if (m_contentRootGrid != null)
            {
                width = m_contentRootGrid.ActualWidth;
                height = m_contentRootGrid.ActualHeight;
            }
            else
            {
                width = 0;
                height = 0;
            }

            switch (m_currentEffectiveTailPlacementMode)
            {
                case TeachingTipPlacementMode.Top:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = TopEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.Bottom:
                    TemplateSettings.TopRightHighlightMargin = BottomPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = BottomPlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.Left:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = LeftEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.Right:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = RightEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.TopLeft:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = TopEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.TopRight:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = TopEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.BottomLeft:
                    TemplateSettings.TopRightHighlightMargin = BottomLeftPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = BottomLeftPlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.BottomRight:
                    TemplateSettings.TopRightHighlightMargin = BottomRightPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = BottomRightPlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.LeftTop:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = LeftEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.LeftBottom:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = LeftEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.RightTop:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = RightEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.RightBottom:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = RightEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.Auto:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = TopEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
                case TeachingTipPlacementMode.Center:
                    TemplateSettings.TopRightHighlightMargin = OtherPlacementTopRightHighlightMargin(width, height);
                    TemplateSettings.TopLeftHighlightMargin = TopEdgePlacementTopLeftHighlightMargin(width, height);
                    break;
            }
        }

        private void UpdateButtonsState()
        {
            if (ActionButtonContent != null && CloseButtonContent != null)
            {
                VisualStateManager.GoToState(this, "BothButtonsVisible", false);
                VisualStateManager.GoToState(this, "FooterCloseButton", false);
            }
            else if (ActionButtonContent != null && IsLightDismissEnabled)
            {
                VisualStateManager.GoToState(this, "ActionButtonVisible", false);
                VisualStateManager.GoToState(this, "FooterCloseButton", false);
            }
            else if (ActionButtonContent != null)
            {
                VisualStateManager.GoToState(this, "ActionButtonVisible", false);
                VisualStateManager.GoToState(this, "HeaderCloseButton", false);
            }
            else if (CloseButtonContent != null)
            {
                VisualStateManager.GoToState(this, "CloseButtonVisible", false);
                VisualStateManager.GoToState(this, "FooterCloseButton", false);
            }
            else if (IsLightDismissEnabled)
            {
                VisualStateManager.GoToState(this, "NoButtonsVisible", false);
                VisualStateManager.GoToState(this, "FooterCloseButton", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoButtonsVisible", false);
                VisualStateManager.GoToState(this, "HeaderCloseButton", false);
            }
        }

        private void UpdateDynamicHeroContentPlacementToTop()
        {
            if (HeroContentPlacement == TeachingTipHeroContentPlacementMode.Auto)
            {
                UpdateDynamicHeroContentPlacementToTopImpl();
            }
        }

        private void UpdateDynamicHeroContentPlacementToTopImpl()
        {
            VisualStateManager.GoToState(this, "HeroContentTop", false);

            if (m_currentHeroContentEffectivePlacementMode != TeachingTipHeroContentPlacementMode.Top)
            {
                m_currentHeroContentEffectivePlacementMode = TeachingTipHeroContentPlacementMode.Top;
            }
        }

        private void UpdateDynamicHeroContentPlacementToBottom()
        {
            if (HeroContentPlacement == TeachingTipHeroContentPlacementMode.Auto)
            {
                UpdateDynamicHeroContentPlacementToBottomImpl();
            }
        }

        private void UpdateDynamicHeroContentPlacementToBottomImpl()
        {
            VisualStateManager.GoToState(this, "HeroContentBottom", false);

            if (m_currentHeroContentEffectivePlacementMode != TeachingTipHeroContentPlacementMode.Bottom)
            {
                m_currentHeroContentEffectivePlacementMode = TeachingTipHeroContentPlacementMode.Bottom;
            }
        }

        /* Event handlers */

        private void ThemeManagerApplicationThemeChanged(object sender, ApplicationThemeChangedEventArgs e)
        {
            if (m_containerGrid != null)
            {
                ElementTheme theme = (ThemeManager.ApplicationTheme == ApplicationTheme.Dark) ? ElementTheme.Dark : ElementTheme.Light;

                m_containerGrid.SetValue(ThemeManager.ElementThemeProperty, theme);
            }
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            RepositionPopup();

            if (IsLightDismissEnabled && m_rootElement != null)
            {
                m_containerGrid.PreviewMouseDown += OnRootElementPreviewMouseDown;
                m_containerGrid.MouseLeave += OnRootElementMouseLeave;
                m_containerGrid.LostMouseCapture += OnRootElementLostMouseCapture;
            }

            if (m_parentWindow != null)
            {
                m_parentWindow.SizeChanged += OnParentWindowSizeChanged;
                m_parentWindow.LocationChanged += OnParentWindowLocationChanged;
                m_parentWindow.StateChanged += OnParentWindowStateChanged;

                if (IsLightDismissEnabled)
                {
                    m_parentWindow.Deactivated += OnParentWindowDeactivated;
                }
            }

            StartOpenAnimation();

            if (IsLightDismissEnabled)
            {
                SetMouseCapture();
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (IsLightDismissEnabled && m_rootElement != null)
            {
                m_rootElement.PreviewMouseDown -= OnRootElementPreviewMouseDown;
                m_rootElement.MouseLeave -= OnRootElementMouseLeave;
                m_rootElement.LostMouseCapture -= OnRootElementLostMouseCapture;
            }

            if (m_parentWindow != null)
            {
                m_parentWindow.SizeChanged -= OnParentWindowSizeChanged;
                m_parentWindow.LocationChanged -= OnParentWindowLocationChanged;
                m_parentWindow.StateChanged -= OnParentWindowStateChanged;
                m_parentWindow.Deactivated -= OnParentWindowDeactivated;
            }

            if (m_popup != null)
            {
                m_popup.Child = null;
            }

            TeachingTipClosedEventArgs myArgs = new TeachingTipClosedEventArgs();

            myArgs.Reason = m_lastCloseReason;

            Closed?.Invoke(this, myArgs);

            if (FrameworkElementAutomationPeer.FromElement(this) is TeachingTipAutomationPeer teachingTipPeer)
            {
                // teachingTipPeer.RaiseWindowClosedEvent(); TODO
             }
        }

        private void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                IsOpen = false;
            }
        }

        private void OnParentWindowStateChanged(object sender, EventArgs e)
        {
            if (m_parentWindow.WindowState == WindowState.Minimized && IsOpen)
            {
                IsOpen = false;
            }
        }

        private void OnParentWindowLocationChanged(object sender, EventArgs e)
        {
            RepositionPopup();
        }

        private void OnParentWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RepositionPopup();
        }

        private void OnRootElementLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!ClickedOnContentArea(e))
            {
                IsOpen = false;
            }
        }

        private void OnRootElementMouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured != m_containerGrid)
            {
                SetMouseCapture();
            }
        }

        private void OnRootElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ClickedOnContentArea(e))
            {
                IsOpen = false;
            }
        }

        private void ClosePopupOnUnloadEvent(object sender, RoutedEventArgs e)
        {
            IsOpen = false;

            ClosePopup();
        }

        private void OnTargetLayoutUpdated(object sender, EventArgs e)
        {
            RepositionPopup();
        }

        private void OnTargetLoaded(object sender, RoutedEventArgs e)
        {
            RepositionPopup();
        }

        private void OnActionButtonClicked(object sender, RoutedEventArgs e)
        {
            ActionButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            CloseButtonClick?.Invoke(this, EventArgs.Empty);

            m_lastCloseReason = TeachingTipCloseReason.CloseButton;

            IsOpen = false;
        }

        private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSizeBasedTemplateSettings();

            // Reset the currentEffectivePlacementMode so that the tail will be updated for the new size as well.
            m_currentEffectiveTipPlacementMode = TeachingTipPlacementMode.Auto;

            if (IsOpen)
            {
                PositionPopup();
            }
        }

        /* Calculate Position Methods */

        private bool PositionTargetedPopup()
        {
            bool tipDoesNotFit = UpdateTail();
            Thickness offset = PlacementMargin;

            double tipHeight;
            double tipWidth;

            if (m_tailOcclusionGrid != null)
            {
                tipHeight = m_tailOcclusionGrid.ActualHeight;
                tipWidth = m_tailOcclusionGrid.ActualWidth;
            }
            else
            {
                tipHeight = 0;
                tipWidth = 0;
            }

            double windowTop;
            double windowLeft;

            if(m_parentWindow.WindowState == WindowState.Normal)
            {
                windowTop = m_parentWindow.Top;
                windowLeft = m_parentWindow.Left;
            }
            else
            {
                windowTop = 0 - m_parentWindowBorder.Top;
                windowLeft = 0 - m_parentWindowBorder.Left;
            }

            if (m_popup != null)
            {
                // Depending on the effective placement mode of the tip we use a combination of the tip's size, the target's position within the app, the target's
                // size, and the target offset property to determine the appropriate vertical and horizontal offsets of the popup that the tip is contained in.
                switch (m_currentEffectiveTipPlacementMode)
                {
                    case TeachingTipPlacementMode.Top:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y - tipHeight - offset.Top);
                        m_popup.HorizontalOffset = windowLeft +  ((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width - tipWidth) / 2.0f));
                        break;

                    case TeachingTipPlacementMode.Bottom:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + offset.Bottom);
                        m_popup.HorizontalOffset = windowLeft +  ((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width - tipWidth) / 2.0f));
                        break;

                    case TeachingTipPlacementMode.Left:
                        m_popup.VerticalOffset = windowTop +  (((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height - tipHeight) / 2.0f);
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X - tipWidth - offset.Left);
                        break;

                    case TeachingTipPlacementMode.Right:
                        m_popup.VerticalOffset = windowTop +  (((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height - tipHeight) / 2.0f);
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + offset.Right);
                        break;

                    case TeachingTipPlacementMode.TopRight:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y - tipHeight - offset.Top);
                        m_popup.HorizontalOffset = windowLeft +  (((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width) / 2.0f) - MinimumTipEdgeToTailCenter()));
                        break;

                    case TeachingTipPlacementMode.TopLeft:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y - tipHeight - offset.Top);
                        m_popup.HorizontalOffset = windowLeft +  (((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width) / 2.0f) - tipWidth + MinimumTipEdgeToTailCenter()));
                        break;

                    case TeachingTipPlacementMode.BottomRight:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + offset.Bottom);
                        m_popup.HorizontalOffset = windowLeft +  (((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width) / 2.0f) - MinimumTipEdgeToTailCenter()));
                        break;

                    case TeachingTipPlacementMode.BottomLeft:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + offset.Bottom);
                        m_popup.HorizontalOffset = windowLeft +  (((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width) / 2.0f) - tipWidth + MinimumTipEdgeToTailCenter()));
                        break;

                    case TeachingTipPlacementMode.LeftTop:
                        m_popup.VerticalOffset = windowTop +  ((((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height) / 2.0f) - tipHeight + MinimumTipEdgeToTailCenter());
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X - tipWidth - offset.Left);
                        break;

                    case TeachingTipPlacementMode.LeftBottom:
                        m_popup.VerticalOffset = windowTop +  ((((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height) / 2.0f) - MinimumTipEdgeToTailCenter());
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X - tipWidth - offset.Left);
                        break;

                    case TeachingTipPlacementMode.RightTop:
                        m_popup.VerticalOffset = windowTop +  ((((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height) / 2.0f) - tipHeight + MinimumTipEdgeToTailCenter());
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + offset.Right);
                        break;

                    case TeachingTipPlacementMode.RightBottom:
                        m_popup.VerticalOffset = windowTop +  ((((m_currentTargetBoundsInCoreWindowSpace.Y * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Height) / 2.0f) - MinimumTipEdgeToTailCenter());
                        m_popup.HorizontalOffset = windowLeft +  (m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + offset.Right);
                        break;

                    case TeachingTipPlacementMode.Center:
                        m_popup.VerticalOffset = windowTop +  (m_currentTargetBoundsInCoreWindowSpace.Y + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f) - tipHeight - offset.Top);
                        m_popup.HorizontalOffset = windowLeft +  ((((m_currentTargetBoundsInCoreWindowSpace.X * 2.0f) + m_currentTargetBoundsInCoreWindowSpace.Width - tipWidth) / 2.0f));
                        break;

                    default:
                        break;
                }
            }

            return tipDoesNotFit;
        }

        private bool PositionUntargetedPopup()
        {
            Rect windowBoundsInCoreWindowSpace = GetEffectiveWindowBoundsInCoreWindowSpace(GetWindowBounds());

            double finalTipHeight;
            double finalTipWidth;

            if (m_tailOcclusionGrid != null)
            {
                finalTipHeight = m_tailOcclusionGrid.ActualHeight;
                finalTipWidth = m_tailOcclusionGrid.ActualWidth;
            }
            else
            {
                finalTipHeight = 0;
                finalTipWidth = 0;
            }

            double windowTop;
            double windowLeft;

            if (m_parentWindow.WindowState == WindowState.Normal)
            {
                windowTop = m_parentWindow.Top;
                windowLeft = m_parentWindow.Left;
            }
            else
            {
                windowTop = 0 - m_parentWindowBorder.Top;
                windowLeft = 0 - m_parentWindowBorder.Left;
            }

            bool tipDoesNotFit = UpdateTail();

            Thickness offset = PlacementMargin;

            if (m_popup != null)
            {
                switch (GetFlowDirectionAdjustedPlacement(PreferredPlacement))
                {
                    case TeachingTipPlacementMode.Auto:
                    case TeachingTipPlacementMode.Bottom:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.X, windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Left, offset.Right));
                        break;

                    case TeachingTipPlacementMode.Top:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.Y, offset.Top));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.X, windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Left, offset.Right));
                        break;

                    case TeachingTipPlacementMode.Left:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.Y, windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Top, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.X, offset.Left));
                        break;

                    case TeachingTipPlacementMode.Right:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.Y, windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Top, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Right));
                        break;

                    case TeachingTipPlacementMode.TopRight:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.Y, offset.Top));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Right));
                        break;

                    case TeachingTipPlacementMode.TopLeft:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.Y, offset.Top));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.X, offset.Left));
                        break;

                    case TeachingTipPlacementMode.BottomRight:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Right));
                        break;

                    case TeachingTipPlacementMode.BottomLeft:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.X, offset.Left));
                        break;

                    case TeachingTipPlacementMode.LeftTop:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.Y, offset.Top));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.X, offset.Left));
                        break;

                    case TeachingTipPlacementMode.LeftBottom:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.X, offset.Left));
                        break;

                    case TeachingTipPlacementMode.RightTop:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipNearPlacementOffset(windowBoundsInCoreWindowSpace.Y, offset.Top));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Right));
                        break;

                    case TeachingTipPlacementMode.RightBottom:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipFarPlacementOffset(windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Right));
                        break;

                    case TeachingTipPlacementMode.Center:
                        m_popup.VerticalOffset = windowTop + (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.Y, windowBoundsInCoreWindowSpace.Height, finalTipHeight, offset.Top, offset.Bottom));
                        m_popup.HorizontalOffset = windowLeft +  (UntargetedTipCenterPlacementOffset(windowBoundsInCoreWindowSpace.X, windowBoundsInCoreWindowSpace.Width, finalTipWidth, offset.Left, offset.Right));
                        break;

                    default:
                        break;
                }
            }

            return tipDoesNotFit;
        }

        private double UntargetedTipFarPlacementOffset(double farWindowCoordinateInCoreWindowSpace, double tipSize, double offset)
        {
            return farWindowCoordinateInCoreWindowSpace - (tipSize + s_untargetedTipWindowEdgeMargin + offset);
        }

        private double UntargetedTipCenterPlacementOffset(double nearWindowCoordinateInCoreWindowSpace, double farWindowCoordinateInCoreWindowSpace, double tipSize, double nearOffset, double farOffset)
        {
            return ((nearWindowCoordinateInCoreWindowSpace + farWindowCoordinateInCoreWindowSpace) / 2) - (tipSize / 2) + nearOffset - farOffset;
        }

        private double UntargetedTipNearPlacementOffset(double nearWindowCoordinateInCoreWindowSpace, double offset)
        {
            return s_untargetedTipWindowEdgeMargin + nearWindowCoordinateInCoreWindowSpace + offset;
        }

        /* Calculate Placement Methods */

        private Tuple<TeachingTipPlacementMode, bool> DetermineEffectivePlacement()
        {
            // Because we do not have access to APIs to give us details about multi monitor scenarios we do not have the ability to correctly
            // Place the tip in scenarios where we have an out of root bounds tip. Since this is the case we have decided to do no special
            // calculations and return the provided value or top if auto was set. This behavior can be removed via the
            // SetReturnTopForOutOfWindowBounds test hook.
            if (!ShouldConstrainToRootBounds && m_returnTopForOutOfWindowPlacement)
            {
                TeachingTipPlacementMode placement = GetFlowDirectionAdjustedPlacement(PreferredPlacement);

                if (placement == TeachingTipPlacementMode.Auto)
                {
                    return Tuple.Create(TeachingTipPlacementMode.Top, false);
                }

                return Tuple.Create(placement, false);
            }

            if (IsOpen && m_currentEffectiveTipPlacementMode != TeachingTipPlacementMode.Auto)
            {
                return Tuple.Create(m_currentEffectiveTipPlacementMode, false);
            }

            double contentWidth;
            double contentHeight;

            if (m_tailOcclusionGrid != null)
            {
                contentWidth = m_tailOcclusionGrid.ActualWidth;
                contentHeight = m_tailOcclusionGrid.ActualHeight;
            }
            else
            {
                contentWidth = 0;
                contentHeight = 0;
            }

            if (m_target != null)
            {
                return DetermineEffectivePlacementTargeted(contentHeight, contentWidth);
            }
            else
            {
                return DetermineEffectivePlacementUntargeted(contentHeight, contentWidth);
            }
        }

        private TeachingTipPlacementMode GetFlowDirectionAdjustedPlacement(TeachingTipPlacementMode placementMode)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                return placementMode;
            }
            else
            {
                switch (placementMode)
                {
                    case TeachingTipPlacementMode.Auto:
                        return TeachingTipPlacementMode.Auto;
                    case TeachingTipPlacementMode.Left:
                        return TeachingTipPlacementMode.Right;
                    case TeachingTipPlacementMode.Right:
                        return TeachingTipPlacementMode.Left;
                    case TeachingTipPlacementMode.Top:
                        return TeachingTipPlacementMode.Top;
                    case TeachingTipPlacementMode.Bottom:
                        return TeachingTipPlacementMode.Bottom;
                    case TeachingTipPlacementMode.LeftBottom:
                        return TeachingTipPlacementMode.RightBottom;
                    case TeachingTipPlacementMode.LeftTop:
                        return TeachingTipPlacementMode.RightTop;
                    case TeachingTipPlacementMode.TopLeft:
                        return TeachingTipPlacementMode.TopRight;
                    case TeachingTipPlacementMode.TopRight:
                        return TeachingTipPlacementMode.TopLeft;
                    case TeachingTipPlacementMode.RightTop:
                        return TeachingTipPlacementMode.LeftTop;
                    case TeachingTipPlacementMode.RightBottom:
                        return TeachingTipPlacementMode.LeftBottom;
                    case TeachingTipPlacementMode.BottomRight:
                        return TeachingTipPlacementMode.BottomLeft;
                    case TeachingTipPlacementMode.BottomLeft:
                        return TeachingTipPlacementMode.BottomRight;
                    case TeachingTipPlacementMode.Center:
                        return TeachingTipPlacementMode.Center;
                }
            }
            return TeachingTipPlacementMode.Auto;
        }

        private Tuple<TeachingTipPlacementMode, bool> DetermineEffectivePlacementTargeted(double contentHeight, double contentWidth)
        {
            // These variables will track which positions the tip will fit in. They all start true and are
            // flipped to false when we find a display condition that is not met.
            Dictionary<TeachingTipPlacementMode, bool> availability = new Dictionary<TeachingTipPlacementMode, bool>();

            availability[TeachingTipPlacementMode.Auto] = false;
            availability[TeachingTipPlacementMode.Top] = true;
            availability[TeachingTipPlacementMode.Bottom] = true;
            availability[TeachingTipPlacementMode.Right] = true;
            availability[TeachingTipPlacementMode.Left] = true;
            availability[TeachingTipPlacementMode.TopLeft] = true;
            availability[TeachingTipPlacementMode.TopRight] = true;
            availability[TeachingTipPlacementMode.BottomLeft] = true;
            availability[TeachingTipPlacementMode.BottomRight] = true;
            availability[TeachingTipPlacementMode.LeftTop] = true;
            availability[TeachingTipPlacementMode.LeftBottom] = true;
            availability[TeachingTipPlacementMode.RightTop] = true;
            availability[TeachingTipPlacementMode.RightBottom] = true;
            availability[TeachingTipPlacementMode.Center] = true;

            double tipHeight = contentHeight + TailShortSideLength();
            double tipWidth = contentWidth + TailShortSideLength();

            // We try to avoid having the tail touch the HeroContent so rule out positions where this would be required
            if (HeroContent != null)
            {
                if (m_heroContentBorder != null && m_nonHeroContentRootGrid != null)
                {
                    if (m_heroContentBorder.ActualHeight > m_nonHeroContentRootGrid.ActualHeight - TailLongSideActualLength())
                    {
                        availability[TeachingTipPlacementMode.Left] = false;
                        availability[TeachingTipPlacementMode.Right] = false;
                    }
                }

                switch (HeroContentPlacement)
                {
                    case TeachingTipHeroContentPlacementMode.Bottom:
                        {
                            availability[TeachingTipPlacementMode.Top] = false;
                            availability[TeachingTipPlacementMode.TopRight] = false;
                            availability[TeachingTipPlacementMode.TopLeft] = false;
                            availability[TeachingTipPlacementMode.RightTop] = false;
                            availability[TeachingTipPlacementMode.LeftTop] = false;
                            availability[TeachingTipPlacementMode.Center] = false;
                            break;
                        }
                    case TeachingTipHeroContentPlacementMode.Top:
                        {
                            availability[TeachingTipPlacementMode.Bottom] = false;
                            availability[TeachingTipPlacementMode.BottomLeft] = false;
                            availability[TeachingTipPlacementMode.BottomRight] = false;
                            availability[TeachingTipPlacementMode.RightBottom] = false;
                            availability[TeachingTipPlacementMode.LeftBottom] = false;
                            break;
                        }
                }
            }

            // When ShouldConstrainToRootBounds is true clippedTargetBounds == availableBoundsAroundTarget
            // We have to separate them because there are checks which care about both.
            var (clippedTargetBounds, availableBoundsAroundTarget) = DetermineSpaceAroundTarget();

            // If the edge of the target isn't in the window.
            if (clippedTargetBounds.Left < 0)
            {
                availability[TeachingTipPlacementMode.LeftBottom] = false;
                availability[TeachingTipPlacementMode.Left] = false;
                availability[TeachingTipPlacementMode.LeftTop] = false;
            }
            // If the right edge of the target isn't in the window.
            if (clippedTargetBounds.Right < 0)
            {
                availability[TeachingTipPlacementMode.RightBottom] = false;
                availability[TeachingTipPlacementMode.Right] = false;
                availability[TeachingTipPlacementMode.RightTop] = false;
            }
            // If the top edge of the target isn't in the window.
            if (clippedTargetBounds.Top < 0)
            {
                availability[TeachingTipPlacementMode.TopLeft] = false;
                availability[TeachingTipPlacementMode.Top] = false;
                availability[TeachingTipPlacementMode.TopRight] = false;
            }
            // If the bottom edge of the target isn't in the window
            if (clippedTargetBounds.Bottom < 0)
            {
                availability[TeachingTipPlacementMode.BottomLeft] = false;
                availability[TeachingTipPlacementMode.Bottom] = false;
                availability[TeachingTipPlacementMode.BottomRight] = false;
            }

            // If the horizontal midpoint is out of the window.
            if (clippedTargetBounds.Left < -m_currentTargetBoundsInCoreWindowSpace.Width / 2 ||
                clippedTargetBounds.Right < -m_currentTargetBoundsInCoreWindowSpace.Width / 2)
            {
                availability[TeachingTipPlacementMode.TopLeft] = false;
                availability[TeachingTipPlacementMode.Top] = false;
                availability[TeachingTipPlacementMode.TopRight] = false;
                availability[TeachingTipPlacementMode.BottomLeft] = false;
                availability[TeachingTipPlacementMode.Bottom] = false;
                availability[TeachingTipPlacementMode.BottomRight] = false;
                availability[TeachingTipPlacementMode.Center] = false;
            }

            // If the vertical midpoint is out of the window.
            if (clippedTargetBounds.Top < -m_currentTargetBoundsInCoreWindowSpace.Height / 2 ||
                clippedTargetBounds.Bottom < -m_currentTargetBoundsInCoreWindowSpace.Height / 2)
            {
                availability[TeachingTipPlacementMode.LeftBottom] = false;
                availability[TeachingTipPlacementMode.Left] = false;
                availability[TeachingTipPlacementMode.LeftTop] = false;
                availability[TeachingTipPlacementMode.RightBottom] = false;
                availability[TeachingTipPlacementMode.Right] = false;
                availability[TeachingTipPlacementMode.RightTop] = false;
                availability[TeachingTipPlacementMode.Center] = false;
            }

            // If the tip is too tall to fit between the top of the target and the top edge of the window or screen.
            if (tipHeight > availableBoundsAroundTarget.Top)
            {
                availability[TeachingTipPlacementMode.Top] = false;
                availability[TeachingTipPlacementMode.TopRight] = false;
                availability[TeachingTipPlacementMode.TopLeft] = false;
            }
            // If the total tip is too tall to fit between the center of the target and the top of the window.
            if (tipHeight > availableBoundsAroundTarget.Top + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f))
            {
                availability[TeachingTipPlacementMode.Center] = false;
            }
            // If the tip is too tall to fit between the center of the target and the top edge of the window.
            if (contentHeight - MinimumTipEdgeToTailCenter() > availableBoundsAroundTarget.Top + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f))
            {
                availability[TeachingTipPlacementMode.RightTop] = false;
                availability[TeachingTipPlacementMode.LeftTop] = false;
            }
            // If the tip is too tall to fit in the window when the tail is centered vertically on the target and the tip.
            if (contentHeight / 2.0f > availableBoundsAroundTarget.Top + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f) ||
                contentHeight / 2.0f > availableBoundsAroundTarget.Bottom + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f))
            {
                availability[TeachingTipPlacementMode.Right] = false;
                availability[TeachingTipPlacementMode.Left] = false;
            }
            // If the tip is too tall to fit between the center of the target and the bottom edge of the window.
            if (contentHeight - MinimumTipEdgeToTailCenter() > availableBoundsAroundTarget.Bottom + (m_currentTargetBoundsInCoreWindowSpace.Height / 2.0f))
            {
                availability[TeachingTipPlacementMode.RightBottom] = false;
                availability[TeachingTipPlacementMode.LeftBottom] = false;
            }
            // If the tip is too tall to fit between the bottom of the target and the bottom edge of the window.
            if (tipHeight > availableBoundsAroundTarget.Bottom)
            {
                availability[TeachingTipPlacementMode.Bottom] = false;
                availability[TeachingTipPlacementMode.BottomLeft] = false;
                availability[TeachingTipPlacementMode.BottomRight] = false;
            }

            // If the tip is too wide to fit between the left edge of the target and the left edge of the window.
            if (tipWidth > availableBoundsAroundTarget.Left)
            {
                availability[TeachingTipPlacementMode.Left] = false;
                availability[TeachingTipPlacementMode.LeftTop] = false;
                availability[TeachingTipPlacementMode.LeftBottom] = false;
            }
            // If the tip is too wide to fit between the center of the target and the left edge of the window.
            if (contentWidth - MinimumTipEdgeToTailCenter() > availableBoundsAroundTarget.Left + (m_currentTargetBoundsInCoreWindowSpace.Width / 2.0f))
            {
                availability[TeachingTipPlacementMode.TopLeft] = false;
                availability[TeachingTipPlacementMode.BottomLeft] = false;
            }
            // If the tip is too wide to fit in the window when the tail is centered horizontally on the target and the tip.
            if (contentWidth / 2.0f > availableBoundsAroundTarget.Left + (m_currentTargetBoundsInCoreWindowSpace.Width / 2.0f) ||
                contentWidth / 2.0f > availableBoundsAroundTarget.Right + (m_currentTargetBoundsInCoreWindowSpace.Width / 2.0f))
            {
                availability[TeachingTipPlacementMode.Top] = false;
                availability[TeachingTipPlacementMode.Bottom] = false;
                availability[TeachingTipPlacementMode.Center] = false;
            }
            // If the tip is too wide to fit between the center of the target and the right edge of the window.
            if (contentWidth - MinimumTipEdgeToTailCenter() > availableBoundsAroundTarget.Right + (m_currentTargetBoundsInCoreWindowSpace.Width / 2.0f))
            {
                availability[TeachingTipPlacementMode.TopRight] = false;
                availability[TeachingTipPlacementMode.BottomRight] = false;
            }
            // If the tip is too wide to fit between the right edge of the target and the right edge of the window.
            if (tipWidth > availableBoundsAroundTarget.Right)
            {
                availability[TeachingTipPlacementMode.Right] = false;
                availability[TeachingTipPlacementMode.RightTop] = false;
                availability[TeachingTipPlacementMode.RightBottom] = false;
            }

            TeachingTipPlacementMode wantedDirection = GetFlowDirectionAdjustedPlacement(PreferredPlacement);
            TeachingTipPlacementMode[] priorities = GetPlacementFallbackOrder(wantedDirection);

            foreach (TeachingTipPlacementMode mode in priorities)
            {
                if (availability[mode])
                {
                    return Tuple.Create(mode, false);
                }
            }

            // The teaching tip wont fit anywhere, set tipDoesNotFit to indicate that we should not open.
            return Tuple.Create(TeachingTipPlacementMode.Top, true);
        }

        private Tuple<TeachingTipPlacementMode, bool> DetermineEffectivePlacementUntargeted(double contentHeight, double contentWidth)
        {
            Rect windowBounds = GetWindowBounds();
            if (!ShouldConstrainToRootBounds)
            {
                Rect screenBoundsInCoreWindowSpace = GetEffectiveScreenBoundsInCoreWindowSpace(windowBounds);
                if (screenBoundsInCoreWindowSpace.Height > contentHeight && screenBoundsInCoreWindowSpace.Width > contentWidth)
                {
                    return Tuple.Create(TeachingTipPlacementMode.Bottom, false);
                }
            }
            else
            {
                Rect windowBoundsInCoreWindowSpace = GetEffectiveWindowBoundsInCoreWindowSpace(windowBounds);
                if (windowBoundsInCoreWindowSpace.Height > contentHeight && windowBoundsInCoreWindowSpace.Width > contentWidth)
                {
                    return Tuple.Create(TeachingTipPlacementMode.Bottom, false);
                }
            }

            // The teaching tip doesn't fit in the window/screen set tipDoesNotFit to indicate that we should not open.
            return Tuple.Create(TeachingTipPlacementMode.Top, true);
        }

        private Tuple<Thickness, Thickness> DetermineSpaceAroundTarget()
        {
            Rect windowBounds = GetWindowBounds();

            Rect windowBoundsInCoreWindowSpace = GetEffectiveWindowBoundsInCoreWindowSpace(windowBounds);
            Rect screenBoundsInCoreWindowSpace = GetEffectiveScreenBoundsInCoreWindowSpace(windowBounds);

            Thickness windowSpaceAroundTarget = new Thickness(
                // Target.Left - Window.Left
                m_currentTargetBoundsInCoreWindowSpace.X - /* 0 except with test window bounds */ windowBoundsInCoreWindowSpace.X,
                // Target.Top - Window.Top
                m_currentTargetBoundsInCoreWindowSpace.Y - /* 0 except with test window bounds */ windowBoundsInCoreWindowSpace.Y,
                // Window.Right - Target.Right
                (windowBoundsInCoreWindowSpace.X + windowBoundsInCoreWindowSpace.Width) - (m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width),
                // Screen.Right - Target.Right
                (windowBoundsInCoreWindowSpace.Y + windowBoundsInCoreWindowSpace.Height) - (m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height));

            Thickness screenSpaceAroundTarget;

            if (!ShouldConstrainToRootBounds)
            {
                screenSpaceAroundTarget = new Thickness(
                    // Target.Left - Screen.Left
                    m_currentTargetBoundsInCoreWindowSpace.X - screenBoundsInCoreWindowSpace.X,
                    // Target.Top - Screen.Top
                    m_currentTargetBoundsInCoreWindowSpace.Y - screenBoundsInCoreWindowSpace.Y,
                    // Screen.Right - Target.Right
                    (screenBoundsInCoreWindowSpace.X + screenBoundsInCoreWindowSpace.Width) - (m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width),
                    // Screen.Bottom - Target.Bottom
                    (screenBoundsInCoreWindowSpace.Y + screenBoundsInCoreWindowSpace.Height) - (m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height));
            }
            else
            {
                screenSpaceAroundTarget = windowSpaceAroundTarget;
            }

            return Tuple.Create(windowSpaceAroundTarget, screenSpaceAroundTarget);
        }

        private TeachingTipPlacementMode[] GetPlacementFallbackOrder(TeachingTipPlacementMode preferredPlacement)
        {
            TeachingTipPlacementMode[] priorityList = new TeachingTipPlacementMode[13];
            priorityList[0] = TeachingTipPlacementMode.Top;
            priorityList[1] = TeachingTipPlacementMode.Bottom;
            priorityList[2] = TeachingTipPlacementMode.Left;
            priorityList[3] = TeachingTipPlacementMode.Right;
            priorityList[4] = TeachingTipPlacementMode.TopLeft;
            priorityList[5] = TeachingTipPlacementMode.TopRight;
            priorityList[6] = TeachingTipPlacementMode.BottomLeft;
            priorityList[7] = TeachingTipPlacementMode.BottomRight;
            priorityList[8] = TeachingTipPlacementMode.LeftTop;
            priorityList[9] = TeachingTipPlacementMode.LeftBottom;
            priorityList[10] = TeachingTipPlacementMode.RightTop;
            priorityList[11] = TeachingTipPlacementMode.RightBottom;
            priorityList[12] = TeachingTipPlacementMode.Center;

            if (IsPlacementBottom(preferredPlacement))
            {
                // Swap to bottom > top
                Swap(ref priorityList[0], ref priorityList[1]);
                Swap(ref priorityList[4], ref priorityList[6]);
                Swap(ref priorityList[5], ref priorityList[7]);
            }
            else if (IsPlacementLeft(preferredPlacement))
            {
                // swap to lateral > vertical
                Swap(ref priorityList[0], ref priorityList[2]);
                Swap(ref priorityList[1], ref priorityList[3]);
                Swap(ref priorityList[4], ref priorityList[8]);
                Swap(ref priorityList[5], ref priorityList[9]);
                Swap(ref priorityList[6], ref priorityList[10]);
                Swap(ref priorityList[7], ref priorityList[11]);
            }
            else if (IsPlacementRight(preferredPlacement))
            {
                // swap to lateral > vertical
                Swap(ref priorityList[0], ref priorityList[2]);
                Swap(ref priorityList[1], ref priorityList[3]);
                Swap(ref priorityList[4], ref priorityList[8]);
                Swap(ref priorityList[5], ref priorityList[9]);
                Swap(ref priorityList[6], ref priorityList[10]);
                Swap(ref priorityList[7], ref priorityList[11]);

                // swap to right > left
                Swap(ref priorityList[0], ref priorityList[1]);
                Swap(ref priorityList[4], ref priorityList[6]);
                Swap(ref priorityList[5], ref priorityList[7]);
            }

            int pivot = Array.IndexOf(priorityList, preferredPlacement);

            if (priorityList.Length != pivot)
            {
                Rotate(priorityList, -pivot);
            }

            return priorityList;
        }

        private Rect GetEffectiveWindowBoundsInCoreWindowSpace(Rect windowBounds)
        {
            return new Rect(0, 0, windowBounds.Width, windowBounds.Height);
        }

        private Rect GetEffectiveScreenBoundsInCoreWindowSpace(Rect windowBounds)
        {
            if (!ShouldConstrainToRootBounds)
            {
                return new Rect(-windowBounds.X, -windowBounds.Y, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            }

            return default;
        }

        /* *** */

        private CornerRadius GetTeachingTipCornerRadius()
        {
            return CornerRadius;
        }

        private double TailShortSideLength()
        {
            if (m_tailPolygon != null)
            {
                return Math.Min(m_tailPolygon.ActualHeight, m_tailPolygon.ActualWidth) - s_tailOcclusionAmount;
            }

            return 0;
        }

        private double TailLongSideActualLength()
        {
            if (m_tailPolygon != null)
            {
                return Math.Max(m_tailPolygon.ActualHeight, m_tailPolygon.ActualWidth);
            }

            return 0;
        }

        private double MinimumTipEdgeToTailCenter()
        {
            if (m_tailOcclusionGrid != null && m_tailPolygon != null)
            {
                return m_tailOcclusionGrid.ColumnDefinitions.Count > 1 ?
                    (m_tailOcclusionGrid.ColumnDefinitions[0].ActualWidth + m_tailOcclusionGrid.ColumnDefinitions[1].ActualWidth + (Math.Max(m_tailPolygon.ActualHeight, m_tailPolygon.ActualWidth) / 2))
                    : 0;
            }

            return 0;
        }

        private double TopLeftCornerRadius()
        {
            return GetTeachingTipCornerRadius().TopLeft;
        }

        private double TopRightCornerRadius()
        {
            return GetTeachingTipCornerRadius().TopRight;
        }

        private double MinimumTipEdgeToTailEdgeMargin()
        {
            if (m_tailOcclusionGrid != null)
            {
                return m_tailOcclusionGrid.ColumnDefinitions.Count > 1 ?
                    m_tailOcclusionGrid.ColumnDefinitions[1].ActualWidth + s_tailOcclusionAmount
                    : 0.0;
            }

            return 0;
        }

        private double TailLongSideLength()
        {
            return TailLongSideActualLength() - (2 * s_tailOcclusionAmount);
        }

        // These values are shifted by one because this is the 1px highlight that sits adjacent to the tip border.
        private Thickness BottomPlacementTopRightHighlightMargin(double width, double height)
        {
            return new Thickness((width / 2) + (TailShortSideLength() - 1.0f), 0, (TopRightCornerRadius() - 1.0f), 0);
        }

        private Thickness BottomRightPlacementTopRightHighlightMargin(double width, double height)
        {
            return new Thickness(MinimumTipEdgeToTailEdgeMargin() + TailLongSideLength() - 1.0f, 0, (TopRightCornerRadius() - 1.0f), 0);
        }

        private Thickness BottomLeftPlacementTopRightHighlightMargin(double width, double height)
        {
            return new Thickness(width - (MinimumTipEdgeToTailEdgeMargin() + 1.0f), 0, (TopRightCornerRadius() - 1.0f), 0);
        }

        private Thickness OtherPlacementTopRightHighlightMargin(double width, double height)
        {
            return new Thickness(0);
        }

        private Thickness BottomPlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 1.0f), 0, (width / 2) + (TailShortSideLength() - 1.0f), 0);
        }

        private Thickness BottomRightPlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 1.0f), 0, width - (MinimumTipEdgeToTailEdgeMargin() + 1.0f), 0);
        }

        private Thickness BottomLeftPlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 1.0f), 0, MinimumTipEdgeToTailEdgeMargin() + TailLongSideLength() - 1.0f, 0);
        }

        private Thickness TopEdgePlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 1.0f), 1, (TopRightCornerRadius() - 1.0f), 0);
        }

        // Shifted by one since the tail edge's border is not accounted for automatically.
        private Thickness LeftEdgePlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 1.0f), 1, (TopRightCornerRadius() - 2.0f), 0);
        }

        private Thickness RightEdgePlacementTopLeftHighlightMargin(double width, double height)
        {
            return new Thickness((TopLeftCornerRadius() - 2.0f), 1, (TopRightCornerRadius() - 1.0f), 0);
        }

        /* *** */

        private bool IsPlacementTop(TeachingTipPlacementMode placement)
        {
            return placement == TeachingTipPlacementMode.Top || placement == TeachingTipPlacementMode.TopLeft || placement == TeachingTipPlacementMode.TopRight;
        }

        private bool IsPlacementBottom(TeachingTipPlacementMode placement)
        {
            return placement == TeachingTipPlacementMode.Bottom || placement == TeachingTipPlacementMode.BottomLeft || placement == TeachingTipPlacementMode.BottomRight;
        }

        private bool IsPlacementLeft(TeachingTipPlacementMode placement)
        {
            return placement == TeachingTipPlacementMode.Left || placement == TeachingTipPlacementMode.LeftTop || placement == TeachingTipPlacementMode.LeftBottom;
        }

        private bool IsPlacementRight(TeachingTipPlacementMode placement)
        {
            return placement == TeachingTipPlacementMode.Right || placement == TeachingTipPlacementMode.RightTop || placement == TeachingTipPlacementMode.RightBottom;
        }

        /* *** */

        private Rect GetTargetBounds()
        {
            double captionHeight = GetWindowCaptionHeight();

            return m_target.TransformToVisual(m_parentWindow).TransformBounds(new Rect(0, captionHeight, m_target.ActualWidth, m_target.ActualHeight));
        }

        private Rect GetWindowBounds()
        {
            Rect bounds;

            if (m_parentWindow == null)
            {
                m_parentWindow = GetParentWindow();
            }

            if (m_parentWindow.WindowState == WindowState.Maximized)
            {
                bounds = new Rect(0 + m_parentWindowBorder.Left, 0 + m_parentWindowBorder.Top + SystemParameters.WindowCaptionHeight, m_parentWindow.ActualWidth, m_parentWindow.ActualHeight);
            }
            else
            {
                bounds = new Rect(m_parentWindow.Left, m_parentWindow.Top, m_parentWindow.Width, m_parentWindow.Height);
            }

            return bounds;
        }

        private double GetWindowCaptionHeight()
        {
            if (m_parentWindow is ModernWindow window)
            {
                return window.ExtendViewIntoTitleBar ? 0 : window.TitleBarHeight;
            }
            else
            {
                return SystemParameters.CaptionHeight;
            }
        }

        private Thickness GetParentWindowBorder()
        {
            int borderPadding = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);
            Thickness curr = SystemParameters.WindowResizeBorderThickness;
            return new Thickness(curr.Left + borderPadding, curr.Top + borderPadding, curr.Right + borderPadding, curr.Top + borderPadding);
        }

        private  Window GetParentWindow()
        {
            Window window = Window.GetWindow(this) ?? Application.Current.MainWindow;

            if (window == null)
            {
                throw new NullReferenceException("The Window cannot be empty.");
            }

            return window;
        }

        /* Utils Methods */

        private bool ClickedOnContentArea(MouseEventArgs args)
        {
            if (m_containerGrid != null)
            {
                Point point = args.GetPosition(m_containerGrid);

                if ((point.X >= 0 && point.X < m_containerGrid.ActualWidth) && (point.Y >= 0 && point.Y < m_containerGrid.ActualHeight))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void SetMouseCapture()
        {
            if (m_containerGrid != null)
            {
                Mouse.Capture(m_containerGrid, CaptureMode.SubTree);
            }
        }

        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        // https://stackoverflow.com/questions/38482696/how-to-efficiently-rotate-an-array
        private static void Rotate<T>(T[] array, int count)
        {
            if (array == null || array.Length < 2) return;
            count %= array.Length;
            if (count == 0) return;
            int left = count < 0 ? -count : array.Length + count;
            int right = count > 0 ? count : array.Length - count;
            if (left <= right)
            {
                for (int i = 0; i < left; i++)
                {
                    var temp = array[0];
                    Array.Copy(array, 1, array, 0, array.Length - 1);
                    array[array.Length - 1] = temp;
                }
            }
            else
            {
                for (int i = 0; i < right; i++)
                {
                    var temp = array[array.Length - 1];
                    Array.Copy(array, 0, array, 1, array.Length - 1);
                    array[0] = temp;
                }
            }
        }

        #endregion
    }
}
