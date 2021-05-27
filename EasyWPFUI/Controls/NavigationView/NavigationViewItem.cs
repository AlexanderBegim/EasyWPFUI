using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Common;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public class NavigationViewItem : NavigationViewItemBase
    {
        private const string c_navigationViewItemPresenterName = "NavigationViewItemPresenter";
        private const string c_repeater = "NavigationViewItemMenuItemsHost";
        private const string c_rootGrid = "NVIRootGrid";
        private const string c_flyoutContentGrid = "FlyoutContentGrid";

        #region Visual States

        private const string c_pressedSelected = "PressedSelected";
        private const string c_pointerOverSelected = "PointerOverSelected";
        private const string c_selected = "Selected";
        private const string c_pressed = "Pressed";
        private const string c_pointerOver = "PointerOver";
        private const string c_disabled = "Disabled";
        private const string c_enabled = "Enabled";
        private const string c_normal = "Normal";
        private const string c_chevronHidden = "ChevronHidden";
        private const string c_chevronVisibleOpen = "ChevronVisibleOpen";
        private const string c_chevronVisibleClosed = "ChevronVisibleClosed";

        private const string c_normalChevronHidden = "NormalChevronHidden";
        private const string c_normalChevronVisibleOpen = "NormalChevronVisibleOpen";
        private const string c_normalChevronVisibleClosed = "NormalChevronVisibleClosed";
        private const string c_pointerOverChevronHidden = "PointerOverChevronHidden";
        private const string c_pointerOverChevronVisibleOpen = "PointerOverChevronVisibleOpen";
        private const string c_pointerOverChevronVisibleClosed = "PointerOverChevronVisibleClosed";
        private const string c_pressedChevronHidden = "PressedChevronHidden";
        private const string c_pressedChevronVisibleOpen = "PressedChevronVisibleOpen";
        private const string c_pressedChevronVisibleClosed = "PressedChevronVisibleClosed";

        #endregion

        #region Events

        internal event TypedEventHandler<DependencyObject, DependencyProperty> IsExpandedChanged;

        #endregion

        #region Icon Property

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(NavigationViewItem), new FrameworkPropertyMetadata(OnIconPropertyChanged));

        public IconElement Icon
        {
            get
            {
                return (IconElement)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewItem navigationViewItem = (NavigationViewItem)d;

            navigationViewItem.OnPropertyChanged(e.Property);
        }

        #endregion

        #region CompactPaneLength Property

        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(NavigationViewItem), new FrameworkPropertyMetadata(48d));

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

        #endregion

        #region SelectsOnInvoked Property

        public static readonly DependencyProperty SelectsOnInvokedProperty = DependencyProperty.Register("SelectsOnInvoked", typeof(bool), typeof(NavigationViewItem), new FrameworkPropertyMetadata(true));

        public bool SelectsOnInvoked
        {
            get
            {
                return (bool)GetValue(SelectsOnInvokedProperty);
            }
            set
            {
                SetValue(SelectsOnInvokedProperty, value);
            }
        }

        #endregion

        #region IsExpanded Property

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(NavigationViewItem), new FrameworkPropertyMetadata(false, OnIsExpandedPropertyChanged));

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewItem navigationViewItem = (NavigationViewItem)d;

            navigationViewItem.OnPropertyChanged(e.Property);
        }

        #endregion

        #region HasUnrealizedChildren Property

        public static readonly DependencyProperty HasUnrealizedChildrenProperty = DependencyProperty.Register("HasUnrealizedChildren", typeof(bool), typeof(NavigationViewItem), new FrameworkPropertyMetadata(false, OnHasUnrealizedChildrenPropertyChanged));

        public bool HasUnrealizedChildren
        {
            get
            {
                return (bool)GetValue(HasUnrealizedChildrenProperty);
            }
            set
            {
                SetValue(HasUnrealizedChildrenProperty, value);
            }
        }

        private static void OnHasUnrealizedChildrenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewItem navigationViewItem = (NavigationViewItem)d;

            navigationViewItem.OnPropertyChanged(e.Property);
        }

        #endregion

        #region IsChildSelected Property

        public static readonly DependencyProperty IsChildSelectedProperty = DependencyProperty.Register("IsChildSelected", typeof(bool), typeof(NavigationViewItem), new FrameworkPropertyMetadata(false));

        public bool IsChildSelected
        {
            get
            {
                return (bool)GetValue(IsChildSelectedProperty);
            }
            set
            {
                SetValue(IsChildSelectedProperty, value);
            }
        }

        #endregion

        #region MenuItems Property

        public static readonly DependencyPropertyKey MenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(IList), typeof(NavigationViewItem), new FrameworkPropertyMetadata(OnMenuItemsPropertyChanged));

        public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public IList MenuItems
        {
            get
            {
                return (IList)GetValue(MenuItemsProperty);
            }
            set
            {
                SetValue(MenuItemsPropertyKey, value);
            }
        }

        private static void OnMenuItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewItem navigationViewItem = (NavigationViewItem)d;

            navigationViewItem.OnPropertyChanged(e.Property);
        }

        #endregion

        #region MenuItemsSource Property

        public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(NavigationViewItem), new FrameworkPropertyMetadata(OnMenuItemsSourcePropertyChanged));

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
            NavigationViewItem navigationViewItem = (NavigationViewItem)d;

            navigationViewItem.OnPropertyChanged(e.Property);
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NavigationViewItem), new FrameworkPropertyMetadata());

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


        #region Methods

        static NavigationViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItem), new FrameworkPropertyMetadata(typeof(NavigationViewItem)));
        }

        public NavigationViewItem()
        {
            SetValue(MenuItemsPropertyKey, new ObservableCollection<object>());
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NavigationViewItemAutomationPeer(this);
        }

        public override void OnApplyTemplate()
        {
            // Stop UpdateVisualState before template is applied. Otherwise the visuals may be unexpected
            m_appliedTemplate = false;

            UnhookEventsAndClearFields();

            base.OnApplyTemplate();

            m_helper.Init(this);

            if (GetTemplateChild(c_rootGrid) is Grid rootGrid)
            {
                m_rootGrid = rootGrid;

                if (FlyoutBase.GetAttachedFlyout(rootGrid) is FlyoutBase flyoutBase)
                {
                    flyoutBase.Closing += OnFlyoutClosing;
                }
            }

            HookInputEvents();

            IsEnabledChanged += OnIsEnabledChanged;

            if (GetTemplateChild("ToolTip") is ToolTip toolTip)
            {
                m_toolTip = toolTip;
            }

            if (GetSplitView() is SplitView splitView)
            {
                splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
                splitView.DisplayModeChanged += OnSplitViewPropertyChanged;
                splitView.CompactPaneLengthChanged += OnSplitViewPropertyChanged;

                UpdateCompactPaneLength();
                UpdateIsClosedCompact();
            }

            // Retrieve reference to NavigationView
            if (GetNavigationView() is NavigationView nvImpl)
            {
                if (GetTemplateChild(c_repeater) is ItemsRepeater repeater)
                {
                    m_repeater = repeater;

                    // Primary element setup happens in NavigationView
                    repeater.ElementPrepared += nvImpl.OnRepeaterElementPrepared;
                    repeater.ElementClearing += nvImpl.OnRepeaterElementClearing;

                    repeater.ItemTemplate = nvImpl.GetNavigationViewItemsFactory();
                }

                UpdateRepeaterItemsSource();
            }

            if (GetTemplateChild(c_flyoutContentGrid) is Grid flyoutContentGrid)
            {
                m_flyoutContentGrid = flyoutContentGrid;
            }

            m_appliedTemplate = true;

            UpdateItemIndentation();
            UpdateVisualStateNoTransition();
            ReparentRepeater();

            // We dont want to update the repeater visibilty during OnApplyTemplate if NavigationView is in a mode when items are shown in a flyout
            if (!ShouldRepeaterShowInFlyout())
            {
                ShowHideChildren();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            SuggestedToolTipChanged(newContent);
            UpdateVisualStateNoTransition();

            if (!IsOnLeftNav())
            {
                // Content has changed for the item, so we want to trigger a re-measure
                if (GetNavigationView() is NavigationView navView)
                {
                    navView.TopNavigationViewItemContentChanged();
                }
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            Control originalSource = e.OriginalSource as Control;
            if (originalSource != null && originalSource.IsKeyboardFocused && InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                // It's used to support bluebar have difference appearance between focused and focused+selection. 
                // For example, we can move the SelectionIndicator 3px up when focused and selected to make sure focus rectange doesn't override SelectionIndicator. 
                // If it's a pointer or programatic, no focus rectangle, so no action
                m_hasKeyboardFocus = true;
                UpdateVisualStateNoTransition();
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (m_hasKeyboardFocus)
            {
                m_hasKeyboardFocus = false;
                UpdateVisualStateNoTransition();
            }
        }

        protected override void OnNavigationViewItemBaseDepthChanged()
        {
            UpdateItemIndentation();
            PropagateDepthToChildren(Depth + 1);
        }

        protected override void OnNavigationViewItemBaseIsSelectedChanged()
        {
            UpdateVisualStateForPointer();
        }

        protected override void OnNavigationViewItemBasePositionChanged()
        {
            UpdateVisualStateNoTransition();
            ReparentRepeater();
        }

        /* Property Changed */

        private void OnPropertyChanged(DependencyProperty property)
        {
            if (property == IconProperty)
            {
                OnIconPropertyChanged();
            }
            else if (property == IsExpandedProperty)
            {
                OnIsExpandedPropertyChanged();
                IsExpandedChanged?.Invoke(this, property);
            }
            else if (property == HasUnrealizedChildrenProperty)
            {
                OnHasUnrealizedChildrenPropertyChanged();
            }
            else if (property == MenuItemsProperty)
            {
                OnMenuItemsPropertyChanged();
            }
            else if (property == MenuItemsSourceProperty)
            {
                OnMenuItemsSourcePropertyChanged();
            }
        }

        private void OnIconPropertyChanged()
        {
            UpdateVisualStateNoTransition();
        }

        private void OnIsExpandedPropertyChanged()
        {
            if (FrameworkElementAutomationPeer.FromElement(this) is AutomationPeer peer)
            {
                NavigationViewItemAutomationPeer navViewItemPeer = peer as NavigationViewItemAutomationPeer;
                navViewItemPeer.RaiseExpandCollapseAutomationEvent(IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
            }

            UpdateVisualState(true);
        }

        private void OnHasUnrealizedChildrenPropertyChanged()
        {
            UpdateVisualStateForChevron();
        }

        private void OnMenuItemsPropertyChanged()
        {
            UpdateRepeaterItemsSource();
            UpdateVisualStateForChevron();
        }

        private void OnMenuItemsSourcePropertyChanged()
        {
            UpdateRepeaterItemsSource();
            UpdateVisualStateForChevron();
        }

        /* Update Visual States */

        internal void UpdateVisualStateNoTransition()
        {
            UpdateVisualState(false /*useTransition*/);
        }

        private void UpdateCompactPaneLength()
        {
            if (GetSplitView() is SplitView splitView)
            {
                SetValue(CompactPaneLengthProperty, splitView.CompactPaneLength);

                // Only update when on left
                if (GetPresenter() is NavigationViewItemPresenter presenter)
                {
                    presenter.UpdateCompactPaneLength(splitView.CompactPaneLength, IsOnLeftNav());
                }
            }
        }

        internal void UpdateIsClosedCompact()
        {
            if (GetSplitView() is SplitView splitView)
            {
                // Check if the pane is closed and if the splitview is in either compact mode.
                m_isClosedCompact = !splitView.IsPaneOpen
                    && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);

                UpdateVisualState(true /*useTransitions*/);
                if (GetPresenter() is NavigationViewItemPresenter presenter)
                {
                    presenter.UpdateClosedCompactVisualState(IsTopLevelItem, m_isClosedCompact);
                }

            }
        }

        private void UpdateNavigationViewItemToolTip()
        {
            object toolTipContent = ToolTipService.GetToolTip(this);

            // no custom tooltip, then use suggested tooltip
            if (toolTipContent == null || toolTipContent == m_suggestedToolTipContent)
            {
                if (ShouldEnableToolTip())
                {
                    // Don't SetToolTip with the same parameter because it close/re-open the ToolTip
                    if (toolTipContent != m_suggestedToolTipContent)
                    {
                        ToolTipService.SetToolTip(this, m_suggestedToolTipContent);
                    }
                }
                else
                {
                    ToolTipService.SetToolTip(this, null);
                }
            }
        }

        private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
        {
            if (m_navigationViewItemPresenter != null)
            {
                string stateName = showIcon ? (showContent ? "IconOnLeft" : "IconOnly") : "ContentOnly";
                VisualStateManager.GoToState(m_navigationViewItemPresenter, stateName, false /*useTransitions*/);
                m_navigationViewItemPresenter.NavigationViewIconPositionState = stateName;
            }
        }

        private void UpdateVisualStateForClosedCompact()
        {
            if (m_navigationViewItemPresenter != null)
            {
                m_navigationViewItemPresenter.UpdateClosedCompactVisualState(IsTopLevelItem, m_isClosedCompact);
            }
        }

        private void UpdateVisualStateForNavigationViewPositionChange()
        {
            string stateName = NavigationViewItemHelper.c_OnLeftNavigation;

            switch (Position)
            {
                case NavigationViewRepeaterPosition.LeftNav:
                case NavigationViewRepeaterPosition.LeftFooter:
                    break;
                case NavigationViewRepeaterPosition.TopPrimary:
                case NavigationViewRepeaterPosition.TopFooter:
                    stateName = NavigationViewItemHelper.c_OnTopNavigationPrimary;
                    break;
                case NavigationViewRepeaterPosition.TopOverflow:
                    stateName = NavigationViewItemHelper.c_OnTopNavigationOverflow;
                    break;
            }

            VisualStateManager.GoToState(this, stateName, false /*useTransitions*/);
        }

        private void UpdateVisualStateForKeyboardFocusedState()
        {
            string focusState = "KeyboardNormal";

            if (m_hasKeyboardFocus)
            {
                focusState = "KeyboardFocused";
            }

            VisualStateManager.GoToState(this, focusState, false /*useTransitions*/);
        }

        private void UpdateVisualStateForToolTip()
        {
            // Since RS5, ToolTip apply to NavigationViewItem directly to make Keyboard focus has tooltip too.
            // If ToolTip TemplatePart is detected, fallback to old logic and apply ToolTip on TemplatePart.
            if (m_toolTip != null)
            {
                bool shouldEnableToolTip = ShouldEnableToolTip();
                object toolTipContent = m_suggestedToolTipContent;
                if (shouldEnableToolTip && toolTipContent != null)
                {
                    m_toolTip.Content = toolTipContent;
                    m_toolTip.IsEnabled = true;
                }
                else
                {
                    m_toolTip.Content = null;
                    m_toolTip.IsEnabled = false;
                }
            }
            else
            {
                UpdateNavigationViewItemToolTip();
            }
        }

        private void UpdateVisualStateForPointer()
        {
            string enabledStateValue = IsEnabled ? c_enabled : c_disabled;
            // DisabledStates and CommonStates
            string selectedStateValue = c_normal;

            if (IsEnabled)
            {
                if (IsSelected)
                {
                    if (m_isPressed)
                    {
                        selectedStateValue = c_pressedSelected;
                    }
                    else if (m_isPointerOver)
                    {
                        selectedStateValue = c_pointerOverSelected;
                    }
                    else
                    {
                        selectedStateValue = c_selected;
                    }
                }
                else if (m_isPointerOver)
                {
                    if (m_isPressed)
                    {
                        selectedStateValue = c_pressed;
                    }
                    else
                    {
                        selectedStateValue = c_pointerOver;
                    }
                }
                else if (m_isPressed)
                {
                    selectedStateValue = c_pressed;
                }
            }
            else
            {
                if (IsSelected)
                {
                    selectedStateValue = c_selected;
                }
            }

            // There are scenarios where the presenter may not exist.
            // For example, the top nav settings item. In that case,
            // update the states for the item itself.
            if (m_navigationViewItemPresenter != null)
            {
                m_navigationViewItemPresenter.PointerState = selectedStateValue;
                VisualStateManager.GoToState(m_navigationViewItemPresenter, enabledStateValue, true);
                VisualStateManager.GoToState(m_navigationViewItemPresenter, selectedStateValue, true);
            }
            else
            {
                VisualStateManager.GoToState(this, enabledStateValue, true);
                VisualStateManager.GoToState(this, selectedStateValue, true);
            }
        }

        private void UpdateVisualState(bool useTransitions)
        {
            if (!m_appliedTemplate)
                return;

            UpdateVisualStateForPointer();

            UpdateVisualStateForNavigationViewPositionChange();

            bool shouldShowIcon = ShouldShowIcon();
            bool shouldShowContent = ShouldShowContent();

            if (IsOnLeftNav())
            {
                if (m_navigationViewItemPresenter is NavigationViewItemPresenter presenter)
                {
                    string iconState = shouldShowIcon ? "IconVisible" : "IconCollapsed";
                    // Backward Compatibility with RS4-, new implementation prefer IconOnLeft/IconOnly/ContentOnly
                    VisualStateManager.GoToState(presenter, iconState, useTransitions);
                    presenter.IconState = iconState;
                }
            }

            UpdateVisualStateForToolTip();

            UpdateVisualStateForIconAndContent(shouldShowIcon, shouldShowContent);

            // visual state for focus state. top navigation use it to provide different visual for selected and selected+focused
            UpdateVisualStateForKeyboardFocusedState();

            UpdateVisualStateForChevron();
        }

        private void UpdateVisualStateForChevron()
        {
            if (m_navigationViewItemPresenter != null)
            {
                string chevronState = HasChildren() && !(m_isClosedCompact && ShouldRepeaterShowInFlyout()) ? (IsExpanded ? c_chevronVisibleOpen : c_chevronVisibleClosed) : c_chevronHidden;
                VisualStateManager.GoToState(m_navigationViewItemPresenter, chevronState, true);
                m_navigationViewItemPresenter.ChevronState = chevronState;
            }
        }

        private void UpdateItemIndentation()
        {
            // Update item indentation based on its depth
            if (m_navigationViewItemPresenter != null)
            {
                int newLeftMargin = Depth * c_itemIndentation;
                m_navigationViewItemPresenter.UpdateContentLeftIndentation(newLeftMargin);
            }
        }

        /* Repeater */

        internal bool ShouldRepeaterShowInFlyout()
        {
            return (m_isClosedCompact && IsTopLevelItem) || IsOnTopPrimary();
        }

        internal ItemsRepeater GetRepeater()
        {
            return m_repeater;
        }

        internal bool IsRepeaterVisible()
        {
            if(m_repeater != null)
            {
                return m_repeater.Visibility == Visibility.Visible;
            }

            return false;
        }

        private void ReparentRepeater()
        {
            if (HasChildren() && m_repeater != null)
            {
                if (ShouldRepeaterShowInFlyout() && !m_isRepeaterParentedToFlyout)
                {
                    // Reparent repeater to flyout
                    // TODO: Replace removeatend with something more specific
                    m_rootGrid.Children.Remove(m_repeater);
                    m_flyoutContentGrid.Children.Add(m_repeater);
                    m_isRepeaterParentedToFlyout = true;

                    PropagateDepthToChildren(0);
                }
                else if (!ShouldRepeaterShowInFlyout() && m_isRepeaterParentedToFlyout)
                {
                    m_flyoutContentGrid.Children.Remove(m_repeater);
                    m_rootGrid.Children.Add(m_repeater);
                    m_isRepeaterParentedToFlyout = false;

                    PropagateDepthToChildren(1);
                }
            }
        }

        private void UpdateRepeaterItemsSource()
        {
            if (m_repeater != null)
            {
                object itemsSource;

                if (MenuItemsSource != null)
                {
                    itemsSource = MenuItemsSource;
                }
                else
                {
                    itemsSource = MenuItems;
                }

                if (m_repeater.ItemsSourceView != null)
                {
                    m_repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
                }

                m_repeater.ItemsSource = itemsSource;
                m_repeater.ItemsSourceView.CollectionChanged += OnItemsSourceViewChanged;
            }
        }

        /* Event handlers */

        internal void OnExpandCollapseChevronTapped(object sender, MouseEventArgs args)
        {
            IsExpanded = !IsExpanded;
            args.Handled = true;
        }

        private void OnPresenterPointerReleased(object sender, MouseEventArgs args)
        {
            if (m_isPressed)
            {
                m_isPressed = false;

                if (m_capturedPointer)
                {
                    UIElement presenter = GetPresenterOrItem();

                    presenter.ReleaseMouseCapture();
                }

                UpdateVisualState(true);
            }
        }

        private void OnPresenterPointerExited(object sender, MouseEventArgs args)
        {
            m_isPointerOver = false;

            UpdateVisualState(true);
        }

        private void OnPresenterPointerCaptureLost(object sender, MouseEventArgs args)
        {
            ProcessPointerCanceled(args);
        }

        private void OnPresenterPointerMoved(object sender, MouseEventArgs e)
        {
            ProcessPointerOver(e);
        }

        private void OnPresenterPointerEntered(object sender, MouseEventArgs e)
        {
            ProcessPointerOver(e);
        }

        private void OnPresenterPointerPressed(object sender, MouseButtonEventArgs e)
        {
            m_isPressed = e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

            UIElement presenter = GetPresenterOrItem();

            if (presenter.CaptureMouse())
            {
                m_capturedPointer = true;
            }

            UpdateVisualState(true);
        }

        private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateVisualStateForChevron();
        }

        private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
        {
            if (args == SplitView.CompactPaneLengthProperty)
            {
                UpdateCompactPaneLength();
            }
            else if (args == SplitView.IsPaneOpenProperty || args == SplitView.DisplayModeProperty)
            {
                UpdateIsClosedCompact();
                ReparentRepeater();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                m_isPressed = false;
                m_isPointerOver = false;

                if (m_capturedPointer)
                {
                    UIElement presenter = GetPresenterOrItem();

                    presenter.ReleaseMouseCapture();
                    m_capturedPointer = false;
                }
            }

            UpdateVisualState(true);
        }

        private void OnFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            IsExpanded = false;
        }

        /* *** */

        internal UIElement GetSelectionIndicator()
        {
            UIElement selectIndicator = m_helper.GetSelectionIndicator();
            if (GetPresenter() is NavigationViewItemPresenter presenter)
            {
                selectIndicator = presenter.GetSelectionIndicator();
            }
            return selectIndicator;
        }

        internal void RotateExpandCollapseChevron(bool isExpanded)
        {
            if (GetPresenter() is NavigationViewItemPresenter presenter)
            {
                presenter.RotateExpandCollapseChevron(isExpanded);
            }
        }

        private NavigationViewItemPresenter GetPresenter()
        {
            return m_navigationViewItemPresenter;
        }

        internal void PropagateDepthToChildren(int depth)
        {
            if (m_repeater is ItemsRepeater repeater)
            {
                int itemsCount = repeater.ItemsSourceView.Count;
                for (int index = 0; index < itemsCount; index++)
                {
                    if (repeater.TryGetElement(index) is UIElement element)
                    {
                        if (element is NavigationViewItemBase nvib)
                        {
                            nvib.Depth = depth;
                        }
                    }
                }
            }
        }

        internal void ShowHideChildren()
        {
            if (m_repeater != null)
            {
                bool shouldShowChildren = IsExpanded;
                Visibility visibility = shouldShowChildren ? Visibility.Visible : Visibility.Collapsed;
                m_repeater.Visibility = visibility;

                if (ShouldRepeaterShowInFlyout())
                {
                    if (shouldShowChildren)
                    {
                        // Verify that repeater is parented correctly
                        if (!m_isRepeaterParentedToFlyout)
                        {
                            ReparentRepeater();
                        }

                        // There seems to be a race condition happening which sometimes
                        // prevents the opening of the flyout. Queue callback as a workaround.
                        //SharedHelpers.QueueCallbackForCompositionRendering(() =>
                        //{
                        //    FlyoutBase.ShowAttachedFlyout(m_rootGrid);
                        //});

                        FlyoutBase.ShowAttachedFlyout(m_rootGrid);
                    }
                    else
                    {
                        if (FlyoutBase.GetAttachedFlyout(m_rootGrid) is FlyoutBase flyout)
                        {
                            flyout.Hide();
                        }
                    }
                }
            }
        }

        private bool ShouldEnableToolTip()
        {
            // We may enable Tooltip for IconOnly in the future, but not now
            return IsOnLeftNav() && m_isClosedCompact;
        }

        private bool ShouldShowIcon()
        {
            return Icon != null;
        }

        private bool ShouldShowContent()
        {
            return Content != null;
        }

        private void SuggestedToolTipChanged(object newContent)
        {
            object potentialString = newContent;
            bool stringableToolTip = (potentialString != null && potentialString is string);

            object newToolTipContent = null;
            if (stringableToolTip)
            {
                newToolTipContent = newContent;
            }

            // Both customer and NavigationViewItem can update ToolTipContent by ToolTipService.SetToolTip or XAML
            // If the ToolTipContent is not the same as m_suggestedToolTipContent, then it's set by customer.
            // Customer's ToolTip take high priority, and we never override Customer's ToolTip.
            object toolTipContent = ToolTipService.GetToolTip(this);
            if (m_suggestedToolTipContent is object oldToolTipContent)
            {
                if (oldToolTipContent == toolTipContent)
                {
                    ToolTipService.SetToolTip(this, null);
                }
            }

            m_suggestedToolTipContent = newToolTipContent;
        }

        private UIElement GetPresenterOrItem()
        {
            if (m_navigationViewItemPresenter != null)
            {
                return m_navigationViewItemPresenter;
            }
            else
            {
                return this;
            }
        }

        private void ProcessPointerCanceled(RoutedEventArgs args)
        {
            m_isPressed = false;
            m_isPointerOver = false;
            m_capturedPointer = false;

            UpdateVisualState(true);
        }

        private void ProcessPointerOver(RoutedEventArgs args)
        {
            if (!m_isPointerOver)
            {
                m_isPointerOver = true;
                UpdateVisualState(true);
            }
        }

        /* *** */

        internal bool HasChildren()
        {
            return MenuItems.Count > 0
                || (MenuItemsSource != null && m_repeater != null && m_repeater.ItemsSourceView.Count > 0)
                || HasUnrealizedChildren;
        }

        private bool IsOnTopPrimary()
        {
            return Position == NavigationViewRepeaterPosition.TopPrimary;
        }

        private bool IsOnLeftNav()
        {
            return Position == NavigationViewRepeaterPosition.LeftNav || Position == NavigationViewRepeaterPosition.LeftFooter;
        }

        /* *** */

        private void HookInputEvents()
        {
            UIElement presenter;

            if(GetTemplateChild(c_navigationViewItemPresenterName) is NavigationViewItemPresenter itemPresenter)
            {
                m_navigationViewItemPresenter = itemPresenter;

                presenter = itemPresenter;
            }
            else
            {
                presenter = this;
            }

            // Handlers that set flags are skipped when args.Handled is already True.
            presenter.MouseDown += OnPresenterPointerPressed;
            presenter.MouseEnter += OnPresenterPointerEntered;
            presenter.MouseMove += OnPresenterPointerMoved;

            // Handlers that reset flags are not skipped when args.Handled is already True to avoid broken states.
            presenter.AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnPresenterPointerReleased), true /*handledEventsToo*/);
            presenter.AddHandler(MouseLeaveEvent, new MouseEventHandler(OnPresenterPointerExited), true /*handledEventsToo*/);
            presenter.AddHandler(LostMouseCaptureEvent, new MouseEventHandler(OnPresenterPointerCaptureLost), true /*handledEventsToo*/);
        }

        private void UnhookInputEvents()
        {
            UIElement presenter = m_navigationViewItemPresenter as UIElement;

            if (presenter == null)
            {
                presenter = this as UIElement;
            }

            presenter.MouseDown -= OnPresenterPointerPressed;
            presenter.MouseEnter -= OnPresenterPointerEntered;
            presenter.MouseMove -= OnPresenterPointerMoved;

            presenter.RemoveHandler(MouseUpEvent, new MouseButtonEventHandler(OnPresenterPointerReleased));
            presenter.RemoveHandler(MouseLeaveEvent, new MouseEventHandler(OnPresenterPointerExited));
            presenter.RemoveHandler(LostMouseCaptureEvent, new MouseEventHandler(OnPresenterPointerCaptureLost));
        }

        private void UnhookEventsAndClearFields()
        {
            UnhookInputEvents();

            if (m_rootGrid != null && FlyoutBase.GetAttachedFlyout(m_rootGrid) is FlyoutBase flyoutBase)
            {
                flyoutBase.Closing -= OnFlyoutClosing;
            }

            if (GetSplitView() is SplitView splitView)
            {
                splitView.IsPaneOpenChanged -= OnSplitViewPropertyChanged;
                splitView.DisplayModeChanged -= OnSplitViewPropertyChanged;
                splitView.CompactPaneLengthChanged -= OnSplitViewPropertyChanged;
            }

            if (m_repeater != null&& GetNavigationView() is NavigationView nvImpl)
            {
                m_repeater.ElementPrepared -= nvImpl.OnRepeaterElementPrepared;
                m_repeater.ElementClearing -= nvImpl.OnRepeaterElementClearing;
                m_repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
            }

            IsEnabledChanged -= OnIsEnabledChanged;

            m_rootGrid = null;
            m_navigationViewItemPresenter = null;
            m_toolTip = null;
            m_repeater = null;
            m_flyoutContentGrid = null;
        }

        #endregion

        private ToolTip m_toolTip;
        NavigationViewItemHelper<NavigationViewItem> m_helper = new NavigationViewItemHelper<NavigationViewItem>();
        private NavigationViewItemPresenter m_navigationViewItemPresenter;
        private object m_suggestedToolTipContent;
        private ItemsRepeater m_repeater;
        private Grid m_flyoutContentGrid;
        private Grid m_rootGrid;
        private bool m_isClosedCompact;
        private bool m_appliedTemplate;
        private bool m_hasKeyboardFocus;

        // Visual state tracking
        private bool m_capturedPointer = false;

        private bool m_isPressed;
        private bool m_isPointerOver;
        private bool m_isRepeaterParentedToFlyout;
    }
}
