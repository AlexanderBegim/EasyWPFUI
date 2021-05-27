// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EasyWPFUI.Controls;
using EasyWPFUI.Common;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Data;
using System.Windows.Threading;
using EasyWPFUI.Extensions;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Media.Animations;
using EasyWPFUI.Automation.Peers;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls
{
    public enum NavigationViewDisplayMode
    {
        Minimal = 0,
        Compact = 1,
        Expanded = 2,
    };

    public enum NavigationViewBackButtonVisible
    {
        Collapsed = 0,
        Visible = 1,
        Auto = 2,
    };

    public enum NavigationViewPaneDisplayMode
    {
        Auto = 0,
        Left = 1,
        Top = 2,
        LeftCompact = 3,
        LeftMinimal = 4
    };

    public enum NavigationViewSelectionFollowsFocus
    {
        Disabled = 0,
        Enabled = 1
    };

    public enum NavigationViewShoulderNavigationEnabled
    {
        WhenSelectionFollowsFocus = 0,
        Always = 1,
        Never = 2
    };

    public enum NavigationViewOverflowLabelMode
    {
        MoreLabel = 0,
        NoLabel = 1
    };

    public enum TopNavigationViewLayoutState
    {
        Uninitialized = 0,
        Initialized
    };

    public enum NavigationRecommendedTransitionDirection
    {
        FromOverflow, // mapping to SlideNavigationTransitionInfo FromLeft
        FromLeft, // SlideNavigationTransitionInfo
        FromRight, // SlideNavigationTransitionInfo
        Default // Currently it's mapping to EntranceNavigationTransitionInfo and is subject to change.
    };

    public class NavigationView : ContentControl
    {
        // General items
        private const string c_togglePaneButtonName = "TogglePaneButton";
        private const string c_paneTitleHolderFrameworkElement = "PaneTitleHolder";
        private const string c_paneTitleFrameworkElement = "PaneTitleTextBlock";
        private const string c_rootSplitViewName = "RootSplitView";
        private const string c_menuItemsHost = "MenuItemsHost";
        private const string c_footerMenuItemsHost = "FooterMenuItemsHost";
        private const string c_selectionIndicatorName = "SelectionIndicator";
        private const string c_paneContentGridName = "PaneContentGrid";
        private const string c_rootGridName = "RootGrid";
        private const string c_contentGridName = "ContentGrid";
        private const string c_searchButtonName = "PaneAutoSuggestButton";
        private const string c_paneToggleButtonIconGridColumnName = "PaneToggleButtonIconWidthColumn";
        private const string c_togglePaneTopPadding = "TogglePaneTopPadding";
        private const string c_contentPaneTopPadding = "ContentPaneTopPadding";
        private const string c_contentLeftPadding = "ContentLeftPadding";
        private const string c_navViewBackButton = "NavigationViewBackButton";
        private const string c_navViewBackButtonToolTip = "NavigationViewBackButtonToolTip";
        private const string c_navViewCloseButton = "NavigationViewCloseButton";
        private const string c_navViewCloseButtonToolTip = "NavigationViewCloseButtonToolTip";
        private const string c_paneShadowReceiverCanvas = "PaneShadowReceiver";
        private const string c_flyoutRootGrid = "FlyoutRootGrid";

        // DisplayMode Top specific items
        private const string c_topNavMenuItemsHost = "TopNavMenuItemsHost";
        private const string c_topNavFooterMenuItemsHost = "TopFooterMenuItemsHost";
        private const string c_topNavOverflowButton = "TopNavOverflowButton";
        private const string c_topNavMenuItemsOverflowHost = "TopNavMenuItemsOverflowHost";
        private const string c_topNavGrid = "TopNavGrid";
        private const string c_topNavContentOverlayAreaGrid = "TopNavContentOverlayAreaGrid";
        private const string c_leftNavPaneAutoSuggestBoxPresenter = "PaneAutoSuggestBoxPresenter";
        private const string c_topNavPaneAutoSuggestBoxPresenter = "TopPaneAutoSuggestBoxPresenter";
        private const string c_paneTitlePresenter = "PaneTitlePresenter";

        // DisplayMode Left specific items
        private const string c_leftNavFooterContentBorder = "FooterContentBorder";
        private const string c_leftNavPaneHeaderContentBorder = "PaneHeaderContentBorder";
        private const string c_leftNavPaneCustomContentBorder = "PaneCustomContentBorder";

        private const string c_itemsContainer = "ItemsContainerGrid";
        private const string c_itemsContainerRow = "ItemsContainerRow";
        private const string c_visualItemsSeparator = "VisualItemsSeparator";
        private const string c_menuItemsScrollViewer = "MenuItemsScrollViewer";
        private const string c_footerItemsScrollViewer = "FooterItemsScrollViewer";

        private const string c_paneHeaderOnTopPane = "PaneHeaderOnTopPane";
        private const string c_paneTitleOnTopPane = "PaneTitleOnTopPane";
        private const string c_paneCustomContentOnTopPane = "PaneCustomContentOnTopPane";
        private const string c_paneFooterOnTopPane = "PaneFooterOnTopPane";
        private const string c_paneHeaderCloseButtonColumn = "PaneHeaderCloseButtonColumn";
        private const string c_paneHeaderToggleButtonColumn = "PaneHeaderToggleButtonColumn";
        private const string c_paneHeaderContentBorderRow = "PaneHeaderContentBorderRow";

        private const int c_backButtonHeight = 40;
        private const int c_backButtonWidth = 40;
        private const int c_paneToggleButtonHeight = 40;
        private const int c_paneToggleButtonWidth = 40;
        private const int c_toggleButtonHeightWhenShouldPreserveNavigationViewRS3Behavior = 56;
        private const int c_backButtonRowDefinition = 1;
        private const double c_paneElevationTranslationZ = 32;

        private const int c_mainMenuBlockIndex = 0;
        private const int c_footerMenuBlockIndex = 1;

        // Shadows specific items
        private const string c_shadowCaster = "ShadowCaster";
        private const string c_shadowCasterEaseInStoryboard = "ShadowCasterEaseInStoryboard";
        private const string c_shadowCasterSmallPaneEaseInStoryboard = "ShadowCasterSmallPaneEaseInStoryboard";
        private const string c_shadowCasterEaseOutStoryboard = "ShadowCasterEaseOutStoryboard";

        private const int s_itemNotFound = -1;

        private static Size c_infSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        #region Visual Components

        private Button m_paneToggleButton;
        private SplitView m_rootSplitView;
        private NavigationViewItem m_settingsItem;
        private RowDefinition m_itemsContainerRow;
        private FrameworkElement m_menuItemsScrollViewer;
        private FrameworkElement m_footerItemsScrollViewer;
        private UIElement m_paneContentGrid;
        private ColumnDefinition m_paneToggleButtonIconGridColumn;
        private FrameworkElement m_paneTitleHolderFrameworkElement;
        private FrameworkElement m_paneTitleFrameworkElement;
        private FrameworkElement m_visualItemsSeparator;
        private Button m_paneSearchButton;
        private Button m_backButton;
        private Button m_closeButton;
        private ItemsRepeater m_leftNavRepeater;
        private ItemsRepeater m_topNavRepeater;
        private ItemsRepeater m_leftNavFooterMenuRepeater;
        private ItemsRepeater m_topNavFooterMenuRepeater;
        private Button m_topNavOverflowButton;
        private ItemsRepeater m_topNavRepeaterOverflowView;
        private Grid m_topNavGrid;
        private Border m_topNavContentOverlayAreaGrid;

        private FrameworkElement m_itemsContainerGrid;

        /* *** */

        private Grid m_shadowCaster;
        private Storyboard m_shadowCasterEaseInStoryboard;
        private Storyboard m_shadowCasterSmallPaneEaseInStoryboard;
        private Storyboard m_shadowCasterEaseOutStoryboard;

        // Indicator animations
        private UIElement m_prevIndicator;
        private UIElement m_nextIndicator;
        private UIElement m_activeIndicator;
        private object m_lastSelectedItemPendingAnimationInTopNav;

        private FrameworkElement m_togglePaneTopPadding;
        private FrameworkElement m_contentPaneTopPadding;
        private FrameworkElement m_contentLeftPadding;

        private CoreApplicationViewTitleBar m_coreTitleBar;

        private ContentControl m_leftNavPaneAutoSuggestBoxPresenter;
        private ContentControl m_topNavPaneAutoSuggestBoxPresenter;

        private ContentControl m_leftNavPaneHeaderContentBorder;
        private ContentControl m_leftNavPaneCustomContentBorder;
        private ContentControl m_leftNavFooterContentBorder;

        private ContentControl m_paneHeaderOnTopPane;
        private ContentControl m_paneTitleOnTopPane;
        private ContentControl m_paneCustomContentOnTopPane;
        private ContentControl m_paneFooterOnTopPane;
        private ContentControl m_paneTitlePresenter;

        private ColumnDefinition m_paneHeaderCloseButtonColumn;
        private ColumnDefinition m_paneHeaderToggleButtonColumn;
        private RowDefinition m_paneHeaderContentBorderRow;

        private NavigationViewItem m_lastItemExpandedIntoFlyout;

        #endregion

        #region Fields

        private bool m_InitialNonForcedModeUpdate = true;
        private NavigationViewItemsFactory m_navigationViewItemsFactory = null;

        private bool m_wasForceClosed = false;
        private bool m_isClosedCompact = false;
        private bool m_blockNextClosingEvent = false;
        private bool m_initialListSizeStateSet = false;

        private TopNavigationViewDataProvider m_topDataProvider = new TopNavigationViewDataProvider();

        private SelectionModel m_selectionModel = new SelectionModel();
        private IList<object> m_selectionModelSource;

        private ItemsSourceView m_menuItemsSource = null;
        private ItemsSourceView m_footerItemsSource = null;

        private bool m_appliedTemplate = false;

        // Identifies whenever a call is the result of OnApplyTemplate
        private bool m_fromOnApplyTemplate = false;

        // Used to defer updating the SplitView displaymode property
        private bool m_updateVisualStateForDisplayModeFromOnLoaded = false;

        // flag is used to stop recursive call. eg:
        // Customer select an item from SelectedItem property->ChangeSelection update ListView->LIstView raise OnSelectChange(we want stop here)->change property do do animation again.
        // Customer clicked listview->listview raised OnSelectChange->SelectedItem property changed->ChangeSelection->Undo the selection by SelectedItem(prevItem) (we want it stop here)->ChangeSelection again ->...
        private bool m_shouldIgnoreNextSelectionChange = false;
        // A flag to track that the selectionchange is caused by selection a item in topnav overflow menu
        private bool m_selectionChangeFromOverflowMenu = false;
        // Flag indicating whether selection change should raise item invoked. This is needed to be able to raise ItemInvoked before SelectionChanged while SelectedItem should point to the clicked item
        private bool m_shouldRaiseItemInvokedAfterSelection = false;

        private TopNavigationViewLayoutState m_topNavigationMode = TopNavigationViewLayoutState.Uninitialized;

        // A threshold to stop recovery from overflow to normal happens immediately on resize.
        private double m_topNavigationRecoveryGracePeriodWidth = 5;

        // There are three ways to change IsPaneOpen:
        // 1, customer call IsPaneOpen=true/false directly or nav.IsPaneOpen is binding with a variable and the value is changed.
        // 2, customer click ToggleButton or splitView.IsPaneOpen->nav.IsPaneOpen changed because of window resize
        // 3, customer changed PaneDisplayMode.
        // 2 and 3 are internal implementation and will call by ClosePane/OpenPane. the flag is to indicate 1 if it's false
        private bool m_isOpenPaneForInteraction = false;

        private bool m_moveTopNavOverflowItemOnFlyoutClose = false;

        private bool m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = false;

        private bool m_OrientationChangedPendingAnimation = false;

        private bool m_TabKeyPrecedesFocusChange = false;

        private bool m_layoutUpdatedToken;

        #endregion

        #region Events

        public event TypedEventHandler<NavigationView, NavigationViewSelectionChangedEventArgs> SelectionChanged;
        public event TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs> ItemInvoked;
        public event TypedEventHandler<NavigationView, NavigationViewDisplayModeChangedEventArgs> DisplayModeChanged;
        public event TypedEventHandler<NavigationView, NavigationViewBackRequestedEventArgs> BackRequested;
        public event TypedEventHandler<NavigationView, object> PaneClosed;
        public event TypedEventHandler<NavigationView, NavigationViewPaneClosingEventArgs> PaneClosing;
        public event TypedEventHandler<NavigationView, object> PaneOpened;
        public event TypedEventHandler<NavigationView, object> PaneOpening;
        public event TypedEventHandler<NavigationView, NavigationViewItemExpandingEventArgs> Expanding;
        public event TypedEventHandler<NavigationView, NavigationViewItemCollapsedEventArgs> Collapsed;

        #endregion


        #region IsPaneOpen Property

        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnIsPaneOpenPropertyChanged));

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

        private static void OnIsPaneOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region CompactModeThresholdWidth Property

        public static readonly DependencyProperty CompactModeThresholdWidthProperty = DependencyProperty.Register("CompactModeThresholdWidth", typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(641d, OnCompactModeThresholdWidthPropertyChanged, CoerceToGreaterThanZero));

        public double CompactModeThresholdWidth
        {
            get
            {
                return (double)GetValue(CompactModeThresholdWidthProperty);
            }
            set
            {
                SetValue(CompactModeThresholdWidthProperty, value);
            }
        }

        private static void OnCompactModeThresholdWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region ExpandedModeThresholdWidth Property

        public static readonly DependencyProperty ExpandedModeThresholdWidthProperty = DependencyProperty.Register("ExpandedModeThresholdWidth", typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(1008d, OnExpandedModeThresholdWidthPropertyChanged, CoerceToGreaterThanZero));

        public double ExpandedModeThresholdWidth
        {
            get
            {
                return (double)GetValue(ExpandedModeThresholdWidthProperty);
            }
            set
            {
                SetValue(ExpandedModeThresholdWidthProperty, value);
            }
        }

        private static void OnExpandedModeThresholdWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region FooterMenuItems Property

        public static readonly DependencyPropertyKey FooterMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("FooterMenuItems", typeof(IList), typeof(NavigationView), new FrameworkPropertyMetadata(OnFooterMenuItemsPropertyChanged));

        public static readonly DependencyProperty FooterMenuItemsProperty = FooterMenuItemsPropertyKey.DependencyProperty;

        public IList FooterMenuItems
        {
            get
            {
                return (IList)GetValue(FooterMenuItemsProperty);
            }
            //set
            //{
            //    SetValue(FooterMenuItemsPropertyKey, value);
            //}
        }

        private static void OnFooterMenuItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region FooterMenuItemsSource Property

        public static readonly DependencyProperty FooterMenuItemsSourceProperty = DependencyProperty.Register("FooterMenuItemsSource", typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(OnExpandedModeThresholdWidthPropertyChanged));

        public object FooterMenuItemsSource
        {
            get
            {
                return GetValue(FooterMenuItemsSourceProperty);
            }
            set
            {
                SetValue(FooterMenuItemsSourceProperty, value);
            }
        }

        private static void OnFooterMenuItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneFooter Property

        public static readonly DependencyProperty PaneFooterProperty = DependencyProperty.Register("PaneFooter", typeof(UIElement), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneFooterPropertyChanged));

        public UIElement PaneFooter
        {
            get
            {
                return (UIElement)GetValue(PaneFooterProperty);
            }
            set
            {
                SetValue(PaneFooterProperty, value);
            }
        }

        private static void OnPaneFooterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region Header Property

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(OnHeaderPropertyChanged));

        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region HeaderTemplate Property

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NavigationView), new FrameworkPropertyMetadata(OnHeaderTemplatePropertyChanged));

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }
            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region DisplayMode Property

        public static readonly DependencyPropertyKey DisplayModePropertyKey = DependencyProperty.RegisterReadOnly("DisplayMode", typeof(NavigationViewDisplayMode), typeof(NavigationView), new FrameworkPropertyMetadata(NavigationViewDisplayMode.Minimal, OnDisplayModePropertyChanged));

        public static readonly DependencyProperty DisplayModeProperty = DisplayModePropertyKey.DependencyProperty;

        public NavigationViewDisplayMode DisplayMode
        {
            get
            {
                return (NavigationViewDisplayMode)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModePropertyKey, value);
            }
        }

        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsSettingsVisible Property

        public static readonly DependencyProperty IsSettingsVisibleProperty = DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnIsSettingsVisiblePropertyChanged));

        public bool IsSettingsVisible
        {
            get
            {
                return (bool)GetValue(IsSettingsVisibleProperty);
            }
            set
            {
                SetValue(IsSettingsVisibleProperty, value);
            }
        }

        private static void OnIsSettingsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsPaneToggleButtonVisible Property

        public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty = DependencyProperty.Register("IsPaneToggleButtonVisible", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnIsPaneToggleButtonVisiblePropertyChanged));

        public bool IsPaneToggleButtonVisible
        {
            get
            {
                return (bool)GetValue(IsPaneToggleButtonVisibleProperty);
            }
            set
            {
                SetValue(IsPaneToggleButtonVisibleProperty, value);
            }
        }

        private static void OnIsPaneToggleButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region AlwaysShowHeader Property

        public static readonly DependencyProperty AlwaysShowHeaderProperty = DependencyProperty.Register("AlwaysShowHeader", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnAlwaysShowHeaderPropertyChanged));

        public bool AlwaysShowHeader
        {
            get
            {
                return (bool)GetValue(AlwaysShowHeaderProperty);
            }
            set
            {
                SetValue(AlwaysShowHeaderProperty, value);
            }
        }

        private static void OnAlwaysShowHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region CompactPaneLength Property

        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(48d, OnCompactPaneLengthPropertyChanged, CoerceToGreaterThanZero));

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

        private static void OnCompactPaneLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region OpenPaneLength Property

        public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(320d, OnOpenPaneLengthPropertyChanged, CoerceToGreaterThanZero));

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

        private static void OnOpenPaneLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneToggleButtonStyleProperty

        public static readonly DependencyProperty PaneToggleButtonStyleProperty = DependencyProperty.Register("PaneToggleButtonStyle", typeof(Style), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneToggleButtonStylePropertyChanged));

        public Style PaneToggleButtonStyle
        {
            get
            {
                return (Style)GetValue(PaneToggleButtonStyleProperty);
            }
            set
            {
                SetValue(PaneToggleButtonStyleProperty, value);
            }
        }

        private static void OnPaneToggleButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region SelectedItem Property

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(OnSelectedItemPropertyChanged));

        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItems Property

        private static readonly DependencyPropertyKey MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(IList), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemsPropertyChanged));

        private static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public IList MenuItems
        {
            get
            {
                return (IList)GetValue(MenuItemsProperty);
            }
            //set
            //{
            //    SetValue(MenuItemsPropertyKey, value);
            //}
        }

        private static void OnMenuItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItemsSource Property

        public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemsSourcePropertyChanged));

        public object MenuItemsSource
        {
            get
            {
                return GetValue(MenuItemsSourceProperty);
            }
            set
            {
                SetValue(MenuItemsSourceProperty, value);
            }
        }

        private static void OnMenuItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region SettingsItem Property

        public static readonly DependencyPropertyKey SettingsItemPropertyKey = DependencyProperty.RegisterReadOnly("SettingsItem", typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(OnSettingsItemPropertyChanged));

        public static readonly DependencyProperty SettingsItemProperty = SettingsItemPropertyKey.DependencyProperty;

        public object SettingsItem
        {
            get
            {
                return GetValue(SettingsItemProperty);
            }
            //set
            //{
            //    SetValue(SettingsItemPropertyKey, value);
            //}
        }

        private static void OnSettingsItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region AutoSuggestBox Property

        public static readonly DependencyProperty AutoSuggestBoxProperty = DependencyProperty.Register("AutoSuggestBox", typeof(AutoSuggestBox), typeof(NavigationView), new FrameworkPropertyMetadata(OnAutoSuggestBoxPropertyChanged));

        public AutoSuggestBox AutoSuggestBox
        {
            get
            {
                return (AutoSuggestBox)GetValue(AutoSuggestBoxProperty);
            }
            set
            {
                SetValue(AutoSuggestBoxProperty, value);
            }
        }

        private static void OnAutoSuggestBoxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItemTemplate Property

        public static readonly DependencyProperty MenuItemTemplateProperty = DependencyProperty.Register("MenuItemTemplate", typeof(DataTemplate), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemTemplatePropertyChanged));

        public DataTemplate MenuItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(MenuItemTemplateProperty);
            }
            set
            {
                SetValue(MenuItemTemplateProperty, value);
            }
        }

        private static void OnMenuItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItemTemplateSelector Property

        public static readonly DependencyProperty MenuItemTemplateSelectorProperty = DependencyProperty.Register("MenuItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemTemplateSelectorPropertyChanged));

        public DataTemplateSelector MenuItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(MenuItemTemplateSelectorProperty);
            }
            set
            {
                SetValue(MenuItemTemplateSelectorProperty, value);
            }
        }

        private static void OnMenuItemTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItemContainerStyle Property

        public static readonly DependencyProperty MenuItemContainerStyleProperty = DependencyProperty.Register("MenuItemContainerStyle", typeof(Style), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemContainerStylePropertyChanged));

        public Style MenuItemContainerStyle
        {
            get
            {
                return (Style)GetValue(MenuItemContainerStyleProperty);
            }
            set
            {
                SetValue(MenuItemContainerStyleProperty, value);
            }
        }

        private static void OnMenuItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region MenuItemContainerStyleSelector Property

        public static readonly DependencyProperty MenuItemContainerStyleSelectorProperty = DependencyProperty.Register("MenuItemContainerStyleSelector", typeof(StyleSelector), typeof(NavigationView), new FrameworkPropertyMetadata(OnMenuItemContainerStyleSelectorPropertyChanged));

        public StyleSelector MenuItemContainerStyleSelector
        {
            get
            {
                return (StyleSelector)GetValue(MenuItemContainerStyleSelectorProperty);
            }
            set
            {
                SetValue(MenuItemContainerStyleSelectorProperty, value);
            }
        }

        private static void OnMenuItemContainerStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsBackButtonVisible Property

        public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register("IsBackButtonVisible", typeof(NavigationViewBackButtonVisible), typeof(NavigationView), new FrameworkPropertyMetadata(NavigationViewBackButtonVisible.Auto, OnIsBackButtonVisiblePropertyChanged));

        public NavigationViewBackButtonVisible IsBackButtonVisible
        {
            get
            {
                return (NavigationViewBackButtonVisible)GetValue(IsBackButtonVisibleProperty);
            }
            set
            {
                SetValue(IsBackButtonVisibleProperty, value);
            }
        }

        private static void OnIsBackButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsBackEnabled Property

        public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.Register("IsBackEnabled", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(OnIsBackEnabledPropertyChanged));

        public bool IsBackEnabled
        {
            get
            {
                return (bool)GetValue(IsBackEnabledProperty);
            }
            set
            {
                SetValue(IsBackEnabledProperty, value);
            }
        }

        private static void OnIsBackEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneTitle Property

        public static readonly DependencyProperty PaneTitleProperty = DependencyProperty.Register("PaneTitle", typeof(string), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneTitlePropertyChanged));

        public string PaneTitle
        {
            get
            {
                return (string)GetValue(PaneTitleProperty);
            }
            set
            {
                SetValue(PaneTitleProperty, value);
            }
        }

        private static void OnPaneTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneDisplayMode Property

        public static readonly DependencyProperty PaneDisplayModeProperty = DependencyProperty.Register("PaneDisplayMode", typeof(NavigationViewPaneDisplayMode), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneDisplayModePropertyChanged));

        public NavigationViewPaneDisplayMode PaneDisplayMode
        {
            get
            {
                return (NavigationViewPaneDisplayMode)GetValue(PaneDisplayModeProperty);
            }
            set
            {
                SetValue(PaneDisplayModeProperty, value);
            }
        }

        private static void OnPaneDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneHeader Property

        public static readonly DependencyProperty PaneHeaderProperty = DependencyProperty.Register("PaneHeader", typeof(UIElement), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneHeaderPropertyChanged));

        public UIElement PaneHeader
        {
            get
            {
                return (UIElement)GetValue(PaneHeaderProperty);
            }
            set
            {
                SetValue(PaneHeaderProperty, value);
            }
        }

        private static void OnPaneHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region PaneCustomContent Property

        public static readonly DependencyProperty PaneCustomContentProperty = DependencyProperty.Register("PaneCustomContent", typeof(UIElement), typeof(NavigationView), new FrameworkPropertyMetadata(OnPaneCustomContentPropertyChanged));

        public UIElement PaneCustomContent
        {
            get
            {
                return (UIElement)GetValue(PaneCustomContentProperty);
            }
            set
            {
                SetValue(PaneCustomContentProperty, value);
            }
        }

        private static void OnPaneCustomContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region ContentOverlay Property

        public static readonly DependencyProperty ContentOverlayProperty = DependencyProperty.Register("ContentOverlay", typeof(UIElement), typeof(NavigationView), new FrameworkPropertyMetadata(OnContentOverlayPropertyChanged));

        public UIElement ContentOverlay
        {
            get
            {
                return (UIElement)GetValue(ContentOverlayProperty);
            }
            set
            {
                SetValue(ContentOverlayProperty, value);
            }
        }

        private static void OnContentOverlayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsPaneVisible Property

        public static readonly DependencyProperty IsPaneVisibleProperty = DependencyProperty.Register("IsPaneVisible", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnIsPaneVisiblePropertyChanged));

        public bool IsPaneVisible
        {
            get
            {
                return (bool)GetValue(IsPaneVisibleProperty);
            }
            set
            {
                SetValue(IsPaneVisibleProperty, value);
            }
        }

        private static void OnIsPaneVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region SelectionFollowsFocus Property

        public static readonly DependencyProperty SelectionFollowsFocusProperty = DependencyProperty.Register("SelectionFollowsFocus", typeof(NavigationViewSelectionFollowsFocus), typeof(NavigationView), new FrameworkPropertyMetadata(NavigationViewSelectionFollowsFocus.Disabled, OnSelectionFollowsFocusPropertyChanged));

        public NavigationViewSelectionFollowsFocus SelectionFollowsFocus
        {
            get
            {
                return (NavigationViewSelectionFollowsFocus)GetValue(SelectionFollowsFocusProperty);
            }
            set
            {
                SetValue(SelectionFollowsFocusProperty, value);
            }
        }

        private static void OnSelectionFollowsFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region TemplateSettings Property

        public static readonly DependencyPropertyKey TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(NavigationViewTemplateSettings), typeof(NavigationView), new FrameworkPropertyMetadata(OnTemplateSettingsPropertyChanged));

        public static readonly DependencyProperty TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;

        public NavigationViewTemplateSettings TemplateSettings
        {
            get
            {
                return (NavigationViewTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            set
            {
                SetValue(TemplateSettingsPropertyKey, value);
            }
        }

        private static void OnTemplateSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region ShoulderNavigationEnabled Property

        public static readonly DependencyProperty ShoulderNavigationEnabledProperty = DependencyProperty.Register("ShoulderNavigationEnabled", typeof(NavigationViewShoulderNavigationEnabled), typeof(NavigationView), new FrameworkPropertyMetadata(NavigationViewShoulderNavigationEnabled.Never, OnShoulderNavigationEnabledPropertyChanged));

        public NavigationViewShoulderNavigationEnabled ShoulderNavigationEnabled
        {
            get
            {
                return (NavigationViewShoulderNavigationEnabled)GetValue(ShoulderNavigationEnabledProperty);
            }
            set
            {
                SetValue(ShoulderNavigationEnabledProperty, value);
            }
        }

        private static void OnShoulderNavigationEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region OverflowLabelMode Property

        public static readonly DependencyProperty OverflowLabelModeProperty = DependencyProperty.Register("OverflowLabelMode", typeof(NavigationViewOverflowLabelMode), typeof(NavigationView), new FrameworkPropertyMetadata(NavigationViewOverflowLabelMode.MoreLabel, OnOverflowLabelModePropertyChanged));

        public NavigationViewOverflowLabelMode OverflowLabelMode
        {
            get
            {
                return (NavigationViewOverflowLabelMode)GetValue(OverflowLabelModeProperty);
            }
            set
            {
                SetValue(OverflowLabelModeProperty, value);
            }
        }

        private static void OnOverflowLabelModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion

        #region IsTitleBarAutoPaddingEnabled Property

        public static readonly DependencyProperty IsTitleBarAutoPaddingEnabledProperty = DependencyProperty.Register("IsTitleBarAutoPaddingEnabled", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, OnIsTitleBarAutoPaddingEnabledPropertyChanged));

        public bool IsTitleBarAutoPaddingEnabled
        {
            get
            {
                return (bool)GetValue(IsTitleBarAutoPaddingEnabledProperty);
            }
            set
            {
                SetValue(IsTitleBarAutoPaddingEnabledProperty, value);
            }
        }

        private static void OnIsTitleBarAutoPaddingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)d;
        }

        #endregion


        #region PaneStateListSizeState Property

        public static readonly DependencyPropertyKey PaneStateListSizeStatePropertyKey = DependencyProperty.RegisterReadOnly("PaneStateListSizeState", typeof(string), typeof(NavigationView), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty PaneStateListSizeStateProperty = PaneStateListSizeStatePropertyKey.DependencyProperty;

        public string PaneStateListSizeState
        {
            get
            {
                return (string)GetValue(PaneStateListSizeStateProperty);
            }
            internal set
            {
                SetValue(PaneStateListSizeStatePropertyKey, value);
            }
        }

        #endregion

        #region Methods

        static NavigationView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationView), new FrameworkPropertyMetadata(typeof(NavigationView)));
        }

        public NavigationView()
        {
            SetValue(TemplateSettingsPropertyKey, new NavigationViewTemplateSettings());

            SizeChanged += OnSizeChanged;

            m_selectionModelSource = new List<object>();
            m_selectionModelSource.Add(null);
            m_selectionModelSource.Add(null);

            ObservableCollection<object> items = new ObservableCollection<object>();
            SetValue(MenuItemsPropertyKey, items);

            ObservableCollection<object> footerItems = new ObservableCollection<object>();
            SetValue(FooterMenuItemsPropertyKey, footerItems);

            WeakReference<NavigationView> weakThis = new WeakReference<NavigationView>(this);
            m_topDataProvider.OnRawDataChanged(args =>
            {
                if (weakThis.TryGetTarget(out var target))
                {
                    target.OnTopNavDataSourceChanged(args);
                }
            });

            Unloaded += OnUnloaded;
            Loaded += OnLoaded;

            m_selectionModel.SingleSelect = true;
            m_selectionModel.Source = m_selectionModelSource;
            m_selectionModel.SelectionChanged += OnSelectionModelSelectionChanged;
            m_selectionModel.ChildrenRequested += OnSelectionModelChildrenRequested;

            m_navigationViewItemsFactory = new NavigationViewItemsFactory();
        }

        ~NavigationView()
        {
            UnhookEventsAndClearFields(true);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return base.OnCreateAutomationPeer();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Stop update anything because of PropertyChange during OnApplyTemplate. Update them all together at the end of this function
            m_appliedTemplate = false;

            UnhookEventsAndClearFields();

            // Set up the pane toggle button click handler
            if (GetTemplateChild(c_togglePaneButtonName) is Button paneToggleButton)
            {
                m_paneToggleButton = paneToggleButton;
                paneToggleButton.Click += OnPaneToggleButtonClick;

                SetPaneToggleButtonAutomationName();

                WindowChrome.SetIsHitTestVisibleInChrome(paneToggleButton, true);
            }

            m_leftNavPaneHeaderContentBorder = GetTemplateChild(c_leftNavPaneHeaderContentBorder) as ContentControl;
            m_leftNavPaneCustomContentBorder = GetTemplateChild(c_leftNavPaneCustomContentBorder) as ContentControl;
            m_leftNavFooterContentBorder = GetTemplateChild(c_leftNavFooterContentBorder) as ContentControl;
            m_paneHeaderOnTopPane = GetTemplateChild(c_paneHeaderOnTopPane) as ContentControl;
            m_paneTitleOnTopPane = GetTemplateChild(c_paneTitleOnTopPane) as ContentControl;
            m_paneCustomContentOnTopPane = GetTemplateChild(c_paneCustomContentOnTopPane) as ContentControl;
            m_paneFooterOnTopPane = GetTemplateChild(c_paneFooterOnTopPane) as ContentControl;

            // Get a pointer to the root SplitView
            if (GetTemplateChild(c_rootSplitViewName) is SplitView splitView)
            {
                m_rootSplitView = splitView;
                splitView.IsPaneOpenChanged += OnSplitViewClosedCompactChanged;
                splitView.DisplayModeChanged += OnSplitViewClosedCompactChanged;

                splitView.PaneClosed += OnSplitViewPaneClosed;
                splitView.PaneClosing += OnSplitViewPaneClosing;
                splitView.PaneOpened += OnSplitViewPaneOpened;
                splitView.PaneOpening += OnSplitViewPaneOpening;

                UpdateIsClosedCompact();
            }

            m_topNavGrid = GetTemplateChild(c_topNavGrid) as Grid;

            // Change code to NOT do this if we're in top nav mode, to prevent it from being realized:
            if (GetTemplateChild(c_menuItemsHost) is ItemsRepeater leftNavRepeater)
            {
                m_leftNavRepeater = leftNavRepeater;

                // API is currently in preview, so setting this via code.
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                if (leftNavRepeater.Layout is StackLayout stackLayoutImpl)
                {
                    stackLayoutImpl.DisableVirtualization = true;
                }

                leftNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                leftNavRepeater.ElementClearing += OnRepeaterElementClearing;

                leftNavRepeater.Loaded += OnRepeaterLoaded;

                leftNavRepeater.ItemTemplate = m_navigationViewItemsFactory;
            }

            // Change code to NOT do this if we're in left nav mode, to prevent it from being realized:
            if (GetTemplateChild(c_topNavMenuItemsHost) is ItemsRepeater topNavRepeater)
            {
                m_topNavRepeater = topNavRepeater;

                // API is currently in preview, so setting this via code
                if (topNavRepeater.Layout is StackLayout stackLayoutImpl)
                {
                    stackLayoutImpl.DisableVirtualization = true;
                }

                topNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                topNavRepeater.ElementClearing += OnRepeaterElementClearing;

                topNavRepeater.Loaded += OnRepeaterLoaded;

                // topNavRepeater.GettingFocus += OnRepeaterGettingFocus;

                topNavRepeater.ItemTemplate = m_navigationViewItemsFactory;
            }

            // Change code to NOT do this if we're in left nav mode, to prevent it from being realized:
            if (GetTemplateChild(c_topNavMenuItemsOverflowHost) is ItemsRepeater topNavListOverflowRepeater)
            {
                m_topNavRepeaterOverflowView = topNavListOverflowRepeater;

                // API is currently in preview, so setting this via code.
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                if (topNavListOverflowRepeater.Layout is StackLayout stackLayoutImpl)
                {
                    stackLayoutImpl.DisableVirtualization = true;
                }

                topNavListOverflowRepeater.ElementPrepared += OnRepeaterElementPrepared;
                topNavListOverflowRepeater.ElementClearing += OnRepeaterElementClearing;

                topNavListOverflowRepeater.ItemTemplate = m_navigationViewItemsFactory;
            }

            if (GetTemplateChild(c_topNavOverflowButton) is Button topNavOverflowButton)
            {
                m_topNavOverflowButton = topNavOverflowButton;
                AutomationProperties.SetName(topNavOverflowButton, Properties.Resources.Strings.Resources.NavigationOverflowButtonName);
                topNavOverflowButton.Content = Properties.Resources.Strings.Resources.NavigationOverflowButtonText;

                //var visual = ElementCompositionPreview.GetElementVisual(topNavOverflowButton);
                //CreateAndAttachHeaderAnimation(visual);

                object toolTip = ToolTipService.GetToolTip(topNavOverflowButton);
                if (toolTip == null)
                {
                    ToolTip tooltip = new ToolTip();
                    tooltip.Content = Properties.Resources.Strings.Resources.NavigationOverflowButtonToolTip;
                    ToolTipService.SetToolTip(topNavOverflowButton, tooltip);
                }

                if (FlyoutBase.GetAttachedFlyout(topNavOverflowButton) is FlyoutBase flyoutBase)
                {
                    flyoutBase.Closing += OnFlyoutClosing;
                }
            }

            // Change code to NOT do this if we're in top nav mode, to prevent it from being realized:
            if (GetTemplateChild(c_footerMenuItemsHost) is ItemsRepeater leftFooterMenuNavRepeater)
            {
                m_leftNavFooterMenuRepeater = leftFooterMenuNavRepeater;

                // API is currently in preview, so setting this via code.
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                if (leftFooterMenuNavRepeater.Layout is StackLayout stackLayoutImpl)
                {
                    stackLayoutImpl.DisableVirtualization = true;
                }

                leftFooterMenuNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                leftFooterMenuNavRepeater.ElementClearing += OnRepeaterElementClearing;

                leftFooterMenuNavRepeater.Loaded += OnRepeaterLoaded;

                leftFooterMenuNavRepeater.ItemTemplate = m_navigationViewItemsFactory;
            }

            // Change code to NOT do this if we're in left nav mode, to prevent it from being realized:
            if (GetTemplateChild(c_topNavFooterMenuItemsHost) is ItemsRepeater topFooterMenuNavRepeater)
            {
                m_topNavFooterMenuRepeater = topFooterMenuNavRepeater;

                // API is currently in preview, so setting this via code.
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                if (topFooterMenuNavRepeater.Layout is StackLayout stackLayoutImpl)
                {
                    stackLayoutImpl.DisableVirtualization = true;
                }

                topFooterMenuNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                topFooterMenuNavRepeater.ElementClearing += OnRepeaterElementClearing;

                topFooterMenuNavRepeater.Loaded += OnRepeaterLoaded;

                topFooterMenuNavRepeater.ItemTemplate = m_navigationViewItemsFactory;
            }

            m_topNavContentOverlayAreaGrid = GetTemplateChild(c_topNavContentOverlayAreaGrid) as Border;
            m_leftNavPaneAutoSuggestBoxPresenter = GetTemplateChild(c_leftNavPaneAutoSuggestBoxPresenter) as ContentControl;
            m_topNavPaneAutoSuggestBoxPresenter = GetTemplateChild(c_topNavPaneAutoSuggestBoxPresenter) as ContentControl;

            // Get pointer to the pane content area, for use in the selection indicator animation
            m_paneContentGrid = GetTemplateChild(c_paneContentGridName) as UIElement;

            m_contentLeftPadding = GetTemplateChild(c_contentLeftPadding) as FrameworkElement;

            m_paneHeaderCloseButtonColumn = GetTemplateChild(c_paneHeaderCloseButtonColumn) as ColumnDefinition;
            m_paneHeaderToggleButtonColumn = GetTemplateChild(c_paneHeaderToggleButtonColumn) as ColumnDefinition;
            m_paneHeaderContentBorderRow = GetTemplateChild(c_paneHeaderContentBorderRow) as RowDefinition;
            m_paneTitleFrameworkElement = GetTemplateChild(c_paneTitleFrameworkElement) as FrameworkElement;
            m_paneTitlePresenter = GetTemplateChild(c_paneTitlePresenter) as ContentControl;

            if (GetTemplateChild(c_paneTitleHolderFrameworkElement) is FrameworkElement paneTitleHolderFrameworkElement)
            {
                m_paneTitleHolderFrameworkElement = paneTitleHolderFrameworkElement;
                paneTitleHolderFrameworkElement.SizeChanged += OnPaneTitleHolderSizeChanged;
            }

            // Set automation name on search button
            if (GetTemplateChild(c_searchButtonName) is Button button)
            {
                m_paneSearchButton = button;
                button.Click += OnPaneSearchButtonClick;

                string searchButtonName = Properties.Resources.Strings.Resources.NavigationViewSearchButtonName;
                AutomationProperties.SetName(button, searchButtonName);
                ToolTip toolTip = new ToolTip();
                toolTip.Content = searchButtonName;
                ToolTipService.SetToolTip(button, toolTip);
            }

            if (GetTemplateChild(c_navViewBackButton) is Button backButton)
            {
                m_backButton = backButton;
                backButton.Click += OnBackButtonClicked;

                string navigationName = Properties.Resources.Strings.Resources.NavigationBackButtonName;
                AutomationProperties.SetName(backButton, navigationName);
            }

            // Register for changes in title bar layout
            if (Window.GetWindow(this) is ModernWindow window && window.GetTitleBar() is CoreApplicationViewTitleBar coreTitleBar)
            {
                m_coreTitleBar = coreTitleBar;
                coreTitleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
                coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;

                if (ShouldPreserveNavigationViewRS4Behavior())
                {
                    m_togglePaneTopPadding = GetTemplateChild(c_togglePaneTopPadding) as FrameworkElement;
                    m_contentPaneTopPadding = GetTemplateChild(c_contentPaneTopPadding) as FrameworkElement;
                }
            }

            if (GetTemplateChild(c_navViewBackButtonToolTip) is ToolTip backButtonToolTip)
            {
                backButtonToolTip.Content = Properties.Resources.Strings.Resources.NavigationBackButtonToolTip;
            }

            if (GetTemplateChild(c_navViewCloseButton) is Button closeButton)
            {
                m_closeButton = closeButton;
                closeButton.Click += OnPaneToggleButtonClick;

                AutomationProperties.SetName(closeButton, Properties.Resources.Strings.Resources.NavigationCloseButtonName);

                WindowChrome.SetIsHitTestVisibleInChrome(closeButton, true);
            }

            if (GetTemplateChild(c_navViewCloseButtonToolTip) is ToolTip closeButtonToolTip)
            {
                closeButtonToolTip.Content = Properties.Resources.Strings.Resources.NavigationButtonOpenName;
            }

            m_itemsContainerRow = GetTemplateChild(c_itemsContainerRow) as RowDefinition;
            m_menuItemsScrollViewer = GetTemplateChild(c_menuItemsScrollViewer) as FrameworkElement;
            m_footerItemsScrollViewer = GetTemplateChild(c_footerItemsScrollViewer) as FrameworkElement;
            m_visualItemsSeparator = GetTemplateChild(c_visualItemsSeparator) as FrameworkElement;

            if (GetTemplateChild(c_itemsContainer) is FrameworkElement itemsContainerRow)
            {
                m_itemsContainerGrid = itemsContainerRow;
                m_itemsContainerGrid.SizeChanged += OnItemsContainerSizeChanged;
            }

            // Get hold of the outermost grid and enable XYKeyboardNavigationMode
            // However, we only want this to work in the content pane + the hamburger button (which is not inside the splitview)
            // so disable it on the grid in the content area of the SplitView
            if (GetTemplateChild(c_rootGridName) is Grid rootGrid)
            {
                KeyboardNavigation.SetDirectionalNavigation(rootGrid, KeyboardNavigationMode.Contained);
            }

            if (GetTemplateChild(c_contentGridName) is Grid contentGrid)
            {
                KeyboardNavigation.SetDirectionalNavigation(contentGrid, KeyboardNavigationMode.None);
            }

            UpdatePaneShadow();

            m_appliedTemplate = true;

            // Do initial setup
            UpdatePaneDisplayMode();
            UpdateHeaderVisibility();
            UpdatePaneTitleFrameworkElementParents();
            UpdateTitleBarPadding();
            UpdatePaneTabFocusNavigation();
            UpdateBackAndCloseButtonsVisibility();
            UpdateSingleSelectionFollowsFocusTemplateSetting();
            UpdatePaneVisibility();
            UpdateVisualState();
            UpdatePaneTitleMargins();
            UpdatePaneLayout();

            // Исправляет баг при котором, элементы Footer меню отображались неправильно.
            if(m_leftNavFooterMenuRepeater != null && m_leftNavFooterMenuRepeater.ItemsSource is IList view && view.Count > 1)
            {
                m_leftNavFooterMenuRepeater.SizeChanged += M_leftNavFooterMenuRepeater_SizeChanged;
            }
        }

        private void M_leftNavFooterMenuRepeater_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // m_leftNavFooterMenuRepeater.SizeChanged -= M_leftNavFooterMenuRepeater_SizeChanged;
            UpdatePaneLayout();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (IsTopNavigationView() && IsTopPrimaryListVisible())
            {
                if (double.IsInfinity(constraint.Width))
                {
                    // We have infinite space, so move all items to primary list
                    m_topDataProvider.MoveAllItemsToPrimaryList();
                }
                else
                {
                    HandleTopNavigationMeasureOverride(constraint);
                }
            }

            LayoutUpdated -= OnLayoutUpdated;
            LayoutUpdated += OnLayoutUpdated;
            m_layoutUpdatedToken = true;

            return base.MeasureOverride(constraint);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            m_TabKeyPrecedesFocusChange = false;

            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyEventArgs eventArgs = e;
            Key key = eventArgs.Key;

            bool handled = false;

            m_TabKeyPrecedesFocusChange = false;

            switch (key)
            {
                case Key.Tab:
                    {
                        // arrow keys navigation through ItemsRepeater don't get here
                        // so handle tab key to distinguish between tab focus and arrow focus navigation
                        m_TabKeyPrecedesFocusChange = true;
                        break;
                    }
                case Key.Left:
                    {
                        bool isAltPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

                        if (isAltPressed && IsPaneOpen && IsLightDismissible())
                        {
                            handled = AttemptClosePaneLightly();
                        }

                        break;
                    }
            }

            eventArgs.Handled = handled;

            base.OnKeyDown(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            DependencyProperty property = e.Property;

            if (property == IsPaneOpenProperty)
            {
                OnIsPaneOpenChanged();
                UpdateVisualStateForDisplayModeGroup(DisplayMode);
            }
            else if (property == CompactModeThresholdWidthProperty || property == ExpandedModeThresholdWidthProperty)
            {
                UpdateAdaptiveLayout(ActualWidth);
            }
            else if (property == AlwaysShowHeaderProperty || property == HeaderProperty)
            {
                UpdateHeaderVisibility();
            }
            else if (property == SelectedItemProperty)
            {
                OnSelectedItemPropertyChanged(e);
            }
            else if (property == PaneTitleProperty)
            {
                UpdatePaneTitleFrameworkElementParents();
                UpdateBackAndCloseButtonsVisibility();
                UpdatePaneToggleSize();
            }
            else if (property == IsBackButtonVisibleProperty)
            {
                UpdateBackAndCloseButtonsVisibility();

                UpdateAdaptiveLayout(ActualWidth);

                if (IsTopNavigationView())
                {
                    InvalidateTopNavPrimaryLayout();
                }

                // Enabling back button shifts grid instead of resizing, so let's update the layout.
                if (m_backButton != null)
                {
                    m_backButton.UpdateLayout();
                }

                UpdatePaneLayout();
            }
            else if (property == MenuItemsSourceProperty)
            {
                UpdateRepeaterItemsSource(true /*forceSelectionModelUpdate*/);
            }
            else if (property == MenuItemsProperty)
            {
                UpdateRepeaterItemsSource(true /*forceSelectionModelUpdate*/);
            }
            else if (property == FooterMenuItemsSourceProperty)
            {
                UpdateFooterRepeaterItemsSource(true /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);
            }
            else if (property == FooterMenuItemsProperty)
            {
                UpdateFooterRepeaterItemsSource(true /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);
            }
            else if (property == PaneDisplayModeProperty)
            {
                // m_wasForceClosed is set to true because ToggleButton is clicked and Pane is closed.
                // When PaneDisplayMode is changed, reset the force flag to make the Pane can be opened automatically again.
                m_wasForceClosed = false;

                CollapseTopLevelMenuItems((NavigationViewPaneDisplayMode)e.OldValue);
                UpdatePaneToggleButtonVisibility();
                UpdatePaneDisplayMode((NavigationViewPaneDisplayMode)e.OldValue, (NavigationViewPaneDisplayMode)e.NewValue);
                UpdatePaneTitleFrameworkElementParents();
                UpdatePaneVisibility();
                UpdateVisualState();
                UpdatePaneButtonsWidths();
            }
            else if (property == IsPaneVisibleProperty)
            {
                UpdatePaneVisibility();
                UpdateVisualStateForDisplayModeGroup(DisplayMode);

                // When NavView is in expaneded mode with fixed window size, setting IsPaneVisible to false doesn't closes the pane
                // We manually close/open it for this case
                if (!IsPaneVisible && IsPaneOpen)
                {
                    ClosePane();
                }

                if (IsPaneVisible && DisplayMode == NavigationViewDisplayMode.Expanded && !IsPaneOpen)
                {
                    OpenPane();
                }
            }
            else if (property == OverflowLabelModeProperty)
            {
                if (m_appliedTemplate)
                {
                    UpdateVisualStateForOverflowButton();
                    InvalidateTopNavPrimaryLayout();
                }
            }
            else if (property == AutoSuggestBoxProperty)
            {
                InvalidateTopNavPrimaryLayout();

                if (e.OldValue is AutoSuggestBox oldAutoSuggestBox)
                {
                    oldAutoSuggestBox.SuggestionChosen += OnAutoSuggestBoxSuggestionChosen;
                }

                if (e.NewValue is AutoSuggestBox newAutoSuggestBox)
                {
                    newAutoSuggestBox.SuggestionChosen += OnAutoSuggestBoxSuggestionChosen;
                }
            }
            else if (property == SelectionFollowsFocusProperty)
            {
                UpdateSingleSelectionFollowsFocusTemplateSetting();
            }
            else if (property == IsPaneToggleButtonVisibleProperty)
            {
                UpdatePaneTitleFrameworkElementParents();
                UpdateBackAndCloseButtonsVisibility();
                UpdatePaneToggleButtonVisibility();
                UpdateVisualState();
            }
            else if (property == IsSettingsVisibleProperty)
            {
                UpdateFooterRepeaterItemsSource(false /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);
            }
            else if (property == CompactPaneLengthProperty)
            {
                // Need to update receiver margins when CompactPaneLength changes
                UpdatePaneShadow();

                // Update pane-button-grid width when pane is closed and we are not in minimal
                UpdatePaneButtonsWidths();
            }
            else if (property == IsTitleBarAutoPaddingEnabledProperty)
            {
                UpdateTitleBarPadding();
            }
            else if (property == MenuItemTemplateProperty || property == MenuItemTemplateSelectorProperty)
            {
                SyncItemTemplates();
            }
        }

        /* Property Methods */

        private static object CoerceToGreaterThanZero(DependencyObject d, object baseValue)
        {
            if (baseValue is double value)
            {
                // Property coercion for OpenPaneLength, CompactPaneLength, CompactModeThresholdWidth, ExpandedModeThresholdWidth
                return Math.Max(value, 0.0);
            }

            return baseValue;
        }

        /* Hierarchical Related Functions */

        internal void Expand(NavigationViewItem item)
        {
            ChangeIsExpandedNavigationViewItem(item, true /*isExpanded*/);
        }

        internal void Collapse(NavigationViewItem item)
        {
            ChangeIsExpandedNavigationViewItem(item, false /*isExpanded*/);
        }

        /* Event Handlers */

        internal void OnRepeaterElementPrepared(ItemsRepeater ir, ItemsRepeaterElementPreparedEventArgs args)
        {
            if (args.Element is NavigationViewItemBase nvib)
            {
                NavigationViewItemBase nvibImpl = nvib;
                nvibImpl.SetNavigationViewParent(this);
                nvibImpl.IsTopLevelItem = IsTopLevelItem(nvib);

                // Visual state info propagation
                NavigationViewRepeaterPosition position = NavigationViewRepeaterPosition.LeftNav;

                if (IsTopNavigationView())
                {
                    if (ir == m_topNavRepeater)
                    {
                        position = NavigationViewRepeaterPosition.TopPrimary;
                    }
                    else if (ir == m_topNavFooterMenuRepeater)
                    {
                        position = NavigationViewRepeaterPosition.TopFooter;
                    }
                    else
                    {
                        position = NavigationViewRepeaterPosition.TopOverflow;
                    }
                }

                if (ir == m_leftNavFooterMenuRepeater)
                {
                    position = NavigationViewRepeaterPosition.LeftFooter;
                }

                nvibImpl.Position = position;

                if (GetParentNavigationViewItemForContainer(nvib) is NavigationViewItem parentNVI)
                {
                    NavigationViewItem parentNVIImpl = parentNVI;
                    int itemDepth = parentNVIImpl.ShouldRepeaterShowInFlyout() ? 0 : parentNVIImpl.Depth + 1;
                    nvibImpl.Depth = itemDepth;
                }
                else
                {
                    nvibImpl.Depth = 0;
                }

                // Apply any custom container styling
                ApplyCustomMenuItemContainerStyling(nvib, ir, args.Index);

                if (args.Element is NavigationViewItem nvi)
                {
                    // Propagate depth to children items if they exist
                    int childDepth;

                    if (position == NavigationViewRepeaterPosition.TopPrimary)
                    {
                        childDepth = 0;
                    }
                    else
                    {
                        childDepth = nvibImpl.Depth + 1;
                    }

                    nvi.PropagateDepthToChildren(childDepth);

                    // Register for item events
                    nvi.MouseDown += OnNavigationViewItemTapped;
                    nvi.KeyDown += OnNavigationViewItemKeyDown;
                    nvi.GotFocus += OnNavigationViewItemOnGotFocus;
                    nvi.IsSelectedChanged += OnNavigationViewItemIsSelectedPropertyChanged;
                    nvi.IsExpandedChanged += OnNavigationViewItemExpandedPropertyChanged;
                }
            }
        }

        internal void OnRepeaterElementClearing(ItemsRepeater ir, ItemsRepeaterElementClearingEventArgs args)
        {
            if (args.Element is NavigationViewItemBase nvib)
            {
                NavigationViewItemBase nvibImpl = nvib;
                nvibImpl.Depth = 0;
                nvibImpl.IsTopLevelItem = false;
                if (nvib is NavigationViewItem nvi)
                {
                    // Revoke all the events that we were listing to on the item
                    nvi.PreviewMouseDown -= OnNavigationViewItemTapped;
                    nvi.KeyDown -= OnNavigationViewItemKeyDown;
                    nvi.GotFocus -= OnNavigationViewItemOnGotFocus;
                    nvi.IsSelectedChanged -= OnNavigationViewItemIsSelectedPropertyChanged;
                    nvi.IsExpandedChanged -= OnNavigationViewItemExpandedPropertyChanged;
                }
            }
        }

        internal void OnTopNavDataSourceChanged(NotifyCollectionChangedEventArgs args)
        {
            CloseTopNavigationViewFlyout();

            // Assume that raw data doesn't change very often for navigationview.
            // So here is a simple implementation and for each data item change, it request a layout change
            // update this in the future if there is performance problem

            // If it's Uninitialized, it means that we didn't start the layout yet.
            if (m_topNavigationMode != TopNavigationViewLayoutState.Uninitialized)
            {
                m_topDataProvider.MoveAllItemsToPrimaryList();
            }

            m_lastSelectedItemPendingAnimationInTopNav = null;
        }

        /* *** */

        internal UIElement FindSelectionIndicator(object item)
        {
            if (item != null)
            {
                if (NavigationViewItemOrSettingsContentFromData(item) is NavigationViewItem container)
                {
                    if (container.GetSelectionIndicator() is UIElement indicator)
                    {
                        return indicator;
                    }
                    else
                    {
                        // Indicator was not found, so maybe the layout hasn't updated yet.
                        // So let's do that now.
                        container.UpdateLayout();
                        return container.GetSelectionIndicator();
                    }
                }
            }
            return null;
        }

        // This method attaches the series of animations which are fired off dependent upon the amount 
        // of space give and the length of the strings involved. It occurs upon re-rendering.
        internal static void CreateAndAttachHeaderAnimation(Visual visual)
        {
            /* This Method Is Empty */
        }

        internal bool IsFullScreenOrTabletMode()
        {
            Window window = null;

            if (Window.GetWindow(this) is Window current)
            {
                window = current;
            }
            else if (Application.Current.MainWindow != null)
            {
                window = Application.Current.MainWindow;
            }

            return window != null ? window.WindowState == WindowState.Maximized : false;
        }

        internal int GetNavigationViewItemCountInPrimaryList()
        {
            return m_topDataProvider.GetNavigationViewItemCountInPrimaryList();
        }

        internal int GetNavigationViewItemCountInTopNav()
        {
            return m_topDataProvider.GetNavigationViewItemCountInTopNav();
        }

        internal SplitView GetSplitView()
        {
            return m_rootSplitView;
        }

        internal TopNavigationViewDataProvider GetTopDataProvider()
        {
            return m_topDataProvider;
        }

        internal void TopNavigationViewItemContentChanged()
        {
            if (m_appliedTemplate)
            {
                m_topDataProvider.InvalidWidthCache();
                InvalidateMeasure();
            }
        }

        internal NavigationViewItemsFactory GetNavigationViewItemsFactory()
        {
            return m_navigationViewItemsFactory;
        }

        internal void OnSettingsInvoked()
        {
            NavigationViewItem settingsItem = m_settingsItem;
            if (settingsItem != null)
            {
                OnNavigationViewItemInvoked(settingsItem);
            }
        }

        /* Used in AutomationPeer */

        internal ItemsRepeater LeftNavRepeater()
        {
            return m_leftNavRepeater;
        }

        internal NavigationViewItem GetSelectedContainer()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem is NavigationViewItem selectedItemContainer)
                {
                    return selectedItemContainer;
                }
                else
                {
                    return NavigationViewItemOrSettingsContentFromData(SelectedItem);
                }
            }
            return null;
        }

        internal ItemsRepeater GetParentItemsRepeaterForContainer(NavigationViewItemBase nvib)
        {
            if (VisualTreeHelper.GetParent(nvib) is DependencyObject parent)
            {
                if (parent is ItemsRepeater parentIR)
                {
                    return parentIR;
                }
            }
            return null;
        }

        internal ItemsRepeater GetParentRootItemsRepeaterForContainer(NavigationViewItemBase nvib)
        {
            ItemsRepeater parentIR = GetParentItemsRepeaterForContainer(nvib);
            NavigationViewItemBase currentNvib = nvib;
            while (!IsRootItemsRepeater(parentIR))
            {
                currentNvib = GetParentNavigationViewItemForContainer(currentNvib);
                //if (currentNvib == null)
                //{
                //    return null;
                //}

                parentIR = GetParentItemsRepeaterForContainer(currentNvib);
            }
            return parentIR;
        }

        internal IndexPath GetIndexPathForContainer(NavigationViewItemBase nvib)
        {
            List<int> path = new List<int>();
            bool isInFooterMenu = false;

            DependencyObject child = nvib;
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
            {
                return IndexPath.CreateFromIndices(path);
            }

            // Search through VisualTree for a root itemsrepeater
            while (parent != null && !IsRootItemsRepeater(parent) && !IsRootGridOfFlyout(parent))
            {
                if (parent is ItemsRepeater parentIR)
                {
                    if (child is UIElement childElement)
                    {
                        path.Insert(0, parentIR.GetElementIndex(childElement));
                    }
                }
                child = parent;
                parent = VisualTreeHelper.GetParent(parent);
            }

            // If the item is in a flyout, then we need to final index of its parent
            if (IsRootGridOfFlyout(parent))
            {
                if (m_lastItemExpandedIntoFlyout is NavigationViewItem nvi)
                {
                    child = nvi;
                    parent = IsTopNavigationView() ? m_topNavRepeater : m_leftNavRepeater;
                }
            }

            // If item is in one of the disconnected ItemRepeaters, account for that in IndexPath calculations
            if (parent == m_topNavRepeaterOverflowView)
            {
                // Convert index of selected item in overflow to index in datasource
                int containerIndex = m_topNavRepeaterOverflowView.GetElementIndex(child as UIElement);
                object item = m_topDataProvider.GetOverflowItems()[containerIndex];
                int indexAtRoot = m_topDataProvider.IndexOf(item);
                path.Insert(0, indexAtRoot);
            }
            else if (parent == m_topNavRepeater)
            {
                // Convert index of selected item in overflow to index in datasource
                int containerIndex = m_topNavRepeater.GetElementIndex(child as UIElement);
                object item = m_topDataProvider.GetPrimaryItems()[containerIndex];
                int indexAtRoot = m_topDataProvider.IndexOf(item);
                path.Insert(0, indexAtRoot);
            }
            else if (parent is ItemsRepeater parentIR)
            {
                path.Insert(0, parentIR.GetElementIndex(child as UIElement));
            }

            isInFooterMenu = parent == m_leftNavFooterMenuRepeater || parent == m_topNavFooterMenuRepeater;

            path.Insert(0, isInFooterMenu ? c_footerMenuBlockIndex : c_mainMenuBlockIndex);

            return IndexPath.CreateFromIndices(path);
        }

        internal object MenuItemFromContainer(DependencyObject container)
        {
            if (container != null)
            {
                if (container is NavigationViewItemBase nvib)
                {
                    if (GetParentItemsRepeaterForContainer(nvib) is ItemsRepeater parentRepeater)
                    {
                        int containerIndex = parentRepeater.GetElementIndex(nvib);
                        if (containerIndex >= 0)
                        {
                            return GetItemFromIndex(parentRepeater, containerIndex);
                        }
                    }
                }
            }
            return null;
        }

        /* Selection handling functions */

        internal void OnNavigationViewItemInvoked(NavigationViewItem nvi)
        {
            m_shouldRaiseItemInvokedAfterSelection = true;

            object selectedItem = SelectedItem;
            bool updateSelection = m_selectionModel != null && nvi.SelectsOnInvoked;
            if (updateSelection)
            {
                IndexPath ip = GetIndexPathForContainer(nvi);

                // Determine if we will update collapse/expand which will happen iff the item has children
                if (DoesNavigationViewItemHaveChildren(nvi))
                {
                    m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = true;
                }
                UpdateSelectionModelSelection(ip); // TODO Выделение здесь
            }

            // Item was invoked but already selected, so raise event here.
            if (selectedItem == SelectedItem)
            {
                RaiseItemInvokedForNavigationViewItem(nvi);
            }

            ToggleIsExpandedNavigationViewItem(nvi);
            ClosePaneIfNeccessaryAfterItemIsClicked(nvi);

            if (updateSelection)
            {
                CloseFlyoutIfRequired(nvi);
            }
        }

        private void OnNavigationViewItemIsSelectedPropertyChanged(DependencyObject sender, DependencyProperty args)
        {
            if (sender is NavigationViewItem nvi)
            {
                // Check whether the container that triggered this call back is the selected container
                bool isContainerSelectedInModel = IsContainerTheSelectedItemInTheSelectionModel(nvi);
                bool isSelectedInContainer = nvi.IsSelected;

                if (isSelectedInContainer && !isContainerSelectedInModel)
                {
                    IndexPath indexPath = GetIndexPathForContainer(nvi);
                    UpdateSelectionModelSelection(indexPath);
                }
                else if (!isSelectedInContainer && isContainerSelectedInModel)
                {
                    IndexPath indexPath = GetIndexPathForContainer(nvi);
                    IndexPath indexPathFromModel = m_selectionModel.SelectedIndex;

                    if (indexPathFromModel != null && indexPath.CompareTo(indexPathFromModel) == 0)
                    {
                        m_selectionModel.DeselectAt(indexPath);
                    }
                }

                if (isSelectedInContainer)
                {
                    nvi.IsChildSelected = false;
                }
            }
        }

        private void OnSelectionModelSelectionChanged(SelectionModel selectionModel, SelectionModelSelectionChangedEventArgs e)
        {
            object selectedItem = selectionModel.SelectedItem;

            // Ignore this callback if:
            // 1. the SelectedItem property of NavigationView is already set to the item
            //    being passed in this callback. This is because the item has already been selected
            //    via API and we are just updating the m_selectionModel state to accurately reflect the new selection.
            // 2. Template has not been applied yet. SelectionModel's selectedIndex state will get properly updated
            //    after the repeater finishes loading.
            // TODO: Update SelectedItem comparison to work for the exact same item datasource scenario
            if (m_shouldIgnoreNextSelectionChange || selectedItem == SelectedItem || !m_appliedTemplate)
            {
                return;
            }

            bool setSelectedItem = true;
            IndexPath selectedIndex = selectionModel.SelectedIndex;

            if (IsTopNavigationView())
            {
                // If selectedIndex does not exist, means item is being deselected through API
                bool isInOverflow = (selectedIndex != null && selectedIndex.GetSize() > 1)
                    ? selectedIndex.GetAt(0) == c_mainMenuBlockIndex && !m_topDataProvider.IsItemInPrimaryList(selectedIndex.GetAt(1))
                    : false;

                if (isInOverflow)
                {
                    // We only want to close the overflow flyout and move the item on selection if it is a leaf node
                    bool itemShouldBeMoved = false;
                    if (GetContainerForIndexPath(selectedIndex) is NavigationViewItemBase selectedContainer)
                    {
                        if (selectedContainer is NavigationViewItem selectedNVI)
                        {
                            if (DoesNavigationViewItemHaveChildren(selectedNVI))
                            {
                                itemShouldBeMoved = false;
                            }
                        }
                    }

                    if (itemShouldBeMoved)
                    {
                        SelectandMoveOverflowItem(selectedItem, selectedIndex, true /*closeFlyout*/);
                        setSelectedItem = false;
                    }
                    else
                    {
                        m_moveTopNavOverflowItemOnFlyoutClose = true;
                    }
                }
            }

            if (setSelectedItem)
            {
                SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(selectedItem);
            }
        }

        private void OnSelectionModelChildrenRequested(SelectionModel selectionModel, SelectionModelChildrenRequestedEventArgs e)
        {
            // this is main menu or footer
            if (e.SourceIndex.GetSize() == 1)
            {
                e.Children = e.Source;
            }
            else if (e.Source is NavigationViewItem nvi)
            {
                e.Children = GetChildren(nvi);
            }
            else if (GetChildrenForItemInIndexPath(e.SourceIndex, true /*forceRealize*/) is object children)
            {
                e.Children = children;
            }
        }

        private void OnSelectedItemPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            object newItem = args.NewValue;
            object oldItem = args.OldValue;

            ChangeSelection(oldItem, newItem);

            if (m_appliedTemplate && IsTopNavigationView())
            {
                if (!m_layoutUpdatedToken ||
                    (newItem != null && m_topDataProvider.IndexOf(newItem) != s_itemNotFound && m_topDataProvider.IndexOf(newItem, NavigationViewSplitVectorID.PrimaryList) == s_itemNotFound)) // selection is in overflow
                {
                    InvalidateTopNavPrimaryLayout();
                }
            }
        }

        private void ChangeSelection(object prevItem, object nextItem)
        {
            bool isSettingsItem = IsSettingsItem(nextItem);

            if (IsSelectionSuppressed(nextItem))
            {
                // This should not be a common codepath. Only happens if customer passes a 'selectionsuppressed' item via API.
                UndoSelectionAndRevertSelectionTo(prevItem, nextItem);
                RaiseItemInvoked(nextItem, isSettingsItem);
            }
            else
            {
                // Other transition other than default only apply to topnav
                // when clicking overflow on topnav, transition is from bottom
                // otherwise if prevItem is on left side of nextActualItem, transition is from left
                //           if prevItem is on right side of nextActualItem, transition is from right
                // click on Settings item is considered Default
                NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default;

                if (IsTopNavigationView())
                {
                    if (m_selectionChangeFromOverflowMenu)
                    {
                        recommendedDirection = NavigationRecommendedTransitionDirection.FromOverflow;
                    }
                    else if (prevItem != null && nextItem != null)
                    {
                        recommendedDirection = GetRecommendedTransitionDirection(NavigationViewItemBaseOrSettingsContentFromData(prevItem),
                            NavigationViewItemBaseOrSettingsContentFromData(nextItem));
                    }
                }

                // Bug 17850504, Customer may use NavigationViewItem.IsSelected in ItemInvoke or SelectionChanged Event.
                // To keep the logic the same as RS4, ItemInvoke is before unselect the old item
                // And SelectionChanged is after we selected the new item.
                object selectedItem = SelectedItem;
                if (m_shouldRaiseItemInvokedAfterSelection)
                {
                    // If selection changed inside ItemInvoked, the flag does not get said to false and the event get's raised again,so we need to set it to false now!
                    m_shouldRaiseItemInvokedAfterSelection = false;
                    RaiseItemInvoked(nextItem, isSettingsItem, NavigationViewItemOrSettingsContentFromData(nextItem), recommendedDirection);
                }

                // Selection was modified inside ItemInvoked, skip everything here!
                if (selectedItem != SelectedItem)
                {
                    return;
                }

                UnselectPrevItem(prevItem, nextItem);
                ChangeSelectStatusForItem(nextItem, true /*selected*/);

                try
                {
                    // Selection changed and we need to notify UIA
                    // HOWEVER expand collapse can also trigger if an item can expand/collapse
                    // There are multiple cases when selection changes:
                    // - Through click on item with no children -> No expand/collapse change
                    // - Through click on item with children -> Expand/collapse change
                    // - Through API with item without children -> No expand/collapse change
                    // - Through API with item with children -> No expand/collapse change
                    if (!m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise)
                    {
                        if (FrameworkElementAutomationPeer.FromElement(this) is AutomationPeer peer)
                        {
                            NavigationViewAutomationPeer navViewItemPeer = peer as NavigationViewAutomationPeer;
                            navViewItemPeer.RaiseSelectionChangedEvent(
                                prevItem, nextItem
                            );
                        }
                    }
                }
                finally
                {
                    m_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = false;
                }

                RaiseSelectionChangedEvent(nextItem, isSettingsItem, recommendedDirection);
                AnimateSelectionChanged(nextItem);

                if (NavigationViewItemOrSettingsContentFromData(nextItem) is NavigationViewItem nvi)
                {
                    ClosePaneIfNeccessaryAfterItemIsClicked(nvi);
                }
            }
        }

        private void UpdateSelectionModelSelection(IndexPath ip)
        {
            IndexPath prevIndexPath = m_selectionModel.SelectedIndex;
            m_selectionModel.SelectAt(ip);
            UpdateIsChildSelected(prevIndexPath, ip);
        }

        /* Item/container info functions */

        private int GetIndexFromItem(ItemsRepeater ir, object data)
        {
            if (ir != null)
            {
                if (ir.ItemsSourceView != null)
                {
                    return ir.ItemsSourceView.IndexOf(data);
                }
            }

            return -1;
        }

        private static object GetItemFromIndex(ItemsRepeater ir, int index)
        {
            if (ir != null)
            {
                if (ir.ItemsSourceView != null)
                {
                    return ir.ItemsSourceView.GetAt(index);
                }
            }
            return null;
        }

        private IndexPath GetIndexPathOfItem(object data)
        {
            if (data is NavigationViewItemBase nvib)
            {
                return GetIndexPathForContainer(nvib);
            }

            // In the databinding scenario, we need to conduct a search where we go through every item,
            // realizing it if necessary.
            if (IsTopNavigationView())
            {
                // First search through primary list
                if (SearchEntireTreeForIndexPath(m_topNavRepeater, data, false /*isFooterRepeater*/) is IndexPath ip)
                {
                    return ip;
                }

                // If item was not located in primary list, search through overflow
                if (SearchEntireTreeForIndexPath(m_topNavRepeaterOverflowView, data, false /*isFooterRepeater*/) is IndexPath ovip)
                {
                    return ovip;
                }

                // If item was not located in primary list and overflow, search through footer
                if (SearchEntireTreeForIndexPath(m_topNavFooterMenuRepeater, data, true /*isFooterRepeater*/) is IndexPath nfip)
                {
                    return nfip;
                }
            }
            else
            {
                if (SearchEntireTreeForIndexPath(m_leftNavRepeater, data, false /*isFooterRepeater*/) is IndexPath ip)
                {
                    return ip;
                }

                // If item was not located in primary list, search through footer
                if (SearchEntireTreeForIndexPath(m_leftNavFooterMenuRepeater, data, true /*isFooterRepeater*/) is IndexPath nfip)
                {
                    return nfip;
                }
            }

            return new IndexPath(new List<int>(0));
        }

        private UIElement GetContainerForIndex(int index, bool inFooter)
        {
            if (IsTopNavigationView())
            {
                // Get the repeater that is presenting the first item
                ItemsRepeater ir = inFooter ? m_topNavFooterMenuRepeater
                    : (m_topDataProvider.IsItemInPrimaryList(index) ? m_topNavRepeater : m_topNavRepeaterOverflowView);

                // Get the index of the item in the repeater
                int irIndex = inFooter ? index : m_topDataProvider.ConvertOriginalIndexToIndex(index);

                // Get the container of the first item
                if (ir.TryGetElement(irIndex) is UIElement container)
                {
                    return container;
                }
            }
            else
            {
                if ((inFooter ? m_leftNavFooterMenuRepeater.TryGetElement(index) : m_leftNavRepeater.TryGetElement(index)) is UIElement container)
                {
                    return container as NavigationViewItemBase;
                }
            }

            return null;
        }

        private NavigationViewItemBase GetContainerForIndexPath(IndexPath ip, bool lastVisible = false)
        {
            if (ip != null && ip.GetSize() > 0)
            {
                if (GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == c_footerMenuBlockIndex /*inFooter*/) is UIElement container)
                {
                    if (lastVisible)
                    {
                        if (container is NavigationViewItem nvi)
                        {
                            if (!nvi.IsExpanded)
                            {
                                return nvi;
                            }
                        }
                    }

                    // TODO: Fix below for top flyout scenario once the flyout is introduced in the XAML.
                    // We want to be able to retrieve containers for items that are in the flyout.
                    // This will return nullptr if requesting children containers of
                    // items in the primary list, or unrealized items in the overflow popup.
                    // However this should not happen.
                    return GetContainerForIndexPath(container, ip, lastVisible);
                }
            }
            return null;
        }

        private NavigationViewItemBase GetContainerForIndexPath(UIElement firstContainer, IndexPath ip, bool lastVisible)
        {
            UIElement container = firstContainer;
            if (ip.GetSize() > 2)
            {
                for (int i = 2; i < ip.GetSize(); i++)
                {
                    bool succeededGettingNextContainer = false;
                    if (container is NavigationViewItem nvi)
                    {
                        if (lastVisible && nvi.IsExpanded == false)
                        {
                            return nvi;
                        }

                        if (nvi.GetRepeater() is ItemsRepeater nviRepeater)
                        {
                            if (nviRepeater.TryGetElement(ip.GetAt(i)) is UIElement nextContainer)
                            {
                                container = nextContainer;
                                succeededGettingNextContainer = true;
                            }
                        }
                    }
                    // If any of the above checks failed, it means something went wrong and we have an index for a non-existent repeater.
                    if (!succeededGettingNextContainer)
                    {
                        return null;
                    }
                }
            }
            return container as NavigationViewItemBase;
        }

        private object GetChildrenForItemInIndexPath(IndexPath ip, bool forceRealize = false)
        {
            if (ip != null && ip.GetSize() > 1)
            {
                if (GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == c_footerMenuBlockIndex /*inFooter*/) is UIElement container)
                {
                    return GetChildrenForItemInIndexPath(container, ip, forceRealize);
                }
            }
            return null;
        }

        private object GetChildrenForItemInIndexPath(UIElement firstContainer, IndexPath ip, bool forceRealize = false)
        {
            UIElement container = firstContainer;
            bool shouldRecycleContainer = false;

            if (ip.GetSize() > 2)
            {
                for (int i = 2; i < ip.GetSize(); i++)
                {
                    bool succeededGettingNextContainer = false;

                    if (container is NavigationViewItem navViewitem)
                    {
                        int nextContainerIndex = ip.GetAt(i);
                        ItemsRepeater nviRepeater = navViewitem.GetRepeater();

                        if (nviRepeater != null && DoesRepeaterHaveRealizedContainers(nviRepeater))
                        {
                            if (nviRepeater.TryGetElement(nextContainerIndex) is UIElement nextContainer)
                            {
                                container = nextContainer;
                                succeededGettingNextContainer = true;
                            }
                        }
                        else if (forceRealize)
                        {
                            if (GetChildren(navViewitem) is object childrenData)
                            {
                                if (shouldRecycleContainer)
                                {
                                    RecycleContainer(navViewitem);
                                    shouldRecycleContainer = false;
                                }

                                // Get children data in an enumarable form
                                ItemsSourceView newDataSource = childrenData as ItemsSourceView;
                                if (childrenData != null && newDataSource == null)
                                {
                                    newDataSource = new InspectingDataSource(childrenData);
                                }

                                if (newDataSource.GetAt(nextContainerIndex) is object data)
                                {
                                    // Resolve databinding for item and search through that item's children
                                    if (ResolveContainerForItem(data, nextContainerIndex) is object nvib)
                                    {
                                        if (nvib is NavigationViewItem nextContainer)
                                        {
                                            // Process x:bind
                                            //if (CachedVisualTreeHelpers.GetDataTemplateComponent(nextContainer) is { } extension)
                                            //{
                                            //    // Clear out old data. 
                                            //    extension.Recycle();
                                            //    int nextPhase = VirtualizationInfo.PhaseReachedEnd;
                                            //    // Run Phase 0
                                            //    extension.ProcessBindings(data, nextContainerIndex, 0 /* currentPhase */, nextPhase);

                                            //    // TODO: If nextPhase is not -1, ProcessBinding for all the phases
                                            //}

                                            container = nextContainer;
                                            shouldRecycleContainer = true;
                                            succeededGettingNextContainer = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // If any of the above checks failed, it means something went wrong and we have an index for a non-existent repeater.
                    if (!succeededGettingNextContainer)
                    {
                        return null;
                    }
                }
            }

            if (container is NavigationViewItem nvi)
            {
                object children = GetChildren(nvi);
                if (shouldRecycleContainer)
                {
                    RecycleContainer(nvi);
                }
                return children;
            }

            return null;
        }

        private UIElement SearchEntireTreeForContainer(ItemsRepeater rootRepeater, object data)
        {
            // TODO: Temporary inefficient solution that results in unnecessary time complexity, fix.
            int index = GetIndexFromItem(rootRepeater, data);
            if (index != -1)
            {
                return rootRepeater.TryGetElement(index);
            }

            for (int i = 0; i < GetContainerCountInRepeater(rootRepeater); i++)
            {
                if (rootRepeater.TryGetElement(i) is UIElement container)
                {
                    // TODO Здесь оптимизировать
                    if (container is NavigationViewItem nvi)
                    {
                        if (nvi.GetRepeater() is ItemsRepeater nviRepeater)
                        {
                            if (SearchEntireTreeForContainer(nviRepeater, data) is UIElement foundElement)
                            {
                                return foundElement;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private IndexPath SearchEntireTreeForIndexPath(ItemsRepeater rootRepeater, object data, bool isFooterRepeater)
        {
            for (int i = 0; i < GetContainerCountInRepeater(rootRepeater); i++)
            {
                if (rootRepeater.TryGetElement(i) is UIElement container)
                {
                    // TODO Здесь оптимизировать
                    if (container is NavigationViewItem nvi)
                    {
                        IndexPath ip = new IndexPath(new List<int> { isFooterRepeater ? c_footerMenuBlockIndex : c_mainMenuBlockIndex, i });
                        if (SearchEntireTreeForIndexPath(nvi, data, ip) is IndexPath indexPath)
                        {
                            return indexPath;
                        }
                    }
                }
            }

            return null;
        }

        // There are two possibilities here if the passed in item has children. Either the children of the passed in container have already been realized,
        // in which case we simply just iterate through the children containers, or they have not been realized yet and we have to iterate through the data
        // and manually realize each item.
        private IndexPath SearchEntireTreeForIndexPath(NavigationViewItem parentContainer, object data, IndexPath ip)
        {
            bool areChildrenRealized = false;
            if (parentContainer.GetRepeater() is ItemsRepeater childrenRepeater)
            {
                if (DoesRepeaterHaveRealizedContainers(childrenRepeater))
                {
                    areChildrenRealized = true;
                    for (int i = 0; i < GetContainerCountInRepeater(childrenRepeater); i++)
                    {
                        if (childrenRepeater.TryGetElement(i) is UIElement container)
                        {
                            if (container is NavigationViewItem nvi)
                            {
                                IndexPath newIndexPath = ip.CloneWithChildIndex(i);
                                if (nvi.Content == data)
                                {
                                    return newIndexPath;
                                }
                                else
                                {
                                    if (SearchEntireTreeForIndexPath(nvi, data, newIndexPath) is IndexPath foundIndexPath)
                                    {
                                        return foundIndexPath;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //If children are not realized, manually realize and search.
            if (!areChildrenRealized)
            {
                if (GetChildren(parentContainer) is object childrenData)
                {
                    // Get children data in an enumarable form
                    var newDataSource = childrenData as ItemsSourceView;
                    if (childrenData != null && newDataSource == null)
                    {
                        newDataSource = new InspectingDataSource(childrenData);
                    }

                    for (int i = 0; i < newDataSource.Count; i++)
                    {
                        var newIndexPath = ip.CloneWithChildIndex(i);
                        var childData = newDataSource.GetAt(i);
                        if (childData == data)
                        {
                            return newIndexPath;
                        }
                        else
                        {
                            // Resolve databinding for item and search through that item's children
                            if (ResolveContainerForItem(childData, i) is NavigationViewItemBase nvib)
                            {
                                if (nvib is NavigationViewItem nvi)
                                {
                                    // Process x:bind
                                    //if (CachedVisualTreeHelpers.GetDataTemplateComponent(nvi) is { } extension)
                                    //{
                                    //    // Clear out old data. 
                                    //    extension.Recycle();
                                    //    int nextPhase = VirtualizationInfo.PhaseReachedEnd;
                                    //    // Run Phase 0
                                    //    extension.ProcessBindings(childData, i, 0 /* currentPhase */, nextPhase);

                                    //    // TODO: If nextPhase is not -1, ProcessBinding for all the phases
                                    //}

                                    if (SearchEntireTreeForIndexPath(nvi, data, newIndexPath) is IndexPath foundIndexPath)
                                    {
                                        return foundIndexPath;
                                    }

                                    //TODO: Recycle container!
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /* *** */

        private ItemsRepeater GetChildRepeaterForIndexPath(IndexPath ip)
        {
            if (GetContainerForIndexPath(ip) is NavigationViewItem container)
            {
                return container.GetRepeater();
            }
            return null;
        }

        private NavigationViewItem GetParentNavigationViewItemForContainer(NavigationViewItemBase nvib)
        {
            // TODO: This scenario does not find parent items when in a flyout, which causes problems if item if first loaded
            // straight in the flyout. Fix. This logic can be merged with the 'GetIndexPathForContainer' logic below.
            DependencyObject parent = GetParentItemsRepeaterForContainer(nvib);
            if (!IsRootItemsRepeater(parent))
            {
                while (parent != null)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is NavigationViewItem nvi)
                    {
                        return nvi;
                    }
                }
            }
            return null;
        }

        private bool IsContainerTheSelectedItemInTheSelectionModel(NavigationViewItemBase nvib)
        {
            if (m_selectionModel.SelectedItem is object selectedItem)
            {
                NavigationViewItemBase selectedItemContainer = selectedItem as NavigationViewItemBase;
                if (selectedItemContainer == null)
                {
                    selectedItemContainer = GetContainerForIndexPath(m_selectionModel.SelectedIndex);
                }

                return selectedItemContainer == nvib;
            }
            return false;
        }

        private int GetContainerCountInRepeater(ItemsRepeater ir)
        {
            if (ir != null)
            {
                if (ir.ItemsSourceView != null)
                {
                    return ir.ItemsSourceView.Count;
                }
            }
            return -1;
        }

        private bool DoesRepeaterHaveRealizedContainers(ItemsRepeater ir)
        {
            if (ir != null)
            {
                if (ir.TryGetElement(0) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSettingsItem(object item)
        {
            bool isSettingsItem = false;
            if (item != null)
            {
                if (m_settingsItem != null)
                {
                    isSettingsItem = (m_settingsItem == item) || (m_settingsItem.Content == item);
                }
            }
            return isSettingsItem;
        }

        private bool IsSelectionSuppressed(object item)
        {
            if (item != null)
            {
                if (NavigationViewItemOrSettingsContentFromData(item) is NavigationViewItem nvi)
                {
                    return !nvi.SelectsOnInvoked;
                }
            }

            return false;
        }

        private bool DoesNavigationViewItemHaveChildren(NavigationViewItem nvi)
        {
            return nvi.MenuItems.Count > 0 || nvi.MenuItemsSource != null || nvi.HasUnrealizedChildren;
        }

        private bool IsTopLevelItem(NavigationViewItemBase nvib)
        {
            return IsRootItemsRepeater(GetParentItemsRepeaterForContainer(nvib));
        }

        private object GetChildren(NavigationViewItem nvi)
        {
            if (nvi.MenuItems.Count > 0)
            {
                return nvi.MenuItems;
            }

            return nvi.MenuItemsSource;
        }

        /* Hierarchy related functions */
        private void ToggleIsExpandedNavigationViewItem(NavigationViewItem nvi)
        {
            ChangeIsExpandedNavigationViewItem(nvi, !nvi.IsExpanded);
        }

        private void ChangeIsExpandedNavigationViewItem(NavigationViewItem nvi, bool isExpanded)
        {
            if (DoesNavigationViewItemHaveChildren(nvi))
            {
                nvi.IsExpanded = isExpanded;
            }
        }

        private void ShowHideChildrenItemsRepeater(NavigationViewItem nvi)
        {

            nvi.ShowHideChildren();

            if (nvi.ShouldRepeaterShowInFlyout())
            {
                m_lastItemExpandedIntoFlyout = nvi.IsExpanded ? nvi : null;
            }

            // If SelectedItem is being hidden/shown, animate SelectionIndicator
            if (!nvi.IsSelected && nvi.IsChildSelected)
            {
                if (!nvi.IsRepeaterVisible() && nvi.IsChildSelected)
                {
                    AnimateSelectionChanged(nvi);
                }
                else
                {
                    AnimateSelectionChanged(FindLowestLevelContainerToDisplaySelectionIndicator());
                }
            }

            nvi.RotateExpandCollapseChevron(nvi.IsExpanded);
        }

        private NavigationViewItem FindLowestLevelContainerToDisplaySelectionIndicator()
        {
            int indexIntoIndex = 1;
            IndexPath selectedIndex = m_selectionModel.SelectedIndex;
            if (selectedIndex != null && selectedIndex.GetSize() > 1)
            {
                if (GetContainerForIndex(selectedIndex.GetAt(indexIntoIndex), selectedIndex.GetAt(0) == c_footerMenuBlockIndex /* inFooter */) is UIElement container)
                {
                    if (container is NavigationViewItem nvi)
                    {
                        NavigationViewItem nviImpl = nvi;
                        bool isRepeaterVisible = nviImpl.IsRepeaterVisible();
                        while (nvi != null && isRepeaterVisible && !nvi.IsSelected && nvi.IsChildSelected)
                        {
                            indexIntoIndex++;
                            isRepeaterVisible = false;
                            if (nviImpl.GetRepeater() is ItemsRepeater repeater)
                            {
                                if (repeater.TryGetElement(selectedIndex.GetAt(indexIntoIndex)) is UIElement childContainer)
                                {
                                    nvi = childContainer as NavigationViewItem;
                                    nviImpl = nvi;
                                    isRepeaterVisible = nviImpl.IsRepeaterVisible();
                                }
                            }
                        }
                        return nvi;
                    }
                }
            }
            return null;
        }

        private void UpdateIsChildSelectedForIndexPath(IndexPath ip, bool isChildSelected)
        {
            // Update the isChildSelected property for every container on the IndexPath (with the exception of the actual container pointed to by the indexpath)
            UIElement container = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == c_footerMenuBlockIndex /*inFooter*/);
            // first index is fo mainmenu or footer
            // second is index of item in mainmenu or footer
            // next in menuitem children 
            int index = 2;
            while (container != null)
            {
                if (container is NavigationViewItem nvi)
                {
                    nvi.IsChildSelected = isChildSelected;
                    if (nvi.GetRepeater() is ItemsRepeater nextIR)
                    {
                        if (index < ip.GetSize() - 1)
                        {
                            container = nextIR.TryGetElement(ip.GetAt(index));
                            index++;
                            continue;
                        }
                    }
                }
                container = null;
            }
        }

        private void UpdateIsChildSelected(IndexPath prevIP, IndexPath nextIP)
        {
            if (prevIP != null && prevIP.GetSize() > 0)
            {
                UpdateIsChildSelectedForIndexPath(prevIP, false /*isChildSelected*/);
            }

            if (nextIP != null && nextIP.GetSize() > 0)
            {
                UpdateIsChildSelectedForIndexPath(nextIP, true /*isChildSelected*/);
            }
        }

        private void CollapseTopLevelMenuItems(NavigationViewPaneDisplayMode oldDisplayMode)
        {
            // We want to make sure only top level items are visible when switching pane modes
            if (oldDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                CollapseMenuItemsInRepeater(m_topNavRepeater);
                CollapseMenuItemsInRepeater(m_topNavRepeaterOverflowView);
            }
            else
            {
                CollapseMenuItemsInRepeater(m_leftNavRepeater);
            }
        }

        private void CollapseMenuItemsInRepeater(ItemsRepeater ir)
        {
            for (int index = 0; index < GetContainerCountInRepeater(ir); index++)
            {
                if (ir.TryGetElement(index) is UIElement element)
                {
                    if (element is NavigationViewItem nvi)
                    {
                        ChangeIsExpandedNavigationViewItem(nvi, false /*isExpanded*/);
                    }
                }
            }
        }

        private void RaiseExpandingEvent(NavigationViewItemBase container)
        {
            NavigationViewItemExpandingEventArgs eventArgs = new NavigationViewItemExpandingEventArgs(this);
            eventArgs.ExpandingItemContainer = container;
            Expanding?.Invoke(this, eventArgs);
        }

        private void RaiseCollapsedEvent(NavigationViewItemBase container)
        {
            NavigationViewItemCollapsedEventArgs eventArgs = new NavigationViewItemCollapsedEventArgs(this);
            eventArgs.CollapsedItemContainer = container;
            Collapsed?.Invoke(this, eventArgs);
        }

        // We only need to close the flyout if the selected item is a leaf node
        private void CloseFlyoutIfRequired(NavigationViewItem selectedItem)
        {
            IndexPath selectedIndex = m_selectionModel.SelectedIndex;

            bool isInModeWithFlyout = false;

            if (m_rootSplitView is SplitView splitView)
            {
                // Check if the pane is closed and if the splitview is in either compact mode.
                SplitViewDisplayMode splitViewDisplayMode = splitView.DisplayMode;
                isInModeWithFlyout = (!splitView.IsPaneOpen && (splitViewDisplayMode == SplitViewDisplayMode.CompactOverlay || splitViewDisplayMode == SplitViewDisplayMode.CompactInline)) ||
                        PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
            }

            if (isInModeWithFlyout && selectedIndex != null && !DoesNavigationViewItemHaveChildren(selectedItem))
            {
                // Item selected is a leaf node, find top level parent and close flyout
                if (GetContainerForIndex(selectedIndex.GetAt(1), selectedIndex.GetAt(0) == c_footerMenuBlockIndex /* inFooter */) is UIElement rootItem)
                {
                    if (rootItem is NavigationViewItem nvi)
                    {
                        NavigationViewItem nviImpl = nvi;
                        if (nviImpl.ShouldRepeaterShowInFlyout())
                        {
                            nvi.IsExpanded = false;
                        }
                    }
                }
            }
        }

        /* Force realization functions */

        private NavigationViewItemBase ResolveContainerForItem(object item, int index)
        {
            ElementFactoryGetArgs args = new ElementFactoryGetArgs();
            args.Data = item;
            args.Index = index;

            if (m_navigationViewItemsFactory.GetElement(args) is UIElement container)
            {
                if (container is NavigationViewItemBase nvib)
                {
                    return nvib;
                }
            }

            return null;
        }

        private void RecycleContainer(UIElement container)
        {
            ElementFactoryRecycleArgs args = new ElementFactoryRecycleArgs();
            args.Element = container;

            m_navigationViewItemsFactory.RecycleElement(args);
        }

        private void OnFlyoutClosing(object sender, FlyoutBaseClosingEventArgs args)
        {
            // If the user selected an parent item in the overflow flyout then the item has not been moved to top primary yet.
            // So we need to move it.
            if (m_moveTopNavOverflowItemOnFlyoutClose && !m_selectionChangeFromOverflowMenu)
            {
                m_moveTopNavOverflowItemOnFlyoutClose = false;

                IndexPath selectedIndex = m_selectionModel.SelectedIndex;
                if (selectedIndex.GetSize() > 0)
                {
                    if (GetContainerForIndex(selectedIndex.GetAt(1), false /*infooter*/) is UIElement firstContainer)
                    {
                        if (firstContainer is NavigationViewItem firstNVI)
                        {
                            // We want to collapse the top level item before we move it
                            firstNVI.IsExpanded = false;
                        }
                    }

                    SelectandMoveOverflowItem(SelectedItem, selectedIndex, false /*closeFlyout*/);
                }
            }
        }

        private void ClosePaneIfNeccessaryAfterItemIsClicked(NavigationViewItem selectedItem)
        {
            if (IsPaneOpen && DisplayMode != NavigationViewDisplayMode.Expanded &&
                !DoesNavigationViewItemHaveChildren(selectedItem) &&
                !m_shouldIgnoreNextSelectionChange)
            {
                ClosePane();
            }
        }

        private bool NeedTopPaddingForRS5OrHigher(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Starting on RS5, we will be using the following IsVisible API together with ExtendViewIntoTitleBar
            // to decide whether to try to add top padding or not.
            // We don't add padding when in fullscreen or tablet mode.
            return coreTitleBar.IsVisible && coreTitleBar.ExtendViewIntoTitleBar && !IsFullScreenOrTabletMode();
        }

        // private void OnAccessKeyInvoked(object sender, AccessKeyInvokedEventArgs  args);

        private NavigationTransitionInfo CreateNavigationTransitionInfo(NavigationRecommendedTransitionDirection recommendedTransitionDirection)
        {
            System.Diagnostics.Debug.WriteLine("CreateNavigationTransitionInfo: Not Implemented");
            return null;
            // TODO Анимации выбора
            // Над этим нужно поработать
            //throw new NotImplementedException();
        }

        private NavigationRecommendedTransitionDirection GetRecommendedTransitionDirection(DependencyObject prev, DependencyObject next)
        {
            NavigationRecommendedTransitionDirection recommendedTransitionDirection = NavigationRecommendedTransitionDirection.Default;
            ItemsRepeater ir = m_topNavRepeater;

            if (prev != null && next != null && ir != null)
            {
                IndexPath prevIndexPath = GetIndexPathForContainer(prev as NavigationViewItemBase);
                IndexPath nextIndexPath = GetIndexPathForContainer(next as NavigationViewItemBase);

                int compare = prevIndexPath.CompareTo(nextIndexPath);

                switch (compare)
                {
                    case -1:
                        recommendedTransitionDirection = NavigationRecommendedTransitionDirection.FromRight;
                        break;
                    case 1:
                        recommendedTransitionDirection = NavigationRecommendedTransitionDirection.FromLeft;
                        break;
                    default:
                        recommendedTransitionDirection = NavigationRecommendedTransitionDirection.Default;
                        break;
                }
            }
            return recommendedTransitionDirection;
        }

        private NavigationViewTemplateSettings GetTemplateSettings()
        {
            return TemplateSettings;
        }

        private bool IsNavigationViewListSingleSelectionFollowsFocus()
        {
            return (SelectionFollowsFocus == NavigationViewSelectionFollowsFocus.Enabled);
        }

        private void UpdateSingleSelectionFollowsFocusTemplateSetting()
        {
            GetTemplateSettings().SingleSelectionFollowsFocus = IsNavigationViewListSingleSelectionFollowsFocus();
        }

        private void OnMenuItemsSourceCollectionChanged(object sender, object args)
        {
            if (!IsTopNavigationView())
            {
                if (m_leftNavRepeater is ItemsRepeater repeater)
                {
                    repeater.UpdateLayout();
                }
                UpdatePaneLayout();
            }
        }

        private void OnFooterItemsSourceCollectionChanged(object sender, object args)
        {
            UpdateFooterRepeaterItemsSource(false /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);

            // Pane footer items changed. This means we might need to reevaluate the pane layout.
            UpdatePaneLayout();
        }

        private void OnOverflowItemsSourceCollectionChanged(object sender, object args)
        {
            if (m_topNavRepeaterOverflowView.ItemsSourceView.Count == 0)
            {
                SetOverflowButtonVisibility(Visibility.Collapsed);
            }
        }

        private void SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(object item)
        {
            SelectedItem = item;
        }

        private void ChangeSelectStatusForItem(object item, bool selected)
        {
            if (NavigationViewItemOrSettingsContentFromData(item) is NavigationViewItem container)
            {
                // If we unselect an item, ListView doesn't tolerate setting the SelectedItem to nullptr. 
                // Instead we remove IsSelected from the item itself, and it make ListView to unselect it.
                // If we select an item, we follow the unselect to simplify the code.
                container.IsSelected = selected;
            }
            else if (selected)
            {
                // If we are selecting an item and have not found a realized container for it,
                // we may need to manually resolve a container for this in order to update the
                // SelectionModel's selected IndexPath.
                IndexPath ip = GetIndexPathOfItem(item);
                if (ip != null && ip.GetSize() > 0)
                {
                    // The SelectedItem property has already been updated. So we want to block any logic from executing
                    // in the SelectionModel selection changed callback.
                    try
                    {
                        m_shouldIgnoreNextSelectionChange = true;
                        UpdateSelectionModelSelection(ip);
                    }
                    finally
                    {
                        m_shouldIgnoreNextSelectionChange = false;
                    }
                }
            }
        }

        private void UnselectPrevItem(object prevItem, object nextItem)
        {
            if (prevItem != null && prevItem != nextItem)
            {
                bool setIgnoreNextSelectionChangeToFalse = !m_shouldIgnoreNextSelectionChange;
                try
                {
                    m_shouldIgnoreNextSelectionChange = true;
                    ChangeSelectStatusForItem(prevItem, false /*selected*/);
                }
                finally
                {
                    if (setIgnoreNextSelectionChangeToFalse)
                    {
                        m_shouldIgnoreNextSelectionChange = false;
                    }
                }
            }
        }

        private void UndoSelectionAndRevertSelectionTo(object prevSelectedItem, object nextItem)
        {
            object selectedItem = null;
            if (prevSelectedItem != null)
            {
                if (IsSelectionSuppressed(prevSelectedItem))
                {
                    AnimateSelectionChanged(null);
                }
                else
                {
                    ChangeSelectStatusForItem(prevSelectedItem, true /*selected*/);
                    AnimateSelectionChangedToItem(prevSelectedItem);
                    selectedItem = prevSelectedItem;
                }
            }
            else
            {
                // Bug 18033309, A SelectsOnInvoked=false item is clicked, if we don't unselect it from listview, the second click will not raise ItemClicked
                // because listview doesn't raise SelectionChange.
                ChangeSelectStatusForItem(nextItem, false /*selected*/);
            }
            SelectedItem = selectedItem;
        }

        private void CloseTopNavigationViewFlyout()
        {
            if (m_topNavOverflowButton is Button button)
            {
                if (Helpers.ButtonHelper.GetFlyout(button) is FlyoutBase flyout)
                {
                    flyout.Hide();
                }
            }
        }

        private void UpdateVisualState(bool useTransitions = false)
        {
            if (m_appliedTemplate)
            {
                AutoSuggestBox box = AutoSuggestBox;
                VisualStateManager.GoToState(this, box != null ? "AutoSuggestBoxVisible" : "AutoSuggestBoxCollapsed", false /*useTransitions*/);

                bool isVisible = IsSettingsVisible;
                VisualStateManager.GoToState(this, isVisible ? "SettingsVisible" : "SettingsCollapsed", false /*useTransitions*/);

                if (IsTopNavigationView())
                {
                    UpdateVisualStateForOverflowButton();
                }
                else
                {
                    UpdateLeftNavigationOnlyVisualState(useTransitions);
                }
            }
        }

        private void UpdateVisualStateForOverflowButton()
        {
            string state = (OverflowLabelMode == NavigationViewOverflowLabelMode.MoreLabel) ?
                "OverflowButtonWithLabel" :
                "OverflowButtonNoLabel";
            VisualStateManager.GoToState(this, state, false /* useTransitions*/);
        }

        private void UpdateLeftNavigationOnlyVisualState(bool useTransitions)
        {
            bool isToggleButtonVisible = IsPaneToggleButtonVisible;
            VisualStateManager.GoToState(this, isToggleButtonVisible ? "TogglePaneButtonVisible" : "TogglePaneButtonCollapsed", false /*useTransitions*/);
        }

        private void UpdatePaneShadow()
        {
            /* This Empty Method */
        }

        private void UpdateNavigationViewItemsFactory()
        {
            object  newItemTemplate = MenuItemTemplate;
            if (newItemTemplate == null)
            {
                newItemTemplate = MenuItemTemplateSelector;
            }

            m_navigationViewItemsFactory.UserElementFactory(newItemTemplate);
        }

        private void SyncItemTemplates()
        {
            UpdateNavigationViewItemsFactory();
        }

        private bool IsRootGridOfFlyout(DependencyObject element)
        {
            if (element is Grid grid)
            {
                return grid.Name == c_flyoutRootGrid;
            }
            return false;
        }

        private bool IsRootItemsRepeater(DependencyObject element)
        {
            if (element != null)
            {
                return (element == m_topNavRepeater ||
                    element == m_leftNavRepeater ||
                    element == m_topNavRepeaterOverflowView ||
                    element == m_leftNavFooterMenuRepeater ||
                    element == m_topNavFooterMenuRepeater);
            }
            return false;
        }

        private void RaiseItemInvoked(object item, bool isSettings, NavigationViewItemBase container = null, NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default)
        {
            object invokedItem = item;
            NavigationViewItemBase invokedContainer = container;

            NavigationViewItemInvokedEventArgs eventArgs = new NavigationViewItemInvokedEventArgs();

            if (container != null)
            {
                invokedItem = container.Content;
            }
            else
            {
                // InvokedItem is container for Settings, but Content of item for other ListViewItem
                if (!isSettings)
                {
                    if (NavigationViewItemBaseOrSettingsContentFromData(item) is NavigationViewItemBase containerFromData)
                    {
                        invokedItem = containerFromData.Content;
                        invokedContainer = containerFromData;
                    }
                }
                else
                {
                    // MUX_ASSERT(item);
                    invokedContainer = item as NavigationViewItemBase;
                    // MUX_ASSERT(invokedContainer);
                }
            }
            eventArgs.InvokedItem = invokedItem;
            eventArgs.InvokedItemContainer = invokedContainer;
            eventArgs.IsSettingsInvoked = isSettings;
            eventArgs.RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recommendedDirection);
            ItemInvoked?.Invoke(this, eventArgs);
        }

        private void RaiseItemInvokedForNavigationViewItem(NavigationViewItem nvi)
        {
            object nextItem = null;
            object prevItem = SelectedItem;
            ItemsRepeater parentIR = GetParentItemsRepeaterForContainer(nvi);

            if (parentIR.ItemsSourceView is ItemsSourceView itemsSourceView)
            {
                ItemsSourceView inspectingDataSource = (InspectingDataSource)itemsSourceView;
                int itemIndex = parentIR.GetElementIndex(nvi);

                // Check that index is NOT -1, meaning it is actually realized
                if (itemIndex != -1)
                {
                    // Something went wrong, item might not be realized yet.
                    nextItem = inspectingDataSource.GetAt(itemIndex);
                }
            }

            // Determine the recommeded transition direction.
            // Any transitions other than `Default` only apply in top nav scenarios.
            NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default;

            if (IsTopNavigationView() && nvi.SelectsOnInvoked)
            {
                bool isInOverflow = parentIR == m_topNavRepeaterOverflowView;
                if (isInOverflow)
                {
                    recommendedDirection = NavigationRecommendedTransitionDirection.FromOverflow;
                }
                else if (prevItem != null)
                {
                    recommendedDirection = GetRecommendedTransitionDirection(NavigationViewItemBaseOrSettingsContentFromData(prevItem), nvi);
                }
            }

            RaiseItemInvoked(nextItem, IsSettingsItem(nvi) /*isSettings*/, nvi, recommendedDirection);
        }

        private void HandleKeyEventForNavigationViewItem(NavigationViewItem nvi, KeyEventArgs args)
        {
            Key key = args.Key;
            switch (key)
            {
                case Key.Enter:
                case Key.Space:
                    args.Handled = true;
                    OnNavigationViewItemInvoked(nvi);
                    break;
                case Key.Home:
                    args.Handled = true;
                    KeyboardFocusFirstItemFromItem(nvi);
                    break;
                case Key.End:
                    args.Handled = true;
                    KeyboardFocusLastItemFromItem(nvi);
                    break;
                case Key.Down:
                    FocusNextDownItem(nvi, args);
                    break;
                case Key.Up:
                    FocusNextUpItem(nvi, args);
                    break;
            }
        }

        private void InvalidateTopNavPrimaryLayout()
        {
            if (m_appliedTemplate && IsTopNavigationView())
            {
                InvalidateMeasure();
            }
        }

        /* Measure functions for top navigation */
        private double MeasureTopNavigationViewDesiredWidth(Size availableSize)
        {
            double desiredWidth = 0;
            if (m_topNavGrid != null)
            {
                m_topNavGrid.Measure(availableSize);
                desiredWidth = m_topNavGrid.DesiredSize.Width;
            }

            return desiredWidth;
        }

        private double MeasureTopNavMenuItemsHostDesiredWidth(Size availableSize)
        {
            double desiredWidth = 0;
            if (m_topNavRepeater != null)
            {
                m_topNavRepeater.Measure(availableSize);
                desiredWidth = m_topNavRepeater.DesiredSize.Width;
            }

            return desiredWidth;
        }

        private double GetTopNavigationViewActualWidth()
        {
            return m_topNavGrid != null ? m_topNavGrid.ActualWidth : 0;
        }

        /* *** */

        private bool HasTopNavigationViewItemNotInPrimaryList()
        {
            return m_topDataProvider.GetPrimaryListSize() != m_topDataProvider.Size();
        }

        private void HandleTopNavigationMeasureOverride(Size availableSize)
        {
            // Determine if TopNav is in Overflow
            if (HasTopNavigationViewItemNotInPrimaryList())
            {
                HandleTopNavigationMeasureOverrideOverflow(availableSize);
            }
            else
            {
                HandleTopNavigationMeasureOverrideNormal(availableSize);
            }

            if (m_topNavigationMode == TopNavigationViewLayoutState.Uninitialized)
            {
                m_topNavigationMode = TopNavigationViewLayoutState.Initialized;
            }
        }

        private void HandleTopNavigationMeasureOverrideNormal(Size availableSize)
        {
            double desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);
            if (desiredWidth > availableSize.Width)
            {
                ResetAndRearrangeTopNavItems(availableSize);
            }
        }

        private void HandleTopNavigationMeasureOverrideOverflow(Size availableSize)
        {
            double desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);
            if (desiredWidth > availableSize.Width)
            {
                ShrinkTopNavigationSize(desiredWidth, availableSize);
            }
            else if (desiredWidth < availableSize.Width)
            {
                double fullyRecoverWidth = m_topDataProvider.WidthRequiredToRecoveryAllItemsToPrimary();
                if (availableSize.Width >= desiredWidth + fullyRecoverWidth + m_topNavigationRecoveryGracePeriodWidth)
                {
                    // It's possible to recover from Overflow to Normal state, so we restart the MeasureOverride from first step
                    ResetAndRearrangeTopNavItems(availableSize);
                }
                else
                {
                    List<int> movableItems = FindMovableItemsRecoverToPrimaryList(availableSize.Width - desiredWidth, new List<int>()/*includeItems*/);
                    m_topDataProvider.MoveItemsToPrimaryList(movableItems);
                }
            }
        }

        private void SetOverflowButtonVisibility(Visibility visibility)
        {
            if (visibility != TemplateSettings.OverflowButtonVisibility)
            {
                GetTemplateSettings().OverflowButtonVisibility = visibility;
            }
        }

        private void SelectOverflowItem(object item, IndexPath ip)
        {
            object itemBeingMoved = item;
            if (ip.GetSize() > 2)
            {
                itemBeingMoved = GetItemFromIndex(m_topNavRepeaterOverflowView, m_topDataProvider.ConvertOriginalIndexToIndex(ip.GetAt(1)));
            }

            // Calculate selected overflow item size.
            int selectedOverflowItemIndex = m_topDataProvider.IndexOf(itemBeingMoved);
            // MUX_ASSERT(selectedOverflowItemIndex != s_itemNotFound);
            double selectedOverflowItemWidth = m_topDataProvider.GetWidthForItem(selectedOverflowItemIndex);

            bool needInvalidMeasure = !m_topDataProvider.IsValidWidthForItem(selectedOverflowItemIndex);

            if (!needInvalidMeasure)
            {
                double actualWidth = GetTopNavigationViewActualWidth();
                double desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);


                // Calculate selected item size
                int selectedItemIndex = s_itemNotFound;
                double selectedItemWidth = 0;
                if (SelectedItem is object selectedItem)
                {
                    selectedItemIndex = m_topDataProvider.IndexOf(selectedItem);
                    if (selectedItemIndex != s_itemNotFound)
                    {
                        selectedItemWidth = m_topDataProvider.GetWidthForItem(selectedItemIndex);
                    }
                }

                double widthAtLeastToBeRemoved = desiredWidth + selectedOverflowItemWidth - actualWidth;

                // calculate items to be removed from primary because a overflow item is selected. 
                // SelectedItem is assumed to be removed from primary first, then added it back if it should not be removed
                List<int> itemsToBeRemoved = FindMovableItemsToBeRemovedFromPrimaryList(widthAtLeastToBeRemoved, new List<int>() /*excludeItems*/);

                // calculate the size to be removed
                double toBeRemovedItemWidth = m_topDataProvider.CalculateWidthForItems(itemsToBeRemoved);

                double widthAvailableToRecover = toBeRemovedItemWidth - widthAtLeastToBeRemoved;
                List<int> itemsToBeAdded = FindMovableItemsRecoverToPrimaryList(widthAvailableToRecover, new List<int> { selectedOverflowItemIndex }/*includeItems*/);

                if (!itemsToBeAdded.Contains(selectedOverflowItemIndex))
                {
                    itemsToBeAdded.Add(selectedOverflowItemIndex);
                }

                m_lastSelectedItemPendingAnimationInTopNav = itemBeingMoved;
                if (ip != null && ip.GetSize() > 0)
                {
                    foreach (var it in itemsToBeRemoved)
                    {
                        if (it == ip.GetAt(1))
                        {
                            if (m_activeIndicator is UIElement indicator)
                            {
                                // If the previously selected item is being moved into overflow, hide its indicator
                                // as we will no longer need to animate from its location.
                                AnimateSelectionChanged(null);
                            }
                            break;
                        }
                    }
                }

                if (m_topDataProvider.HasInvalidWidth(itemsToBeAdded))
                {
                    needInvalidMeasure = true;
                }
                else
                {
                    // Exchange items between Primary and Overflow
                    {
                        m_topDataProvider.MoveItemsToPrimaryList(itemsToBeAdded);
                        m_topDataProvider.MoveItemsOutOfPrimaryList(itemsToBeRemoved);
                    }

                    if (NeedRearrangeOfTopElementsAfterOverflowSelectionChanged(selectedOverflowItemIndex))
                    {
                        needInvalidMeasure = true;
                    }

                    if (!needInvalidMeasure)
                    {
                        SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
                        InvalidateMeasure();
                    }
                }
            }

            // TODO: Verify that this is no longer needed and delete
            if (needInvalidMeasure)
            {
                // not all items have known width, need to redo the layout
                m_topDataProvider.MoveAllItemsToPrimaryList();
                SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
                InvalidateTopNavPrimaryLayout();
            }
        }

        private void SelectandMoveOverflowItem(object selectedItem, IndexPath selectedIndex, bool closeFlyout)
        {
            // SelectOverflowItem is moving data in/out of overflow.
            m_selectionChangeFromOverflowMenu = true;

            try
            {
                if (closeFlyout)
                {
                    CloseTopNavigationViewFlyout();
                }

                if (!IsSelectionSuppressed(selectedItem))
                {
                    SelectOverflowItem(selectedItem, selectedIndex);
                }
            }
            finally
            {
                m_selectionChangeFromOverflowMenu = false;
            }
        }

        private void ResetAndRearrangeTopNavItems(Size availableSize)
        {
            if (HasTopNavigationViewItemNotInPrimaryList())
            {
                m_topDataProvider.MoveAllItemsToPrimaryList();
            }
            ArrangeTopNavItems(availableSize);
        }

        private void ArrangeTopNavItems(Size availableSize)
        {
            SetOverflowButtonVisibility(Visibility.Collapsed);
            double desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);
            if (!(desiredWidth < availableSize.Width))
            {
                // overflow
                SetOverflowButtonVisibility(Visibility.Visible);
                double desiredWidthForOverflowButton = MeasureTopNavigationViewDesiredWidth(c_infSize);

                // MUX_ASSERT(desiredWidthForOverflowButton >= desiredWidth);
                m_topDataProvider.OverflowButtonWidth(desiredWidthForOverflowButton - desiredWidth);

                ShrinkTopNavigationSize(desiredWidthForOverflowButton, availableSize);
            }
        }

        private void ShrinkTopNavigationSize(double desiredWidth, Size availableSize)
        {
            UpdateTopNavigationWidthCache();

            int selectedItemIndex = GetSelectedItemIndex();

            double possibleWidthForPrimaryList = MeasureTopNavMenuItemsHostDesiredWidth(c_infSize) - (desiredWidth - availableSize.Width);
            if (possibleWidthForPrimaryList >= 0)
            {
                // Remove all items which is not visible except first item and selected item.
                List<int> itemToBeRemoved = FindMovableItemsBeyondAvailableWidth(possibleWidthForPrimaryList);
                // should keep at least one item in primary
                KeepAtLeastOneItemInPrimaryList(itemToBeRemoved, true/*shouldKeepFirst*/);
                m_topDataProvider.MoveItemsOutOfPrimaryList(itemToBeRemoved);
            }

            // measure again to make sure SelectedItem is realized
            desiredWidth = MeasureTopNavigationViewDesiredWidth(c_infSize);

            double widthAtLeastToBeRemoved = desiredWidth - availableSize.Width;
            if (widthAtLeastToBeRemoved > 0)
            {
                List<int> itemToBeRemoved = FindMovableItemsToBeRemovedFromPrimaryList(widthAtLeastToBeRemoved, new List<int> { selectedItemIndex });

                // At least one item is kept on primary list
                KeepAtLeastOneItemInPrimaryList(itemToBeRemoved, false/*shouldKeepFirst*/);

                // There should be no item is virtualized in this step
                // MUX_ASSERT(!m_topDataProvider.HasInvalidWidth(itemToBeRemoved));
                m_topDataProvider.MoveItemsOutOfPrimaryList(itemToBeRemoved);
            }
        }

        private List<int> FindMovableItemsRecoverToPrimaryList(double availableWidth, List<int> includeItems)
        {
            List<int> toBeMoved = new List<int>();

            int size = m_topDataProvider.Size();

            // Included Items take high priority, all of them are included in recovery list
            foreach (int index in includeItems)
            {
                double width = m_topDataProvider.GetWidthForItem(index);
                toBeMoved.Add(index);
                availableWidth -= width;
            }

            int i = 0;
            while (i < size && availableWidth > 0)
            {
                if (!m_topDataProvider.IsItemInPrimaryList(i) && !includeItems.Contains(i))
                {
                    double width = m_topDataProvider.GetWidthForItem(i);
                    if (availableWidth >= width)
                    {
                        toBeMoved.Add(i);
                        availableWidth -= width;
                    }
                    else
                    {
                        break;
                    }
                }
                i++;
            }
            // Keep at one item is not in primary list. Two possible reason: 
            //  1, Most likely it's caused by m_topNavigationRecoveryGracePeriod
            //  2, virtualization and it doesn't have cached width
            if (i == size && toBeMoved.Count > 0)
            {
                toBeMoved.RemoveAt(toBeMoved.Count - 1);
            }
            return toBeMoved;
        }

        private List<int> FindMovableItemsToBeRemovedFromPrimaryList(double widthAtLeastToBeRemoved, List<int> excludeItems)
        {
            List<int> toBeMoved = new List<int>();

            int i = m_topDataProvider.Size() - 1;
            while (i >= 0 && widthAtLeastToBeRemoved > 0)
            {
                if (m_topDataProvider.IsItemInPrimaryList(i))
                {
                    if (!excludeItems.Contains(i))
                    {
                        double width = m_topDataProvider.GetWidthForItem(i);
                        toBeMoved.Add(i);
                        widthAtLeastToBeRemoved -= width;
                    }
                }
                i--;
            }

            return toBeMoved;
        }

        private List<int> FindMovableItemsBeyondAvailableWidth(double availableWidth)
        {
            List<int> toBeMoved = new List<int>();
            if (m_topNavRepeater is ItemsRepeater ir)
            {
                int selectedItemIndexInPrimary = m_topDataProvider.IndexOf(SelectedItem, NavigationViewSplitVectorID.PrimaryList);
                int size = m_topDataProvider.GetPrimaryListSize();

                double requiredWidth = 0;

                for (int i = 0; i < size; i++)
                {
                    if (i != selectedItemIndexInPrimary)
                    {
                        bool shouldMove = true;
                        if (requiredWidth <= availableWidth)
                        {
                            var container = ir.TryGetElement(i);
                            if (container != null)
                            {
                                if (container is UIElement containerAsUIElement)
                                {
                                    var width = containerAsUIElement.DesiredSize.Width;
                                    requiredWidth += width;
                                    shouldMove = requiredWidth > availableWidth;
                                }
                            }
                            else
                            {
                                // item is virtualized but not realized.                    
                            }
                        }

                        if (shouldMove)
                        {
                            toBeMoved.Add(i);
                        }
                    }
                }
            }

            return m_topDataProvider.ConvertPrimaryIndexToIndex(toBeMoved);
        }

        private void KeepAtLeastOneItemInPrimaryList(List<int> itemInPrimaryToBeRemoved, bool shouldKeepFirst)
        {
            if ((itemInPrimaryToBeRemoved.Count > 0) && itemInPrimaryToBeRemoved.Count == m_topDataProvider.GetPrimaryListSize())
            {
                if (shouldKeepFirst)
                {
                    itemInPrimaryToBeRemoved.RemoveAt(0);
                }
                else
                {
                    itemInPrimaryToBeRemoved.RemoveAt(itemInPrimaryToBeRemoved.Count - 1);
                }
            }
        }

        private void UpdateTopNavigationWidthCache()
        {
            int size = m_topDataProvider.GetPrimaryListSize();
            if (m_topNavRepeater is ItemsRepeater ir)
            {
                for (int i = 0; i < size; i++)
                {
                    if (ir.TryGetElement(i) is UIElement containerAsUIElement)
                    {
                        double width = containerAsUIElement.DesiredSize.Width;
                        m_topDataProvider.UpdateWidthForPrimaryItem(i, width);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private int GetSelectedItemIndex()
        {
            return m_topDataProvider.IndexOf(SelectedItem);
        }

        private double GetPaneToggleButtonWidth()
        {
            return (double)(TryFindResource("PaneToggleButtonWidth") ?? (double)c_paneToggleButtonWidth);
        }

        private double GetPaneToggleButtonHeight()
        {
            return (double)(TryFindResource("PaneToggleButtonHeight") ?? (double)c_paneToggleButtonHeight);
        }

        private bool BumperNavigation(int offset)
        {
            // By passing an offset indicating direction (ideally 1 or -1, meaning right or left respectively)
            // we'll try to move focus to an item. We won't be moving focus to items in the overflow menu and this won't
            // work on left navigation, only dealing with the top primary list here and only with items that don't have
            // !SelectsOnInvoked set to true. If !SelectsOnInvoked is true, we'll skip the item and try focusing on the next one
            // that meets the conditions, in the same direction.
            NavigationViewShoulderNavigationEnabled shoulderNavigationEnabledParamValue = ShoulderNavigationEnabled;
            bool shoulderNavigationForcedDisabled = (shoulderNavigationEnabledParamValue == NavigationViewShoulderNavigationEnabled.Never);
            bool shoulderNavigationOptionalDisabled = (shoulderNavigationEnabledParamValue == NavigationViewShoulderNavigationEnabled.WhenSelectionFollowsFocus
                && SelectionFollowsFocus == NavigationViewSelectionFollowsFocus.Disabled);

            if (!IsTopNavigationView() || shoulderNavigationOptionalDisabled || shoulderNavigationForcedDisabled)
            {
                return false;
            }

            bool shoulderNavigationSelectionFollowsFocusEnabled = (SelectionFollowsFocus == NavigationViewSelectionFollowsFocus.Enabled
                && shoulderNavigationEnabledParamValue == NavigationViewShoulderNavigationEnabled.WhenSelectionFollowsFocus);

            bool shoulderNavigationEnabled = (shoulderNavigationSelectionFollowsFocusEnabled
                || shoulderNavigationEnabledParamValue == NavigationViewShoulderNavigationEnabled.Always);

            if (!shoulderNavigationEnabled)
            {
                return false;
            }

            object item = SelectedItem;

            if (item != null)
            {
                if (NavigationViewItemOrSettingsContentFromData(item) is NavigationViewItem nvi)
                {
                    IndexPath indexPath = GetIndexPathForContainer(nvi);
                    bool isInFooter = indexPath.GetAt(0) == c_footerMenuBlockIndex;

                    int indexInMainList = isInFooter ? -1 : indexPath.GetAt(1);
                    int indexInFooter = isInFooter ? indexPath.GetAt(1) : -1;

                    ItemsRepeater topNavRepeater = m_topNavRepeater;
                    int topPrimaryListSize = m_topDataProvider.GetPrimaryListSize();

                    ItemsRepeater footerRepeater = m_topNavFooterMenuRepeater;
                    int footerItemsSize = FooterMenuItems.Count;

                    if (IsSettingsVisible)
                    {
                        footerItemsSize++;
                    }

                    if (indexInMainList >= 0)
                    {

                        if (SelectSelectableItemWithOffset(indexInMainList, offset, topNavRepeater, topPrimaryListSize))
                        {
                            return true;
                        }

                        // No sutable item found in main list so try to select item in footer
                        if (offset > 0)
                        {
                            return SelectSelectableItemWithOffset(-1, offset, footerRepeater, footerItemsSize);
                        }

                        return false;
                    }

                    if (indexInFooter >= 0)
                    {

                        if (SelectSelectableItemWithOffset(indexInFooter, offset, footerRepeater, footerItemsSize))
                        {
                            return true;
                        }

                        // No sutable item found in footer so try to select item in main list
                        if (offset < 0)
                        {
                            return SelectSelectableItemWithOffset(topPrimaryListSize, offset, topNavRepeater, topPrimaryListSize);
                        }
                    }
                }
            }

            return false;
        }

        private bool SelectSelectableItemWithOffset(int startIndex, int offset, ItemsRepeater repeater, int repeaterCollectionSize)
        {
            startIndex += offset;
            while (startIndex > -1 && startIndex < repeaterCollectionSize)
            {
                UIElement newItem = repeater.TryGetElement(startIndex);
                if (newItem is NavigationViewItem newNavViewItem)
                {
                    // This is done to skip Separators or other items that are not NavigationViewItems
                    if (newNavViewItem.SelectsOnInvoked)
                    {
                        newNavViewItem.IsSelected = true;
                        return true;
                    }
                }

                startIndex += offset;
            }
            return false;
        }

        private bool IsTopNavigationView()
        {
            return PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
        }

        private bool IsTopPrimaryListVisible()
        {
            return m_topNavRepeater != null && (TemplateSettings.TopPaneVisibility == Visibility.Visible);
        }

        /* *** */

        private void CreateAndHookEventsToSettings()
        {
            if (m_settingsItem == null)
            {
                return;
            }

            NavigationViewItem settingsItem = m_settingsItem;
            SymbolIcon settingsIcon = new SymbolIcon(Symbol.Setting);
            settingsItem.Icon = settingsIcon;

            // Do localization for settings item label and Automation Name
            string localizedSettingsName = Properties.Resources.Strings.Resources.SettingsButtonName;
            AutomationProperties.SetName(settingsItem, localizedSettingsName);
            settingsItem.Tag = localizedSettingsName;
            UpdateSettingsItemToolTip();

            // Add the name only in case of horizontal nav
            if (!IsTopNavigationView())
            {
                settingsItem.Content = localizedSettingsName;
            }
            else
            {
                settingsItem.Content = null;
            }

            // hook up SettingsItem
            SetValue(SettingsItemPropertyKey, settingsItem);
        }

        private void OnIsPaneOpenChanged()
        {
            if (IsPaneOpen && m_wasForceClosed)
            {
                m_wasForceClosed = false; // remove the pane open flag since Pane is opened.
            }
            else if (!m_isOpenPaneForInteraction && !IsPaneOpen)
            {
                if (m_rootSplitView != null)
                {
                    // splitview.IsPaneOpen and nav.IsPaneOpen is two way binding. If nav.IsPaneOpen=false and splitView.IsPaneOpen=true,
                    // then the pane has been closed by API and we treat it as a forced close.
                    // If, however, splitView.IsPaneOpen=false, then nav.IsPaneOpen is just following the SplitView here and the pane
                    // was closed, for example, due to app window resizing. We don't set the force flag in this situation.
                    m_wasForceClosed = m_rootSplitView.IsPaneOpen;
                }
                else
                {
                    // If there is no SplitView (for example it hasn't been loaded yet) then nav.IsPaneOpen was set directly
                    // so we treat it as a closed force.
                    m_wasForceClosed = true;
                }
            }

            SetPaneToggleButtonAutomationName();
            UpdatePaneTabFocusNavigation();
            UpdateSettingsItemToolTip();
            UpdatePaneTitleFrameworkElementParents();

            UpdatePaneButtonsWidths();
        }

        private void UpdatePaneButtonsWidths()
        {
            double newButtonWidths = CompactPaneLength;

            if (DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                newButtonWidths = c_paneToggleButtonWidth;
            }

            if (m_backButton != null)
            {
                m_backButton.Width = newButtonWidths;
            }

            if (m_paneToggleButton != null)
            {
                m_paneToggleButton.MinWidth = newButtonWidths;
                if (m_paneToggleButton.Template.FindName(c_paneToggleButtonIconGridColumnName, m_paneToggleButton) is ColumnDefinition iconGridColumnElement)
                {
                    if (iconGridColumnElement is ColumnDefinition paneToggleButtonIconColumn)
                    {
                        paneToggleButtonIconColumn.Width = new GridLength(newButtonWidths);
                    }
                }
            }
        }

        private void UpdateHeaderVisibility()
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            UpdateHeaderVisibility(DisplayMode);
        }

        private void UpdateHeaderVisibility(NavigationViewDisplayMode displayMode)
        {
            // Ignore AlwaysShowHeader property in case DisplayMode is Minimal and it's not Top NavigationView
            bool showHeader = AlwaysShowHeader || (!IsTopNavigationView() && displayMode == NavigationViewDisplayMode.Minimal);

            // Like bug 17517627, Customer like WallPaper Studio 10 expects a HeaderContent visual even if Header() is null. 
            // App crashes when they have dependency on that visual, but the crash is not directly state that it's a header problem.   
            // NavigationView doesn't use quirk, but we determine the version by themeresource.
            // As a workaround, we 'quirk' it for RS4 or before release. if it's RS4 or before, HeaderVisible is not related to Header().
            // If theme resource is RS5 or later, we will not show header if header is null.
            showHeader = Header != null && showHeader;

            VisualStateManager.GoToState(this, showHeader ? "HeaderVisible" : "HeaderCollapsed", false /*useTransitions*/);
        }

        private void UpdatePaneToggleButtonVisibility()
        {
            bool visible = IsPaneToggleButtonVisible && !IsTopNavigationView();
            GetTemplateSettings().PaneToggleButtonVisibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePaneDisplayMode()
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            if (!IsTopNavigationView())
            {
                UpdateAdaptiveLayout(ActualWidth, true /*forceSetDisplayMode*/);

                SwapPaneHeaderContent(m_leftNavPaneHeaderContentBorder, m_paneHeaderOnTopPane, "PaneHeader");
                SwapPaneHeaderContent(m_leftNavPaneCustomContentBorder, m_paneCustomContentOnTopPane, "PaneCustomContent");
                SwapPaneHeaderContent(m_leftNavFooterContentBorder, m_paneFooterOnTopPane, "PaneFooter");

                CreateAndHookEventsToSettings();
            }
            else
            {
                ClosePane();
                SetDisplayMode(NavigationViewDisplayMode.Minimal, true);

                SwapPaneHeaderContent(m_paneHeaderOnTopPane, m_leftNavPaneHeaderContentBorder, "PaneHeader");
                SwapPaneHeaderContent(m_paneCustomContentOnTopPane, m_leftNavPaneCustomContentBorder, "PaneCustomContent");
                SwapPaneHeaderContent(m_paneFooterOnTopPane, m_leftNavFooterContentBorder, "PaneFooter");

                CreateAndHookEventsToSettings();
            }

            UpdateContentBindingsForPaneDisplayMode();
            UpdateRepeaterItemsSource(false /*forceSelectionModelUpdate*/);
            UpdateFooterRepeaterItemsSource(false /*sourceCollectionReset*/, false /*sourceCollectionChanged*/);

            if (SelectedItem != null)
            {
                m_OrientationChangedPendingAnimation = true;
            }
        }

        private void UpdatePaneDisplayMode(NavigationViewPaneDisplayMode oldDisplayMode, NavigationViewPaneDisplayMode newDisplayMode)
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            UpdatePaneDisplayMode();

            // For better user experience, We help customer to Open/Close Pane automatically when we switch between LeftMinimal <-> Left.
            // From other navigation PaneDisplayMode to LeftMinimal, we expect pane is closed.
            // From LeftMinimal to Left, it is expected the pane is open. For other configurations, this seems counterintuitive.
            // See #1702 and #1787
            if (!IsTopNavigationView())
            {
                if (IsPaneOpen)
                {
                    if (newDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
                    {
                        ClosePane();
                    }
                }
                else
                {
                    if (oldDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal
                        && newDisplayMode == NavigationViewPaneDisplayMode.Left)
                    {
                        OpenPane();
                    }
                }
            }
        }

        private void UpdatePaneVisibility()
        {
            if (IsPaneVisible)
            {
                if (IsTopNavigationView())
                {
                    TemplateSettings.LeftPaneVisibility = Visibility.Collapsed;
                    TemplateSettings.TopPaneVisibility = Visibility.Visible;
                }
                else
                {
                    TemplateSettings.TopPaneVisibility = Visibility.Collapsed;
                    TemplateSettings.LeftPaneVisibility = Visibility.Visible;
                }

                VisualStateManager.GoToState(this, "PaneVisible", false /*useTransitions*/);
            }
            else
            {
                TemplateSettings.TopPaneVisibility = Visibility.Collapsed;
                TemplateSettings.LeftPaneVisibility = Visibility.Collapsed;

                VisualStateManager.GoToState(this, "PaneCollapsed", false /*useTransitions*/);
            }
        }

        private void UpdateContentBindingsForPaneDisplayMode()
        {
            UIElement autoSuggestBoxContentControl = null;
            UIElement notControl = null;

            if (!IsTopNavigationView())
            {
                autoSuggestBoxContentControl = m_leftNavPaneAutoSuggestBoxPresenter;
                notControl = m_topNavPaneAutoSuggestBoxPresenter;
            }
            else
            {
                autoSuggestBoxContentControl = m_topNavPaneAutoSuggestBoxPresenter;
                notControl = m_leftNavPaneAutoSuggestBoxPresenter;
            }

            if (autoSuggestBoxContentControl != null)
            {
                if (notControl != null)
                {
                    notControl.ClearValue(ContentControl.ContentProperty);
                }

                //SharedHelpers.SetBinding("AutoSuggestBox", autoSuggestBoxContentControl, ContentControl.ContentProperty);

                BindingOperations.SetBinding(autoSuggestBoxContentControl, ContentControl.ContentProperty, new Binding("AutoSuggestBox") { RelativeSource = RelativeSource.TemplatedParent });
            }
        }

        private void UpdatePaneTabFocusNavigation()
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            KeyboardNavigationMode mode = KeyboardNavigationMode.Local;

            if (m_rootSplitView != null)
            {
                // If the pane is open in an overlay (light-dismiss) mode, trap keyboard focus inside the pane
                if (IsPaneOpen && (m_rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay || m_rootSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay))
                {
                    mode = KeyboardNavigationMode.Cycle;
                }
            }

            if (m_paneContentGrid != null)
            {
                KeyboardNavigation.SetTabNavigation(m_paneContentGrid, mode);
            }
        }

        private void UpdatePaneToggleSize()
        {
            if (!ShouldPreserveNavigationViewRS3Behavior())
            {
                if (m_rootSplitView != null)
                {
                    double width = GetPaneToggleButtonWidth();
                    double togglePaneButtonWidth = width;

                    if (ShouldShowBackButton() && m_rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
                    {
                        double backButtonWidth = c_backButtonWidth;
                        if (m_backButton is Button backButton)
                        {
                            backButtonWidth = backButton.Width;
                        }

                        width += backButtonWidth;
                    }

                    if (!m_isClosedCompact && PaneTitle?.Length > 0)
                    {
                        if (m_rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay && IsPaneOpen)
                        {
                            width = OpenPaneLength;
                            togglePaneButtonWidth = OpenPaneLength - ((ShouldShowBackButton() || ShouldShowCloseButton()) ? c_backButtonWidth : 0);
                        }
                        else if (!(m_rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay && !IsPaneOpen))
                        {
                            width = OpenPaneLength;
                            togglePaneButtonWidth = OpenPaneLength;
                        }
                    }

                    if (m_paneToggleButton is Button toggleButton)
                    {
                        toggleButton.Width = togglePaneButtonWidth;
                    }
                }
            }
        }

        private void UpdateBackAndCloseButtonsVisibility()
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            bool shouldShowBackButton = ShouldShowBackButton();
            Visibility backButtonVisibility = shouldShowBackButton ? Visibility.Visible : Visibility.Collapsed;
            NavigationViewVisualStateDisplayMode visualStateDisplayMode = GetVisualStateDisplayMode(DisplayMode);
            bool useLeftPaddingForBackOrCloseButton =
                (visualStateDisplayMode == NavigationViewVisualStateDisplayMode.Minimal && !IsTopNavigationView()) ||
                visualStateDisplayMode == NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
            double leftPaddingForBackOrCloseButton = 0.0;
            double paneHeaderPaddingForToggleButton = 0.0;
            double paneHeaderPaddingForCloseButton = 0.0;
            double paneHeaderContentBorderRowMinHeight = 0.0;

            GetTemplateSettings().BackButtonVisibility = backButtonVisibility;

            if (m_paneToggleButton != null && IsPaneToggleButtonVisible)
            {
                paneHeaderContentBorderRowMinHeight = GetPaneToggleButtonHeight();
                paneHeaderPaddingForToggleButton = GetPaneToggleButtonWidth();

                if (useLeftPaddingForBackOrCloseButton)
                {
                    leftPaddingForBackOrCloseButton = paneHeaderPaddingForToggleButton;
                }
            }

            if (m_backButton != null)
            {
                if (ShouldPreserveNavigationViewRS4Behavior())
                {
                    m_backButton.Visibility = backButtonVisibility;
                }

                if (useLeftPaddingForBackOrCloseButton && backButtonVisibility == Visibility.Visible)
                {
                    leftPaddingForBackOrCloseButton += m_backButton.Width;
                }
            }

            if (m_closeButton != null)
            {
                Visibility closeButtonVisibility = ShouldShowCloseButton() ? Visibility.Visible : Visibility.Collapsed;

                m_closeButton.Visibility = closeButtonVisibility;

                if (closeButtonVisibility == Visibility.Visible)
                {
                    paneHeaderContentBorderRowMinHeight = Math.Max(paneHeaderContentBorderRowMinHeight, m_closeButton.Height);

                    if (useLeftPaddingForBackOrCloseButton)
                    {
                        paneHeaderPaddingForCloseButton = m_closeButton.Width;
                        leftPaddingForBackOrCloseButton += paneHeaderPaddingForCloseButton;
                    }
                }
            }

            if (m_contentLeftPadding != null)
            {
                m_contentLeftPadding.Width = leftPaddingForBackOrCloseButton;
            }

            if (m_paneHeaderToggleButtonColumn != null)
            {
                // Account for the PaneToggleButton's width in the PaneHeader's placement.
                m_paneHeaderToggleButtonColumn.Width = new GridLength(paneHeaderPaddingForToggleButton, GridUnitType.Pixel);
            }

            if (m_paneHeaderCloseButtonColumn != null)
            {
                // Account for the CloseButton's width in the PaneHeader's placement.
                m_paneHeaderCloseButtonColumn.Width = new GridLength(paneHeaderPaddingForCloseButton, GridUnitType.Pixel);
            }

            if (m_paneTitleHolderFrameworkElement != null)
            {
                if (paneHeaderContentBorderRowMinHeight == 0.00 && m_paneTitleHolderFrameworkElement.Visibility == Visibility.Visible)
                {
                    // Handling the case where the PaneTottleButton is collapsed and the PaneTitle's height needs to push the rest of the NavigationView's UI down.
                    paneHeaderContentBorderRowMinHeight = m_paneTitleHolderFrameworkElement.ActualHeight;
                }
            }

            if (m_paneHeaderContentBorderRow != null)
            {
                m_paneHeaderContentBorderRow.MinHeight = paneHeaderContentBorderRowMinHeight;
            }

            if (m_paneContentGrid is Grid paneContentGrid)
            {
                RowDefinitionCollection rowDefs = paneContentGrid.RowDefinitions;

                if (rowDefs.Count >= c_backButtonRowDefinition)
                {
                    RowDefinition rowDef = rowDefs[c_backButtonRowDefinition];

                    int backButtonRowHeight = 0;
                    if (!IsOverlay() && shouldShowBackButton)
                    {
                        backButtonRowHeight = c_backButtonHeight;
                    }
                    else if (ShouldPreserveNavigationViewRS3Behavior())
                    {
                        // This row represented the height of the hamburger+margin in RS3 and prior
                        backButtonRowHeight = c_toggleButtonHeightWhenShouldPreserveNavigationViewRS3Behavior;
                    }

                    GridLength length = new GridLength(backButtonRowHeight);
                    rowDef.Height = length;
                }
            }

            if (!ShouldPreserveNavigationViewRS4Behavior())
            {
                VisualStateManager.GoToState(this, shouldShowBackButton ? "BackButtonVisible" : "BackButtonCollapsed", false /*useTransitions*/);
            }

            UpdateTitleBarPadding();
        }

        private void UpdatePaneTitleMargins()
        {
            if (ShouldPreserveNavigationViewRS4Behavior() && m_paneTitleFrameworkElement != null)
            {
                double width = GetPaneToggleButtonWidth();

                if (ShouldShowBackButton() && IsOverlay())
                {
                    width += c_backButtonWidth;
                }

                m_paneTitleFrameworkElement.Margin = new Thickness(width, 0, 0, 0); // see "Hamburger title" on uni
            }
        }

        private void UpdateLeftRepeaterItemSource(object items)
        {
            UpdateItemsRepeaterItemsSource(m_leftNavRepeater, items);
            // Left pane repeater has a new items source, update pane layout.
            UpdatePaneLayout();
        }

        private void UpdateTopNavRepeatersItemSource(object items)
        {
            // Change data source and setup vectors
            m_topDataProvider.SetDataSource(items);

            // rebinding
            UpdateTopNavPrimaryRepeaterItemsSource(items);
            UpdateTopNavOverflowRepeaterItemsSource(items);
        }

        private void UpdateTopNavPrimaryRepeaterItemsSource(object items)
        {
            if (items != null)
            {
                UpdateItemsRepeaterItemsSource(m_topNavRepeater, m_topDataProvider.GetPrimaryItems());
            }
            else
            {
                UpdateItemsRepeaterItemsSource(m_topNavRepeater, null);
            }
        }

        private void UpdateTopNavOverflowRepeaterItemsSource(object items)
        {
            //m_topNavOverflowItemsCollectionChangedRevoker.revoke();

            if (m_topNavRepeaterOverflowView is ItemsRepeater overflowRepeater)
            {
                if (overflowRepeater.ItemsSource != null)
                {
                    overflowRepeater.ItemsSourceView.CollectionChanged -= OnOverflowItemsSourceCollectionChanged;
                }

                if (items != null)
                {
                    IList itemsSource = m_topDataProvider.GetOverflowItems();
                    overflowRepeater.ItemsSource = itemsSource;

                    // We listen to changes to the overflow menu item collection so we can set the visibility of the overflow button
                    // to collapsed when it no longer has any items.
                    //
                    // Normally, MeasureOverride() kicks off updating the button's visibility, however, it is not run when the overflow menu
                    // only contains a *single* item and we
                    // - either remove that menu item or
                    // - remove menu items displayed in the NavigationView pane until there is enough room for the single overflow menu item
                    //   to be displayed in the pane
                    overflowRepeater.ItemsSourceView.CollectionChanged += OnOverflowItemsSourceCollectionChanged; // (winrt::auto_revoke, { this, &NavigationView::OnOverflowItemsSourceCollectionChanged });
                }
                else
                {
                    overflowRepeater.ItemsSource = null;
                }
            }
        }

        private static void UpdateItemsRepeaterItemsSource(ItemsRepeater ir, object itemsSource)
        {
            if (ir != null)
            {
                ir.ItemsSource = itemsSource;
            }
        }

        private void UpdateSelectionForMenuItems()
        {
            // Allow customer to set selection by NavigationViewItem.IsSelected.
            // If there are more than two items are set IsSelected=true, the first one is actually selected.
            // If SelectedItem is set, IsSelected is ignored.
            //         <NavigationView.MenuItems>
            //              <NavigationViewItem Content = "Collection" IsSelected = "True" / >
            //         </NavigationView.MenuItems>
            if (SelectedItem == null)
            {
                bool foundFirstSelected = false;

                // firstly check Menu items
                if (MenuItems is IList menuItems)
                {
                    foundFirstSelected = UpdateSelectedItemFromMenuItems(menuItems);
                }

                // then do same for footer items and tell wenever selected item alreadyfound in MenuItems
                if (FooterMenuItems is IList footerItems)
                {
                    UpdateSelectedItemFromMenuItems(footerItems, foundFirstSelected);
                }
            }
        }

        private bool UpdateSelectedItemFromMenuItems(IList menuItems, bool foundFirstSelected = false)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i] is NavigationViewItem item)
                {
                    if (item.IsSelected)
                    {
                        if (!foundFirstSelected)
                        {
                            try
                            {
                                m_shouldIgnoreNextSelectionChange = true;
                                SelectedItem = item;
                                foundFirstSelected = true;
                            }
                            finally
                            {
                                m_shouldIgnoreNextSelectionChange = false;
                            }
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                    }
                }
            }
            return foundFirstSelected;
        }

        private void UpdateRepeaterItemsSource(bool forceSelectionModelUpdate)
        {
            object itemsSource;
            if (MenuItemsSource is object menuItemsSource)
            {
                itemsSource = menuItemsSource;
            }
            else
            {
                UpdateSelectionForMenuItems();
                itemsSource = MenuItems;
            }

            // Selection Model has same representation of data regardless
            // of pane mode, so only update if the ItemsSource data itself
            // has changed.
            if (forceSelectionModelUpdate)
            {
                m_selectionModelSource[0] = itemsSource;
            }

            if (m_menuItemsSource != null)
                m_menuItemsSource.CollectionChanged -= OnMenuItemsSourceCollectionChanged;

            m_menuItemsSource = new InspectingDataSource(itemsSource);
            m_menuItemsSource.CollectionChanged += OnMenuItemsSourceCollectionChanged;

            if (IsTopNavigationView())
            {
                UpdateLeftRepeaterItemSource(null);
                UpdateTopNavRepeatersItemSource(itemsSource);
                InvalidateTopNavPrimaryLayout();
            }
            else
            {
                UpdateTopNavRepeatersItemSource(null);
                UpdateLeftRepeaterItemSource(itemsSource);
            }
        }

        private void UpdateFooterRepeaterItemsSource(bool sourceCollectionReseted, bool sourceCollectionChanged)
        {
            if (!m_appliedTemplate) return;

            object itemsSource;
            if (FooterMenuItemsSource is object menuItemsSource)
            {
                itemsSource = menuItemsSource;
            }
            else
            {
                UpdateSelectionForMenuItems();
                itemsSource = FooterMenuItems;
            }

            UpdateItemsRepeaterItemsSource(m_leftNavFooterMenuRepeater, null);
            UpdateItemsRepeaterItemsSource(m_topNavFooterMenuRepeater, null);

            if (m_settingsItem == null || sourceCollectionChanged || sourceCollectionReseted)
            {
                var dataSource = new List<object>();

                if (m_settingsItem == null)
                {
                    m_settingsItem = new NavigationViewItem();
                    NavigationViewItem settingsItem = m_settingsItem;
                    settingsItem.Name = "SettingsItem";
                    m_navigationViewItemsFactory.SettingsItem(settingsItem);
                }

                if (sourceCollectionReseted)
                {
                    m_footerItemsSource.CollectionChanged -= OnFooterItemsSourceCollectionChanged;
                    m_footerItemsSource = null;
                }

                if (m_footerItemsSource == null)
                {
                    m_footerItemsSource = new InspectingDataSource(itemsSource);
                    m_footerItemsSource.CollectionChanged += OnFooterItemsSourceCollectionChanged;
                }

                if (m_footerItemsSource != null)
                {
                    NavigationViewItem settingsItem = m_settingsItem;
                    int size = m_footerItemsSource.Count;

                    for (int i = 0; i < size; i++)
                    {
                        object item = m_footerItemsSource.GetAt(i);
                        dataSource.Add(item);
                    }

                    if (IsSettingsVisible)
                    {
                        CreateAndHookEventsToSettings();
                        // add settings item to the end of footer
                        dataSource.Add(settingsItem);
                    }
                }

                m_selectionModelSource[1] = dataSource;
            }

            if (IsTopNavigationView())
            {
                UpdateItemsRepeaterItemsSource(m_topNavFooterMenuRepeater, m_selectionModelSource[1]);
            }
            else
            {
                if (m_leftNavFooterMenuRepeater is ItemsRepeater repeater)
                {
                    UpdateItemsRepeaterItemsSource(m_leftNavFooterMenuRepeater, m_selectionModelSource[1]);

                    // Footer items changed and we need to recalculate the layout.
                    // However repeater "lags" behind, so we need to force it to reevaluate itself now.
                    repeater.InvalidateMeasure();
                    repeater.UpdateLayout();

                    // Footer items changed, so let's update the pane layout.
                    UpdatePaneLayout();
                }

                if (m_settingsItem is NavigationViewItem settings)
                {
                    settings.BringIntoView();
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            double width = args.NewSize.Width;
            UpdateAdaptiveLayout(width);
            UpdateTitleBarPadding();
            UpdateBackAndCloseButtonsVisibility();
            UpdatePaneLayout();
        }

        private void OnItemsContainerSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdatePaneLayout();
        }

        private void OnLayoutUpdated(object sender, object e)
        {
            LayoutUpdated -= OnLayoutUpdated;
            m_layoutUpdatedToken = false;

            // In topnav, when an item in overflow menu is clicked, the animation is delayed because that item is not move to primary list yet.
            // And it depends on LayoutUpdated to re-play the animation. m_lastSelectedItemPendingAnimationInTopNav is the last selected overflow item.
            if (m_lastSelectedItemPendingAnimationInTopNav is object lastSelectedItemInTopNav)
            {
                m_lastSelectedItemPendingAnimationInTopNav = null;
                AnimateSelectionChanged(lastSelectedItemInTopNav);
            }

            if (m_OrientationChangedPendingAnimation)
            {
                m_OrientationChangedPendingAnimation = false;
                AnimateSelectionChanged(SelectedItem);
            }
        }

        private void UpdateAdaptiveLayout(double width, bool forceSetDisplayMode = false)
        {
            // In top nav, there is no adaptive pane layout
            if (IsTopNavigationView())
            {
                return;
            }

            if (m_rootSplitView == null)
            {
                return;
            }

            // If we decide we want it to animate open/closed when you resize the
            // window we'll have to change how we figure out the initial state
            // instead of this:
            m_initialListSizeStateSet = false; // see UpdateIsClosedCompact()

            NavigationViewDisplayMode displayMode = NavigationViewDisplayMode.Compact;

            NavigationViewPaneDisplayMode paneDisplayMode = PaneDisplayMode;
            if (paneDisplayMode == NavigationViewPaneDisplayMode.Auto)
            {
                if (width >= ExpandedModeThresholdWidth)
                {
                    displayMode = NavigationViewDisplayMode.Expanded;
                }
                else if (width < CompactModeThresholdWidth)
                {
                    displayMode = NavigationViewDisplayMode.Minimal;
                }
            }
            else if (paneDisplayMode == NavigationViewPaneDisplayMode.Left)
            {
                displayMode = NavigationViewDisplayMode.Expanded;
            }
            else if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact)
            {
                displayMode = NavigationViewDisplayMode.Compact;
            }
            else if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
            {
                displayMode = NavigationViewDisplayMode.Minimal;
            }
            else
            {
                // MUX_FAIL_FAST();
            }

            if (!forceSetDisplayMode && m_InitialNonForcedModeUpdate)
            {
                if (displayMode == NavigationViewDisplayMode.Minimal ||
                    displayMode == NavigationViewDisplayMode.Compact)
                {
                    ClosePane();
                }
                m_InitialNonForcedModeUpdate = false;
            }

            NavigationViewDisplayMode previousMode = DisplayMode;
            SetDisplayMode(displayMode, forceSetDisplayMode);

            if (displayMode == NavigationViewDisplayMode.Expanded && IsPaneVisible)
            {
                if (!m_wasForceClosed)
                {
                    OpenPane();
                }
            }

            if (previousMode == NavigationViewDisplayMode.Expanded
                && displayMode == NavigationViewDisplayMode.Compact)
            {
                m_initialListSizeStateSet = false;
                ClosePane();
            }
        }

        private void UpdatePaneLayout()
        {
            if (!IsTopNavigationView())
            {
                double totalAvailableHeight = 0;
                if (m_itemsContainerRow is RowDefinition paneContentRow)
                {
                    // 20px is the padding between the two item lists
                    if (m_leftNavFooterContentBorder is ContentControl paneFooter)
                    {
                        totalAvailableHeight = paneContentRow.ActualHeight - 29 - paneFooter.ActualHeight;
                    }
                    else
                    {
                        totalAvailableHeight = paneContentRow.ActualHeight - 29;
                    }
                }

                // Only continue if we have a positive amount of space to manage.
                if (totalAvailableHeight > 0)
                {
                    // We need this value more than twice, so cache it.
                    var totalAvailableHeightHalf = totalAvailableHeight / 2;

                    double heightForMenuItems = 0;
                    if (m_footerItemsScrollViewer is FrameworkElement footerItemsScrollViewer)
                    {
                        if (m_leftNavFooterMenuRepeater is ItemsRepeater footerItemsRepeater)
                        {
                            // We know the actual height of footer items, so use that to determine how to split pane.
                            if (m_leftNavRepeater is ItemsRepeater menuItems)
                            {
                                var footersActualHeight = footerItemsRepeater.ActualHeight;
                                var menuItemsActualHeight = menuItems.ActualHeight;
                                if (totalAvailableHeight > menuItemsActualHeight + footersActualHeight)
                                {
                                    // We have enough space for two so let everyone get as much as they need.
                                    footerItemsScrollViewer.MaxHeight = footersActualHeight;
                                    if (m_visualItemsSeparator is FrameworkElement separator)
                                    {
                                        separator.Visibility = Visibility.Collapsed;
                                    }
                                    heightForMenuItems = totalAvailableHeight - footersActualHeight;
                                }
                                else if (menuItemsActualHeight <= totalAvailableHeightHalf)
                                {
                                    // Footer items exceed over the half, so let's limit them.
                                    footerItemsScrollViewer.MaxHeight = totalAvailableHeight - menuItemsActualHeight;
                                    if (m_visualItemsSeparator is FrameworkElement separator)
                                    {
                                        separator.Visibility = Visibility.Visible;
                                    }
                                    heightForMenuItems = menuItemsActualHeight;
                                }
                                else if (footersActualHeight <= totalAvailableHeightHalf)
                                {
                                    // Menu items exceed over the half, so let's limit them.
                                    footerItemsScrollViewer.MaxHeight = footersActualHeight;
                                    if (m_visualItemsSeparator is FrameworkElement separator)
                                    {
                                        separator.Visibility = Visibility.Visible;
                                    }
                                    heightForMenuItems = totalAvailableHeight - footersActualHeight;
                                }
                                else
                                {
                                    // Both are more than half the height, so split evenly.
                                    footerItemsScrollViewer.MaxHeight = totalAvailableHeightHalf;
                                    if (m_visualItemsSeparator is FrameworkElement separator)
                                    {
                                        separator.Visibility = Visibility.Visible;
                                    }
                                    heightForMenuItems = totalAvailableHeightHalf;
                                }
                            }
                            else
                            {
                                // Couldn't determine the menuItems.
                                // Let's just take all the height and let the other repeater deal with it.
                                heightForMenuItems = totalAvailableHeight - footerItemsRepeater.ActualHeight;
                            }
                        }
                        else
                        {
                            // We have no idea how much space to occupy as we are not able to get the size of the footer repeater.
                            // Stick with 50% as backup.
                            footerItemsScrollViewer.MaxHeight = totalAvailableHeightHalf;
                        }
                    }
                    else
                    {
                        // We couldn't find a good strategy, so limit to 50% percent for the menu items.
                        heightForMenuItems = totalAvailableHeightHalf;
                    }

                    // Footer items should have precedence as that usually contains very
                    // important items such as settings or the profile.

                    if (m_menuItemsScrollViewer is FrameworkElement menuItemsScrollViewer)
                    {
                        // Update max height for menu items.
                        menuItemsScrollViewer.MaxHeight = heightForMenuItems;
                    }
                }
            }
        }

        private void SetDisplayMode(NavigationViewDisplayMode displayMode, bool forceSetDisplayMode = false)
        {
            // Need to keep the VisualStateGroup "DisplayModeGroup" updated even if the actual
            // display mode is not changed. This is due to the fact that there can be a transition between
            // 'Minimal' and 'MinimalWithBackButton'.
            UpdateVisualStateForDisplayModeGroup(displayMode);

            if (forceSetDisplayMode || DisplayMode != displayMode)
            {
                // Update header visibility based on what the new display mode will be
                UpdateHeaderVisibility(displayMode);

                UpdatePaneTabFocusNavigation();

                UpdatePaneToggleSize();

                RaiseDisplayModeChanged(displayMode);
            }
        }

        // To support TopNavigationView, DisplayModeGroup in visualstate(We call it VisualStateDisplayMode) is decoupled with DisplayMode.
        // The VisualStateDisplayMode is the combination of TopNavigationView, DisplayMode, PaneDisplayMode.
        // Here is the mapping:
        //    TopNav -> Minimal
        //    PaneDisplayMode::Left || (PaneDisplayMode::Auto && DisplayMode::Expanded) -> Expanded
        //    PaneDisplayMode::LeftCompact || (PaneDisplayMode::Auto && DisplayMode::Compact) -> Compact
        //    Map others to Minimal or MinimalWithBackButton 
        private NavigationViewVisualStateDisplayMode GetVisualStateDisplayMode(NavigationViewDisplayMode displayMode)
        {
            NavigationViewPaneDisplayMode paneDisplayMode = PaneDisplayMode;

            if (IsTopNavigationView())
            {
                return NavigationViewVisualStateDisplayMode.Minimal;
            }

            if (paneDisplayMode == NavigationViewPaneDisplayMode.Left ||
                (paneDisplayMode == NavigationViewPaneDisplayMode.Auto && displayMode == NavigationViewDisplayMode.Expanded))
            {
                return NavigationViewVisualStateDisplayMode.Expanded;
            }

            if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact ||
                (paneDisplayMode == NavigationViewPaneDisplayMode.Auto && displayMode == NavigationViewDisplayMode.Compact))
            {
                return NavigationViewVisualStateDisplayMode.Compact;
            }

            // In minimal mode, when the NavView is closed, the HeaderContent doesn't have
            // its own dedicated space, and must 'share' the top of the NavView with the 
            // pane toggle button ('hamburger' button) and the back button.
            // When the NavView is open, the close button is taking space instead of the back button.
            if (ShouldShowBackButton() || ShouldShowCloseButton())
            {
                return NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
            }
            else
            {
                return NavigationViewVisualStateDisplayMode.Minimal;
            }
        }

        private void UpdateVisualStateForDisplayModeGroup(NavigationViewDisplayMode displayMode)
        {
            if (m_rootSplitView is SplitView splitView)
            {
                NavigationViewVisualStateDisplayMode visualStateDisplayMode = GetVisualStateDisplayMode(displayMode);
                string visualStateName = "";
                SplitViewDisplayMode splitViewDisplayMode = SplitViewDisplayMode.Overlay;
                string visualStateNameMinimal = "Minimal";

                switch (visualStateDisplayMode)
                {
                    case NavigationViewVisualStateDisplayMode.MinimalWithBackButton:
                        visualStateName = "MinimalWithBackButton";
                        splitViewDisplayMode = SplitViewDisplayMode.Overlay;
                        break;
                    case NavigationViewVisualStateDisplayMode.Minimal:
                        visualStateName = visualStateNameMinimal;
                        splitViewDisplayMode = SplitViewDisplayMode.Overlay;
                        break;
                    case NavigationViewVisualStateDisplayMode.Compact:
                        visualStateName = "Compact";
                        splitViewDisplayMode = SplitViewDisplayMode.CompactOverlay;
                        break;
                    case NavigationViewVisualStateDisplayMode.Expanded:
                        visualStateName = "Expanded";
                        splitViewDisplayMode = SplitViewDisplayMode.CompactInline;
                        break;
                }

                // When the pane is made invisible we need to collapse the pane part of the SplitView
                if (!IsPaneVisible)
                {
                    splitViewDisplayMode = SplitViewDisplayMode.CompactOverlay;
                }

                bool handled = false;
                if (visualStateName == visualStateNameMinimal && IsTopNavigationView())
                {
                    // TopNavigationMinimal was introduced in 19H1. We need to fallback to Minimal if the customer uses an older template.
                    handled = VisualStateManager.GoToState(this, "TopNavigationMinimal", false /*useTransitions*/);
                }
                if (!handled)
                {
                    VisualStateManager.GoToState(this, visualStateName, false /*useTransitions*/);
                }

                // Updating the splitview 'DisplayMode' property in some diplaymodes causes children to be added to the popup root.
                // This causes an exception if the NavigationView is in the popup root itself (as SplitView is trying to add children to the tree while it is being measured).
                // Due to this, we want to defer updating this property for all calls coming from `OnApplyTemplate`to the OnLoaded function.
                if (m_fromOnApplyTemplate)
                {
                    m_updateVisualStateForDisplayModeFromOnLoaded = true;
                }
                else
                {
                    splitView.DisplayMode = splitViewDisplayMode;
                }
            }
        }

        /* Event Handlers */

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (m_updateVisualStateForDisplayModeFromOnLoaded)
            {
                m_updateVisualStateForDisplayModeFromOnLoaded = false;
                UpdateVisualStateForDisplayModeGroup(DisplayMode);
            }

            if (m_coreTitleBar != null)
            {
                m_coreTitleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
                m_coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;
            }
            // Update pane buttons now since we the CompactPaneLength is actually known now.
            UpdatePaneButtonsWidths();
        }

        // If app is .net app, the lifetime of NavigationView maybe depends on garbage collection.
        // Unlike other revoker, TitleBar is in global space and we need to stop receiving changed event when it's unloaded.
        // So we do hook it in Loaded and Unhook it in Unloaded
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (m_coreTitleBar != null)
            {
                m_coreTitleBar.LayoutMetricsChanged -= OnTitleBarMetricsChanged;
                m_coreTitleBar.IsVisibleChanged -= OnTitleBarIsVisibleChanged;
            }
        }

        private void OnRepeaterLoaded(object sender, RoutedEventArgs args)
        {
            if (SelectedItem is object item)
            {
                if (!IsSelectionSuppressed(item))
                {
                    if (NavigationViewItemOrSettingsContentFromData(item) is NavigationViewItem navViewItem)
                    {
                        navViewItem.IsSelected = true;
                    }
                }
                AnimateSelectionChanged(item);
            }
        }

        private void OnPaneToggleButtonClick(object sender, RoutedEventArgs args)
        {
            if (IsPaneOpen)
            {
                m_wasForceClosed = true;
                ClosePane();
            }
            else
            {
                m_wasForceClosed = false;
                OpenPane();
            }
        }

        private void OnPaneSearchButtonClick(object sender, RoutedEventArgs args)
        {
            m_wasForceClosed = false;
            OpenPane();

            if (AutoSuggestBox is AutoSuggestBox autoSuggestBox)
            {
                //autoSuggestBox.Focus(FocusState.Keyboard);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    autoSuggestBox.Focus();
                }), DispatcherPriority.Loaded);
            }
        }

        private void OnPaneTitleHolderSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateBackAndCloseButtonsVisibility();
        }

        private void OnNavigationViewItemTapped(object sender, RoutedEventArgs args)
        {
            if (sender is NavigationViewItem nvi)
            {
                OnNavigationViewItemInvoked(nvi);
                nvi.Focus();
                args.Handled = true;
            }
        }

        private void OnNavigationViewItemKeyDown(object sender, KeyEventArgs args)
        {
            if ((args.Key == Key.Enter || args.Key == Key.Space))
            {
                // Only handle those keys if the key is not being held down!
                if (!(args.KeyStates == KeyStates.Down))
                {
                    if (sender is NavigationViewItem nvi)
                    {
                        HandleKeyEventForNavigationViewItem(nvi, args);
                    }
                }
            }
            else
            {
                if (sender is NavigationViewItem nvi)
                {
                    HandleKeyEventForNavigationViewItem(nvi, args);
                }
            }
        }

        //private void OnRepeaterGettingFocus(object sender, GettingFocusEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        private void OnNavigationViewItemOnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is NavigationViewItem nvi)
            {
                // Achieve selection follows focus behavior
                if (IsNavigationViewListSingleSelectionFollowsFocus())
                {
                    // if nvi is already selected we don't need to invoke it again
                    // otherwise ItemInvoked fires twice when item was tapped
                    // or fired when window gets focus
                    if (nvi.SelectsOnInvoked && !nvi.IsSelected)
                    {
                        if (IsTopNavigationView())
                        {
                            if (GetParentItemsRepeaterForContainer(nvi) is ItemsRepeater parentIR)
                            {
                                if (parentIR != m_topNavRepeaterOverflowView)
                                {
                                    OnNavigationViewItemInvoked(nvi);
                                }
                            }
                        }
                        else
                        {
                            OnNavigationViewItemInvoked(nvi);
                        }
                    }
                }
            }
        }

        private void OnNavigationViewItemExpandedPropertyChanged(DependencyObject sender, DependencyProperty args)
        {
            if (sender is NavigationViewItem nvi)
            {
                if (nvi.IsExpanded)
                {
                    RaiseExpandingEvent(nvi);
                }

                ShowHideChildrenItemsRepeater(nvi);

                if (!nvi.IsExpanded)
                {
                    RaiseCollapsedEvent(nvi);
                }
            }
        }

        private void RaiseSelectionChangedEvent(object nextItem, bool isSettingsItem, NavigationRecommendedTransitionDirection recommendedDirection = NavigationRecommendedTransitionDirection.Default)
        {
            NavigationViewSelectionChangedEventArgs eventArgs = new NavigationViewSelectionChangedEventArgs();
            eventArgs.SelectedItem = nextItem;
            eventArgs.IsSettingsSelected = isSettingsItem;
            if (NavigationViewItemBaseOrSettingsContentFromData(nextItem) is NavigationViewItemBase container)
            {
                eventArgs.SelectedItemContainer = container;
            }
            eventArgs.RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recommendedDirection);
            SelectionChanged?.Invoke(this, eventArgs);
        }

        private void OnTitleBarMetricsChanged(object sender, object args)
        {
            UpdateTitleBarPadding();
        }

        private void OnTitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarPadding();
        }

        private void UpdateTitleBarPadding()
        {
            if (!m_appliedTemplate)
            {
                return;
            }

            double topPadding = 0;

            if (m_coreTitleBar != null)
            {
                bool needsTopPadding = false;

                // Do not set a top padding when the IsTitleBarAutoPaddingEnabled property is set to False.
                if (IsTitleBarAutoPaddingEnabled)
                {
                    if (ShouldPreserveNavigationViewRS3Behavior())
                    {
                        needsTopPadding = true;
                    }
                    else if (ShouldPreserveNavigationViewRS4Behavior())
                    {
                        // For RS4 apps maintain the behavior that we shipped for RS4.
                        // We keep this behavior for app compact purposes.
                        needsTopPadding = !m_coreTitleBar.ExtendViewIntoTitleBar;
                    }
                    else
                    {
                        needsTopPadding = NeedTopPaddingForRS5OrHigher(m_coreTitleBar);
                    }
                }

                if (needsTopPadding)
                {
                    // Only add extra padding if the NavView is the "root" of the app,
                    // but not if the app is expanding into the titlebar
                    UIElement root = (Window.GetWindow(this) ?? Application.Current.MainWindow).Content as UIElement;
                    GeneralTransform gt = ControlExtensions.AltTransformToVisual(this, root); // TransformToVisual(root);
                    Point pos = gt.Transform(new Point());

                    if (pos.Y == 0.0)
                    {
                        topPadding = m_coreTitleBar.Height;
                    }
                }

                if (ShouldPreserveNavigationViewRS4Behavior())
                {
                    if (m_togglePaneTopPadding is FrameworkElement fe)
                    {
                        fe.Height = topPadding;
                    }

                    if (m_contentPaneTopPadding is FrameworkElement fep)
                    {
                        fep.Height = topPadding;
                    }
                }

                FrameworkElement paneTitleHolderFrameworkElement = m_paneTitleHolderFrameworkElement;
                Button paneToggleButton = m_paneToggleButton;

                bool setPaneTitleHolderFrameworkElementMargin = paneTitleHolderFrameworkElement != null && paneTitleHolderFrameworkElement.Visibility == Visibility.Visible;
                bool setPaneToggleButtonMargin = !setPaneTitleHolderFrameworkElementMargin && paneToggleButton != null && paneToggleButton.Visibility == Visibility.Visible;

                if (setPaneTitleHolderFrameworkElementMargin || setPaneToggleButtonMargin)
                {
                    Thickness thickness = new Thickness(0, 0, 0, 0);

                    if (ShouldShowBackButton())
                    {
                        if (IsOverlay())
                        {
                            thickness = new Thickness(c_backButtonWidth, 0, 0, 0);
                        }
                        else
                        {
                            thickness = new Thickness(0, c_backButtonHeight, 0, 0);
                        }
                    }
                    else if (ShouldShowCloseButton() && IsOverlay())
                    {
                        thickness = new Thickness(c_backButtonWidth, 0, 0, 0);
                    }

                    if (setPaneTitleHolderFrameworkElementMargin)
                    {
                        // The PaneHeader is hosted by PaneTitlePresenter and PaneTitleHolder.
                        paneTitleHolderFrameworkElement.Margin = thickness;
                    }
                    else
                    {
                        // The PaneHeader is hosted by PaneToggleButton
                        paneToggleButton.Margin = thickness;
                    }
                }
            }

            if (TemplateSettings is NavigationViewTemplateSettings templateSettings)
            {
                // 0.0 and 0.00000000 is not the same in double world. try to reduce the number of TopPadding update event. epsilon is 0.1 here.
                if (Math.Abs(templateSettings.TopPadding - topPadding) > 0.1)
                {
                    GetTemplateSettings().TopPadding = topPadding;
                }
            }
        }

        private void OnAutoSuggestBoxSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // When in compact or minimal, we want to close pane when an item gets selected.
            if (DisplayMode != NavigationViewDisplayMode.Expanded && args.SelectedItem != null)
            {
                ClosePane();
            }
        }

        private void RaiseDisplayModeChanged(NavigationViewDisplayMode displayMode)
        {
            SetValue(DisplayModePropertyKey, displayMode);

            NavigationViewDisplayModeChangedEventArgs eventArgs = new NavigationViewDisplayModeChangedEventArgs();
            eventArgs.DisplayMode = displayMode;

            DisplayModeChanged?.Invoke(this, eventArgs);
        }

        private void AnimateSelectionChanged(object nextItem)
        {
            // If we are delaying animation due to item movement in top nav overflow, dont do anything
            if (m_lastSelectedItemPendingAnimationInTopNav != null)
            {
                return;
            }

            UIElement prevIndicator = m_activeIndicator;
            UIElement nextIndicator = FindSelectionIndicator(nextItem);

            bool haveValidAnimation = false;
            // It's possible that AnimateSelectionChanged is called multiple times before the first animation is complete.
            // To have better user experience, if the selected target is the same, keep the first animation
            // If the selected target is not the same, abort the first animation and launch another animation.
            if (m_prevIndicator != null || m_nextIndicator != null) // There is ongoing animation
            {
                if (nextIndicator != null && m_nextIndicator == nextIndicator) // animate to the same target, just wait for animation complete
                {
                    if (prevIndicator != null && prevIndicator != m_prevIndicator)
                    {
                        ResetElementAnimationProperties(prevIndicator, 0.0f);
                    }
                    haveValidAnimation = true;
                }
                else
                {
                    // If the last animation is still playing, force it to complete.
                    OnAnimationComplete(null, null);
                }
            }

            if (!haveValidAnimation)
            {
                UIElement paneContentGrid = m_paneContentGrid;
                if ((prevIndicator != nextIndicator) && paneContentGrid != null && prevIndicator != null && nextIndicator != null && false)
                {
                    // Make sure both indicators are visible and in their original locations
                    ResetElementAnimationProperties(prevIndicator, 1.0);
                    ResetElementAnimationProperties(nextIndicator, 1.0);

                    // get the item positions in the pane
                    Point point = new Point(0, 0);
                    double prevPos;
                    double nextPos;

                    Point prevPosPoint = prevIndicator.TransformToVisual(paneContentGrid).Transform(point);
                    Point nextPosPoint = nextIndicator.TransformToVisual(paneContentGrid).Transform(point);
                    Size prevSize = prevIndicator.RenderSize;
                    Size nextSize = nextIndicator.RenderSize;

                    bool areElementsAtSameDepth = false;
                    if (IsTopNavigationView())
                    {
                        prevPos = prevPosPoint.X;
                        nextPos = nextPosPoint.X;
                        areElementsAtSameDepth = prevPosPoint.Y == nextPosPoint.Y;
                    }
                    else
                    {
                        prevPos = prevPosPoint.Y;
                        nextPos = nextPosPoint.Y;
                        areElementsAtSameDepth = prevPosPoint.X == nextPosPoint.X;
                    }

                    //Visual visual = ElementCompositionPreview.GetElementVisual(this);
                    //CompositionScopedBatch scopedBatch = visual.Compositor().CreateScopedBatch(CompositionBatchTypes.Animation);

                    Storyboard scopedBatch = new Storyboard();

                    if (!areElementsAtSameDepth)
                    {
                        bool isNextBelow = prevPosPoint.Y < nextPosPoint.Y;
                        if (prevIndicator.RenderSize.Height > prevIndicator.RenderSize.Width)
                        {
                            PlayIndicatorNonSameLevelAnimations(prevIndicator, true, isNextBelow ? false : true);
                        }
                        else
                        {
                            PlayIndicatorNonSameLevelTopPrimaryAnimation(prevIndicator, true);
                        }

                        if (nextIndicator.RenderSize.Height > nextIndicator.RenderSize.Width)
                        {
                            PlayIndicatorNonSameLevelAnimations(nextIndicator, false, isNextBelow ? true : false);
                        }
                        else
                        {
                            PlayIndicatorNonSameLevelTopPrimaryAnimation(nextIndicator, false);
                        }

                    }
                    else
                    {

                        double outgoingEndPosition = nextPos - prevPos;
                        double incomingStartPosition = prevPos - nextPos;

                        // Play the animation on both the previous and next indicators
                        PlayIndicatorAnimations(prevIndicator,
                            0,
                            outgoingEndPosition,
                            prevSize,
                            nextSize,
                            true);
                        PlayIndicatorAnimations(nextIndicator,
                            incomingStartPosition,
                            0,
                            prevSize,
                            nextSize,
                            false);
                    }

                    //scopedBatch.End();
                    m_prevIndicator = prevIndicator;
                    m_nextIndicator = nextIndicator;

                    scopedBatch.Completed += OnAnimationComplete;
                    scopedBatch.Begin(this, true);
                }
                else
                {
                    // if all else fails, or if animations are turned off, attempt to correctly set the positions and opacities of the indicators.
                    ResetElementAnimationProperties(prevIndicator, 0.0);
                    ResetElementAnimationProperties(nextIndicator, 1.0);
                }

                m_activeIndicator = nextIndicator;
            }


            // TODO Анимации выбора
            // Над этим нужно поработать
            System.Diagnostics.Debug.WriteLine("CreateNavigationTransitionInfo: Not Implemented");
            return;
            throw new NotImplementedException();
        }

        private void AnimateSelectionChangedToItem(object selectedItem)
        {
            if (selectedItem != null && !IsSelectionSuppressed(selectedItem))
            {
                AnimateSelectionChanged(selectedItem);
            }
        }

        private void PlayIndicatorAnimations(UIElement indicator, double yFrom, double yTo, Size beginSize, Size endSize, bool isOutgoing)
        {
            // TODO Анимации выбора
            // Над этим нужно поработать
            // throw new NotImplementedException();
        }

        private void PlayIndicatorNonSameLevelAnimations(UIElement indicator, bool isOutgoing, bool fromTop)
        {
            // TODO Анимации выбора
            // Над этим нужно поработать
            // throw new NotImplementedException();
        }

        private void PlayIndicatorNonSameLevelTopPrimaryAnimation(UIElement indicator, bool isOutgoing)
        {
            // TODO Анимации выбора
            // Над этим нужно поработать
            // throw new NotImplementedException();
        }

        private void OnAnimationComplete(object sender, EventArgs args)
        {
            ResetElementAnimationProperties(m_prevIndicator, 0.0);
            m_prevIndicator = null;

            ResetElementAnimationProperties(m_nextIndicator, 1.0);
            m_nextIndicator = null;
            // TODO Анимации выбора
            // Над этим нужно поработать
            // throw new NotImplementedException();
        }

        private void ResetElementAnimationProperties(UIElement element, double desiredOpacity)
        {
            if(element != null)
            {
                element.Opacity = desiredOpacity;
            }
            // TODO Анимации выбора
            // Над этим нужно поработать
            // throw new NotImplementedException();
        }

        private NavigationViewItem NavigationViewItemOrSettingsContentFromData(object data)
        {
            return GetContainerForData<NavigationViewItem>(data);
        }

        private NavigationViewItemBase NavigationViewItemBaseOrSettingsContentFromData(object data)
        {
            return GetContainerForData<NavigationViewItemBase>(data);
        }

        /* *** */

        private T GetContainerForData<T>(object data) where T : class
        {
            if (data == null)
            {
                return null;
            }

            if (data is T nvi)
            {
                return nvi;
            }

            // First conduct a basic top level search in main menu, which should succeed for a lot of scenarios.
            ItemsRepeater mainRepeater = IsTopNavigationView() ? m_topNavRepeater : m_leftNavRepeater;
            int itemIndex = GetIndexFromItem(mainRepeater, data);
            if (itemIndex >= 0)
            {
                if (mainRepeater.TryGetElement(itemIndex) is UIElement container)
                {
                    return container as T;
                }
            }

            // then look in footer menu
            ItemsRepeater footerRepeater = IsTopNavigationView() ? m_topNavFooterMenuRepeater : m_leftNavFooterMenuRepeater;
            itemIndex = GetIndexFromItem(footerRepeater, data);
            if (itemIndex >= 0)
            {
                if (footerRepeater.TryGetElement(itemIndex) is UIElement container)
                {
                    return container as T;
                }
            }

            // If unsuccessful, unfortunately we are going to have to search through the whole tree
            // TODO: Either fix or remove implementation for TopNav.
            // It may not be required due to top nav rarely having realized children in its default state.
            if (SearchEntireTreeForContainer(mainRepeater, data) is UIElement mcontainer)
            {
                return mcontainer as T;
            }

            if (SearchEntireTreeForContainer(footerRepeater, data) is UIElement fcontainer)
            {
                return fcontainer as T;
            }

            return null;
        }

        private void OpenPane()
        {
            try
            {
                m_isOpenPaneForInteraction = true;
                IsPaneOpen = true;
            }
            finally
            {
                m_isOpenPaneForInteraction = false;
            }
        }

        private void ClosePane()
        {
            CollapseMenuItemsInRepeater(m_leftNavRepeater);

            try
            {
                m_isOpenPaneForInteraction = true;
                IsPaneOpen = false; // the SplitView is two-way bound to this value 
            }
            finally
            {
                m_isOpenPaneForInteraction = false;
            }
        }

        private bool AttemptClosePaneLightly()
        {
            bool pendingPaneClosingCancel = false;

            NavigationViewPaneClosingEventArgs eventArgs = new NavigationViewPaneClosingEventArgs();
            PaneClosing?.Invoke(this, eventArgs);
            pendingPaneClosingCancel = eventArgs.Cancel;

            if (!pendingPaneClosingCancel || m_wasForceClosed)
            {
                m_blockNextClosingEvent = true;
                ClosePane();
                return true;
            }

            return false;
        }

        // The automation name and tooltip for the pane toggle button changes depending on whether it is open or closed
        // put the logic here as it will be called in a couple places
        private void SetPaneToggleButtonAutomationName()
        {
            string navigationName;
            if (IsPaneOpen)
            {
                navigationName = Properties.Resources.Strings.Resources.NavigationButtonOpenName;
            }
            else
            {
                navigationName = Properties.Resources.Strings.Resources.NavigationButtonClosedName;
            }

            if (m_paneToggleButton is Button paneToggleButton)
            {
                AutomationProperties.SetName(paneToggleButton, navigationName);
                ToolTip toolTip = new ToolTip();
                toolTip.Content = navigationName;
                ToolTipService.SetToolTip(paneToggleButton, toolTip);
            }
        }

        private void SwapPaneHeaderContent(ContentControl newParent, ContentControl oldParent, string propertyPathName)
        {
            if (newParent != null)
            {
                if (oldParent != null)
                {
                    oldParent.ClearValue(ContentControl.ContentProperty);
                }

                // SharedHelpers.SetBinding(propertyPathName, newParent, ContentControl.ContentProperty);

                BindingOperations.SetBinding(newParent, ContentControl.ContentProperty, new Binding(propertyPathName) { RelativeSource = RelativeSource.TemplatedParent });
            }
        }

        private void UpdateSettingsItemToolTip()
        {
            if (m_settingsItem is NavigationViewItem settingsItem)
            {
                if (!IsTopNavigationView() && IsPaneOpen)
                {
                    ToolTipService.SetToolTip(settingsItem, null);
                }
                else
                {
                    string localizedSettingsName = Properties.Resources.Strings.Resources.SettingsButtonName;
                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = localizedSettingsName;
                    ToolTipService.SetToolTip(settingsItem, toolTip);
                }
            }
        }

        // Updates the PaneTitleHolder.Visibility and PaneTitleTextBlock.Parent properties based on the PaneDisplayMode, PaneTitle and IsPaneToggleButtonVisible properties.
        private void UpdatePaneTitleFrameworkElementParents()
        {
            if (m_paneTitleHolderFrameworkElement is FrameworkElement paneTitleHolderFrameworkElement)
            {
                bool isPaneToggleButtonVisible = IsPaneToggleButtonVisible;
                bool isTopNavigationView = IsTopNavigationView();

                paneTitleHolderFrameworkElement.Visibility =
                    (isPaneToggleButtonVisible ||
                    isTopNavigationView ||
                    PaneTitle.Length == 0 ||
                    (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !IsPaneOpen)) ? Visibility.Collapsed : Visibility.Visible;

                if (m_paneTitleFrameworkElement is FrameworkElement paneTitleFrameworkElement)
                {
                    Action first = SetPaneTitleFrameworkElementParent(m_paneToggleButton, paneTitleFrameworkElement, isTopNavigationView || !isPaneToggleButtonVisible);
                    Action second = SetPaneTitleFrameworkElementParent(m_paneTitlePresenter, paneTitleFrameworkElement, isTopNavigationView || isPaneToggleButtonVisible);
                    Action third = SetPaneTitleFrameworkElementParent(m_paneTitleOnTopPane, paneTitleFrameworkElement, !isTopNavigationView || isPaneToggleButtonVisible);
                    (first ?? second ?? third)?.Invoke();
                }
            }
        }

        private Action SetPaneTitleFrameworkElementParent(ContentControl parent, FrameworkElement paneTitle, bool shouldNotContainPaneTitle)
        {
            if (parent != null)
            {
                if ((parent.Content == paneTitle) == shouldNotContainPaneTitle)
                {
                    if (shouldNotContainPaneTitle)
                    {
                        parent.Content = null;
                    }
                    else
                    {
                        return new Action(() =>
                        {
                            parent.Content = paneTitle;
                        });
                    }
                }
            }
            return null;
        }

        private void OnSplitViewClosedCompactChanged(DependencyObject sender, DependencyProperty args)
        {
            if (args == SplitView.IsPaneOpenProperty || args == SplitView.DisplayModeProperty)
            {
                UpdateIsClosedCompact();
            }
        }

        private void OnSplitViewPaneClosed(DependencyObject sender, object obj)
        {
            PaneClosed?.Invoke(this, null);
        }

        private void OnSplitViewPaneClosing(DependencyObject sender, SplitViewPaneClosingEventArgs args)
        {
            bool pendingPaneClosingCancel = false;
            if (PaneClosing != null)
            {
                if (!m_blockNextClosingEvent) // If this is true, we already sent one out "manually" and don't need to forward SplitView's event
                {
                    NavigationViewPaneClosingEventArgs eventArgs = new NavigationViewPaneClosingEventArgs();
                    eventArgs.SplitViewClosingArgs(args);
                    PaneClosing?.Invoke(this, eventArgs);
                    pendingPaneClosingCancel = eventArgs.Cancel;
                }
                else
                {
                    m_blockNextClosingEvent = false;
                }
            }

            if (!pendingPaneClosingCancel) // will be set in above event!
            {
                if (m_rootSplitView is SplitView splitView)
                {
                    if (m_leftNavRepeater is ItemsRepeater paneList)
                    {
                        if (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline)
                        {
                            // See UpdateIsClosedCompact 'RS3+ animation timing enhancement' for explanation:
                            VisualStateManager.GoToState(this, "ListSizeCompact", true /*useTransitions*/);
                            PaneStateListSizeState = "ListSizeCompact";
                            UpdatePaneToggleSize();
                        }
                    }
                }
            }
        }

        private void OnSplitViewPaneOpened(DependencyObject sender, object obj)
        {
            PaneOpened?.Invoke(this, null);
        }

        private void OnSplitViewPaneOpening(DependencyObject sender, object obj)
        {
            if (m_leftNavRepeater != null)
            {
                // See UpdateIsClosedCompact 'RS3+ animation timing enhancement' for explanation:
                VisualStateManager.GoToState(this, "ListSizeFull", true /*useTransitions*/); ;
                PaneStateListSizeState = "ListSizeFull";
            }

            PaneOpening?.Invoke(this, null);
        }

        internal void UpdateIsClosedCompact()
        {
            if (m_rootSplitView is SplitView splitView)
            {
                // Check if the pane is closed and if the splitview is in either compact mode.
                SplitViewDisplayMode splitViewDisplayMode = splitView.DisplayMode;
                m_isClosedCompact = !splitView.IsPaneOpen && (splitViewDisplayMode == SplitViewDisplayMode.CompactOverlay || splitViewDisplayMode == SplitViewDisplayMode.CompactInline);
                VisualStateManager.GoToState(this, m_isClosedCompact ? "ClosedCompact" : "NotClosedCompact", true /*useTransitions*/);

                // Set the initial state of the list size
                if (!m_initialListSizeStateSet)
                {
                    m_initialListSizeStateSet = true; // Если это восстановить будет проблемма с всплывающим окном при
                    string stateName = m_isClosedCompact ? "ListSizeCompact" : "ListSizeFull";
                    VisualStateManager.GoToState(this, stateName, true /*useTransitions*/);
                    PaneStateListSizeState = stateName;
                }

                UpdateTitleBarPadding();
                UpdateBackAndCloseButtonsVisibility();
                UpdatePaneTitleMargins();
                UpdatePaneToggleSize();
            }
        }

        private void OnBackButtonClicked(object sender, RoutedEventArgs args)
        {
            NavigationViewBackRequestedEventArgs eventArgs = new NavigationViewBackRequestedEventArgs();
            BackRequested?.Invoke(this, eventArgs);
        }

        private bool IsOverlay()
        {
            if (m_rootSplitView != null)
            {
                return m_rootSplitView.DisplayMode == SplitViewDisplayMode.Overlay;
            }
            else
            {
                return false;
            }
        }

        private bool IsLightDismissible()
        {
            if (m_rootSplitView != null)
            {
                return m_rootSplitView.DisplayMode != SplitViewDisplayMode.Inline && m_rootSplitView.DisplayMode != SplitViewDisplayMode.CompactInline;
            }
            else
            {
                return false;
            }
        }

        private bool ShouldShowBackButton()
        {
            if (m_backButton != null && !ShouldPreserveNavigationViewRS3Behavior())
            {
                if (DisplayMode == NavigationViewDisplayMode.Minimal && IsPaneOpen)
                {
                    return false;
                }

                return ShouldShowBackOrCloseButton();
            }

            return false;
        }

        private bool ShouldShowCloseButton()
        {
            if (m_backButton != null && !ShouldPreserveNavigationViewRS3Behavior() && m_closeButton != null)
            {
                if (!IsPaneOpen)
                {
                    return false;
                }

                NavigationViewPaneDisplayMode paneDisplayMode = PaneDisplayMode;

                if (paneDisplayMode != NavigationViewPaneDisplayMode.LeftMinimal &&
                    (paneDisplayMode != NavigationViewPaneDisplayMode.Auto || DisplayMode != NavigationViewDisplayMode.Minimal))
                {
                    return false;
                }

                return ShouldShowBackOrCloseButton();
            }

            return false;
        }

        private bool ShouldShowBackOrCloseButton()
        {
            NavigationViewBackButtonVisible visibility = IsBackButtonVisible;
            return (visibility == NavigationViewBackButtonVisible.Visible || (visibility == NavigationViewBackButtonVisible.Auto));
        }

        private void UnhookEventsAndClearFields(bool isFromDestructor = false)
        {
            if (m_coreTitleBar != null)
            {
                m_coreTitleBar.LayoutMetricsChanged -= OnTitleBarMetricsChanged;
                m_coreTitleBar.IsVisibleChanged -= OnTitleBarIsVisibleChanged;
            }

            if (m_paneToggleButton != null)
            {
                m_paneToggleButton.Click -= OnPaneToggleButtonClick;
            }

            m_settingsItem = null;

            if (m_paneSearchButton != null)
            {
                m_paneSearchButton.Click -= OnPaneSearchButtonClick;
                m_paneSearchButton = null;
            }

            m_paneHeaderOnTopPane = null;
            m_paneTitleOnTopPane = null;

            if (m_itemsContainerGrid != null)
            {
                m_itemsContainerGrid.SizeChanged -= OnItemsContainerSizeChanged;
            }

            if (m_paneTitleHolderFrameworkElement != null)
            {
                m_paneTitleHolderFrameworkElement.SizeChanged -= OnPaneTitleHolderSizeChanged;
                m_paneTitleHolderFrameworkElement = null;
            }

            m_paneTitleFrameworkElement = null;
            m_paneTitlePresenter = null;

            m_paneHeaderCloseButtonColumn = null;
            m_paneHeaderToggleButtonColumn = null;
            m_paneHeaderContentBorderRow = null;

            if (m_leftNavRepeater != null)
            {
                m_leftNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;
                m_leftNavRepeater.ElementClearing -= OnRepeaterElementClearing;
                //m_leftNavRepeater.IsVisibleChanged -= OnRepeaterIsVisibleChanged;
                m_leftNavRepeater = null;
            }

            if (m_topNavRepeater != null)
            {
                m_topNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;
                m_topNavRepeater.ElementClearing -= OnRepeaterElementClearing;
                //m_topNavRepeater.IsVisibleChanged -= OnRepeaterIsVisibleChanged;
                m_topNavRepeater = null;
            }

            if (m_leftNavFooterMenuRepeater != null)
            {
                m_leftNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;
                m_leftNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
                //m_leftNavFooterMenuRepeater.IsVisibleChanged -= OnRepeaterIsVisibleChanged;
                m_leftNavFooterMenuRepeater = null;
            }

            if (m_topNavFooterMenuRepeater != null)
            {
                m_topNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;
                m_topNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
                // m_topNavFooterMenuRepeater.IsVisibleChanged -= OnRepeaterIsVisibleChanged;
                m_topNavFooterMenuRepeater = null;

                // m_footerItemsCollectionChangedRevoker?.Revoke();
                // m_menuItemsCollectionChangedRevoker?.Revoke();

                if (m_footerItemsSource != null)
                {
                    m_footerItemsSource.CollectionChanged -= OnFooterItemsSourceCollectionChanged;
                }

                if (m_menuItemsSource != null)
                {
                    m_menuItemsSource.CollectionChanged -= OnMenuItemsSourceCollectionChanged;
                }

                if (m_topNavRepeaterOverflowView != null)
                {
                    m_topNavRepeaterOverflowView.ElementPrepared -= OnRepeaterElementPrepared;
                    m_topNavRepeaterOverflowView.ElementClearing -= OnRepeaterElementClearing;
                    m_topNavRepeaterOverflowView = null;
                }

                // m_topNavOverflowItemsCollectionChangedRevoker?.Revoke();

                if (m_topNavRepeaterOverflowView != null)
                {
                    m_topNavRepeaterOverflowView.ItemsSourceView.CollectionChanged -= OnOverflowItemsSourceCollectionChanged;
                }

                if (isFromDestructor)
                {
                    m_selectionModel.SelectionChanged -= OnSelectionModelSelectionChanged;
                }
            }
        }

        private bool ShouldPreserveNavigationViewRS4Behavior()
        {
            // Since RS5, we support topnav
            return m_topNavGrid == null;
        }

        private bool ShouldPreserveNavigationViewRS3Behavior()
        {
            // Since RS4, we support backbutton
            return m_backButton == null;
        }

        private bool NeedRearrangeOfTopElementsAfterOverflowSelectionChanged(int selectedOriginalIndex)
        {
            bool needRearrange = false;

            IList primaryList = m_topDataProvider.GetPrimaryItems();
            int primaryListSize = primaryList.Count;
            int indexInPrimary = m_topDataProvider.ConvertOriginalIndexToIndex(selectedOriginalIndex);

            // We need to verify that through various overflow selection combinations, the primary
            // items have not been put into a state of non-logical item layout (aka not in proper sequence).
            // To verify this, if the newly selected item has items following it in the primary items:
            // - we verify that they are meant to follow the selected item as specified in the original order
            // - we verify that the preceding item is meant to directly precede the selected item in the original order
            // If these two conditions are not met, we move all items to the primary list and trigger a re-arrangement of the items.
            if (indexInPrimary < primaryListSize - 1)
            {
                int nextIndexInPrimary = indexInPrimary + 1;
                int nextIndexInOriginal = selectedOriginalIndex + 1;
                int prevIndexInOriginal = selectedOriginalIndex - 1;

                // Check whether item preceding the selected is not directly preceding
                // in the original.
                if (indexInPrimary > 0)
                {
                    List<int> prevIndexInVector = new List<int>();
                    prevIndexInVector.Add(nextIndexInPrimary - 1);
                    List<int> prevOriginalIndexOfPrevPrimaryItem = m_topDataProvider.ConvertPrimaryIndexToIndex(prevIndexInVector);
                    if (prevOriginalIndexOfPrevPrimaryItem[0] != prevIndexInOriginal)
                    {
                        needRearrange = true;
                    }
                }

                // Check whether items following the selected item are out of order
                while (!needRearrange && nextIndexInPrimary < primaryListSize)
                {
                    List<int> nextIndexInVector = new List<int>();
                    nextIndexInVector.Add(nextIndexInPrimary);
                    List<int> originalIndex = m_topDataProvider.ConvertPrimaryIndexToIndex(nextIndexInVector);
                    if (nextIndexInOriginal != originalIndex[0])
                    {
                        needRearrange = true;
                        break;
                    }
                    nextIndexInPrimary++;
                    nextIndexInOriginal++;
                }
            }

            return needRearrange;
        }

        private void KeyboardFocusFirstItemFromItem(NavigationViewItemBase nvib)
        {
            ItemsRepeater parentIR = GetParentRootItemsRepeaterForContainer(nvib);
            UIElement firstElement = parentIR.TryGetElement(0);

            if (firstElement is Control controlFirst)
            {
                controlFirst.Focus();
            }
        }

        private void KeyboardFocusLastItemFromItem(NavigationViewItemBase nvib)
        {
            ItemsRepeater parentIR = GetParentRootItemsRepeaterForContainer(nvib);

            if (parentIR.ItemsSourceView is ItemsSourceView itemsSourceView)
            {
                int lastIndex = itemsSourceView.Count - 1;
                if (parentIR.TryGetElement(lastIndex) is UIElement lastElement)
                {
                    if (lastElement is Control controlLast)
                    {
                        controlLast.Focus();
                    }
                }
            }
        }

        private void FocusNextDownItem(NavigationViewItem nvi, KeyEventArgs args)
        {
            if (args.OriginalSource != nvi)
            {
                return;
            }

            if (DoesNavigationViewItemHaveChildren(nvi))
            {
                NavigationViewItem nviImpl = nvi;
                if (nviImpl.GetRepeater() is ItemsRepeater childRepeater)
                {
                    //var firstFocusableElement = FocusManager.FindFirstFocusableElement(childRepeater);
                    //if (firstFocusableElement is Control controlFirst)
                    //{
                    //    args.Handled = controlFirst.Focus();
                    //}
                }
            }
        }

        private void FocusNextUpItem(NavigationViewItem nvi, KeyEventArgs args)
        {
            if (args.OriginalSource != nvi)
            {
                return;
            }

            bool shouldHandleFocus = true;
            NavigationViewItem nviImpl = nvi;
            // bool nextFocusableElement = FocusManager.FindNextFocusableElement(FocusNavigationDirection.Up);
            UIElement nextFocusableElement = FindNextFocusableElement(FocusNavigationDirection.Up);

            if (nextFocusableElement is NavigationViewItem nextFocusableNVI)
            {
                NavigationViewItem nextFocusableNVIImpl = nextFocusableNVI;

                if (nextFocusableNVIImpl.Depth == nviImpl.Depth)
                {
                    // If we not at the top of the list for our current depth and the item above us has children, check whether we should move focus onto a child
                    if (DoesNavigationViewItemHaveChildren(nextFocusableNVI))
                    {
                        // Focus on last lowest level visible container
                        if (nextFocusableNVIImpl.GetRepeater() is ItemsRepeater childRepeater)
                        {
                            //if (FocusManager.FindLastFocusableElement(childRepeater) is UIElement lastFocusableElement)
                            //{
                            //    if (lastFocusableElement is Control lastFocusableNVI)
                            //    {
                            //        args.Handled = lastFocusableNVI.Focus(FocusState.Keyboard));
                            //    }
                            //}
                            //else
                            //{
                            //    args.Handled = nextFocusableNVIImpl.Focus(FocusState.Keyboard));
                            //}

                            if (childRepeater.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last)))
                            {
                                args.Handled = true;
                            }
                            else
                            {
                                args.Handled = nextFocusableNVIImpl.Focus(/*FocusState.Keyboard*/);
                            }
                        }
                    }
                    else
                    {
                        // Traversing up a list where XYKeyboardFocus will result in correct behavior
                        shouldHandleFocus = false;
                    }
                }
            }

            // We are at the top of the list, focus on parent
            if (shouldHandleFocus && !args.Handled && nviImpl.Depth > 0)
            {
                if (GetParentNavigationViewItemForContainer(nvi) is NavigationViewItem parentContainer)
                {
                    args.Handled = parentContainer.Focus();
                }
            }
        }

        public UIElement FindNextFocusableElement(FocusNavigationDirection focusNavigationDirection)
        {
            if (Keyboard.FocusedElement is UIElement focusedElement)
            {
                return focusedElement.PredictFocus(focusNavigationDirection) as UIElement;
            }

            return null;
        }

        private void ApplyCustomMenuItemContainerStyling(NavigationViewItemBase nvib, ItemsRepeater ir, int index)
        {
            if (MenuItemContainerStyle is Style menuItemContainerStyle)
            {
                nvib.Style = menuItemContainerStyle;
            }
            else if (MenuItemContainerStyleSelector is StyleSelector menuItemContainerStyleSelector)
            {
                if (ir.ItemsSourceView is ItemsSourceView itemsSourceView)
                {
                    if (itemsSourceView.GetAt(index) is object item)
                    {
                        if (menuItemContainerStyleSelector.SelectStyle(item, nvib) is Style selectedStyle)
                        {
                            nvib.Style = selectedStyle;
                        }
                    }
                }
            }
        }

        private void SetDropShadow()
        {
            /* This method is Empty */
        }

        private void UnsetDropShadow()
        {
            /* This method is Empty */
        }

        private void ShadowCasterEaseOutStoryboard_Completed(Grid shadowCaster)
        {
            /* This method is Empty */
        }

        #endregion

        // Cache these objects for the view as they are expensive to query via GetForCurrentView() calls.
        // ViewManagement.ApplicationView m_applicationView = null;
        // ViewManagement.UIViewSettings m_uiViewSettings = null;
    }
}
