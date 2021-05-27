// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public enum AnimationContext
    {
        None = 0,
        CollectionChangeAdd = 1,
        CollectionChangeRemove = 2,
        CollectionChangeReset = 4,
        LayoutTransition = 8,
    }

    public enum ElementRealizationOptions
    {
        None = 0,
        ForceCreate = 1,
        SuppressAutoRecycle = 2
    }

    public enum FlowLayoutLineAlignment
    {
        Start = 0,
        Center = 1,
        End = 2,
        SpaceAround = 3,
        SpaceBetween = 4,
        SpaceEvenly = 5
    }

    public enum UniformGridLayoutItemsJustification
    {
        Start = 0,
        Center = 1,
        End = 2,
        SpaceAround = 3,
        SpaceBetween = 4,
        SpaceEvenly = 5
    }

    public enum UniformGridLayoutItemsStretch
    {
        None = 0,
        Fill = 1,
        Uniform = 2
    }

    public interface IKeyIndexMapping
    {
        string KeyFromIndex(int index);
        int IndexFromKey(string key);
    }

    public interface IElementFactoryShim
    {
        UIElement GetElement(ElementFactoryGetArgs args);
        void RecycleElement(ElementFactoryRecycleArgs args);
    }

    [ContentProperty("ItemTemplate")]
    public class ItemsRepeater : Panel
    {
        public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> ElementClearing;
        public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementIndexChangedEventArgs> ElementIndexChanged;
        public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementPreparedEventArgs> ElementPrepared;

        #region ItemsSource Property

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(ItemsRepeater), new FrameworkPropertyMetadata(OnItemsSourcePropertyChanged));

        public object ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            if (e.NewValue != e.OldValue)
            {
                object newValue = e.NewValue;
                ItemsSourceView newDataSource = newValue as ItemsSourceView;
                if (newValue != null && newDataSource == null)
                {
                    newDataSource = new InspectingDataSource(newValue);
                }

                repeater.OnDataSourcePropertyChanged(repeater.ItemsSourceView, newDataSource);
            }
        }

        #endregion

        #region ItemsSourceView Property

        public ItemsSourceView ItemsSourceView { get; private set; }

        #endregion

        #region ItemTemplate Property

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(object), typeof(ItemsRepeater), new FrameworkPropertyMetadata(OnItemTemplatePropertyChanged));

        public object ItemTemplate
        {
            get
            {
                return GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            repeater.OnItemTemplateChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region Layout Property

        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(ItemsRepeater), new FrameworkPropertyMetadata(OnLayoutPropertyChanged));

        public Layout Layout
        {
            get
            {
                return (Layout)GetValue(LayoutProperty);
            }
            set
            {
                SetValue(LayoutProperty, value);
            }
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            repeater.OnLayoutChanged(e.OldValue as Layout, e.NewValue as Layout);
        }

        #endregion

        #region Animator Property

        public static readonly DependencyProperty AnimatorProperty = DependencyProperty.Register("Animator", typeof(ElementAnimator), typeof(ItemsRepeater), new FrameworkPropertyMetadata(OnAnimatorPropertyChanged));

        public ElementAnimator Animator
        {
            get
            {
                return (ElementAnimator)GetValue(AnimatorProperty);
            }
            set
            {
                SetValue(AnimatorProperty, value);
            }
        }

        private static void OnAnimatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            repeater.OnAnimatorChanged(e.OldValue as ElementAnimator, e.NewValue as ElementAnimator);
        }

        #endregion

        #region HorizontalCacheLength Property

        public static readonly DependencyProperty HorizontalCacheLengthProperty = DependencyProperty.Register("HorizontalCacheLength", typeof(double), typeof(ItemsRepeater), new FrameworkPropertyMetadata(2.0, OnHorizontalCacheLengthPropertyChanged));

        public double HorizontalCacheLength
        {
            get
            {
                return (double)GetValue(HorizontalCacheLengthProperty);
            }
            set
            {
                SetValue(HorizontalCacheLengthProperty, value);
            }
        }

        private static void OnHorizontalCacheLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            repeater.m_viewportManager.HorizontalCacheLength = (double)e.NewValue;
        }

        #endregion

        #region VerticalCacheLength Property

        public static readonly DependencyProperty VerticalCacheLengthProperty = DependencyProperty.Register("VerticalCacheLength", typeof(double), typeof(ItemsRepeater), new FrameworkPropertyMetadata(2.0, OnVerticalCacheLengthPropertyChanged));

        public double VerticalCacheLength
        {
            get
            {
                return (double)GetValue(VerticalCacheLengthProperty);
            }
            set
            {
                SetValue(VerticalCacheLengthProperty, value);
            }
        }

        private static void OnVerticalCacheLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsRepeater repeater = d as ItemsRepeater;

            repeater.VerticalCacheLength = (double)e.NewValue;
        }

        #endregion


        #region Static Properties

        public static Point ClearedElementsArrangePosition { get; set; } = new Point(-10000.0, -10000.0);

        public static Rect InvalidRect { get; set; } = Rect.Empty;

        #endregion


        #region Methods

        static ItemsRepeater()
        {

        }

        public ItemsRepeater()
        {
            ViewManager = new ViewManager(this);
            AnimationManager = new AnimationManager(this);
            m_viewportManager = new ViewportManagerDownlevel(this);

            SetCurrentValue(LayoutProperty, new StackLayout());

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            var layout = Layout as VirtualizingLayout;
            OnLayoutChanged(null, layout);
        }

        ~ItemsRepeater()
        {
            m_itemTemplate = null;
            m_animator = null;
            m_layout = null;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RepeaterAutomationPeer(this);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (m_isLayoutInProgress)
            {
                throw new Exception("Reentrancy detected during layout.");
            }

            if (IsProcessingCollectionChange)
            {
                throw new Exception("Cannot run layout in the middle of a collection change.");
            }

            m_viewportManager.OnOwnerMeasuring();

            m_isLayoutInProgress = true;

            ViewManager.PrunePinnedElements();
            Rect extent = default;
            Size desiredSize = default;

            try
            {
                if (Layout is Layout layout)
                {
                    VirtualizingLayoutContext layoutContext = GetLayoutContext();

                    // Checking if we have an data template and it is empty
                    if (m_isItemTemplateEmpty)
                    {
                        // Has no content, so we will draw nothing and request zero space
                        extent = new Rect(LayoutOrigin.X, LayoutOrigin.Y, 0, 0);
                    }
                    else
                    {
                        desiredSize = layout.Measure(layoutContext, availableSize);
                        extent = new Rect(LayoutOrigin.X, LayoutOrigin.Y, desiredSize.Width, desiredSize.Height);
                    }

                    // Clear auto recycle candidate elements that have not been kept alive by layout - i.e layout did not
                    // call GetElementAt(index).
                    UIElementCollection children = Children;
                    for (int i = 0; i < children.Count; ++i)
                    {
                        UIElement element = children[i];
                        VirtualizationInfo virtInfo = GetVirtualizationInfo(element);

                        if (virtInfo.Owner == ElementOwner.Layout &&
                            virtInfo.AutoRecycleCandidate &&
                            !virtInfo.KeepAlive)
                        {
                            // REPEATER_TRACE_INFO(L"AutoClear - %d \n", virtInfo->Index());
                            ClearElementImpl(element);
                        }
                    }
                }
            }
            finally
            {
                m_isLayoutInProgress = false;
            }

            m_viewportManager.SetLayoutExtent(extent);
            m_lastAvailableSize = availableSize;
            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (m_isLayoutInProgress)
            {
                throw new Exception("Reentrancy detected during layout.");
            }

            if (IsProcessingCollectionChange)
            {
                throw new Exception("Cannot run layout in the middle of a collection change.");
            }

            m_isLayoutInProgress = true;

            Size arrangeSize = Size.Empty;

            try
            {
                if (Layout is Layout layout)
                {
                    arrangeSize = layout.Arrange(GetLayoutContext(), finalSize);
                }

                // The view manager might clear elements during this call.
                // That's why we call it before arranging cleared elements
                // off screen.
                ViewManager.OnOwnerArranged();

                UIElementCollection children = Children;

                for (int i = 0; i < children.Count; ++i)
                {
                    UIElement element = children[i];
                    VirtualizationInfo virtInfo = GetVirtualizationInfo(element);
                    virtInfo.KeepAlive = false;

                    if (virtInfo.Owner == ElementOwner.ElementFactory || virtInfo.Owner == ElementOwner.PinnedPool)
                    {
                        // Toss it away. And arrange it with size 0 so that XYFocus won't use it.
                        element.Arrange(new Rect(
                            ClearedElementsArrangePosition.X - element.DesiredSize.Width,
                            ClearedElementsArrangePosition.Y - element.DesiredSize.Height,
                            0.0,
                            0.0));
                    }
                    else
                    {
                        Rect newBounds = CachedVisualTreeHelpers.GetLayoutSlot(element as FrameworkElement);

                        if (virtInfo.ArrangeBounds != ItemsRepeater.InvalidRect &&
                            newBounds != virtInfo.ArrangeBounds)
                        {
                            AnimationManager.OnElementBoundsChanged(element, virtInfo.ArrangeBounds, newBounds);
                        }

                        virtInfo.ArrangeBounds = newBounds;
                    }
                }
            }
            finally
            {
                m_isLayoutInProgress = false;
            }

            m_viewportManager.OnOwnerArranged();
            AnimationManager.OnOwnerArranged();

            return arrangeSize;
        }

        // Mapping APIs
        internal int GetElementIndex(UIElement element)
        {
            return GetElementIndexImpl(element);
        }

        internal UIElement TryGetElement(int index)
        {
            return GetElementFromIndexImpl(index);
        }

        internal UIElement GetOrCreateElement(int index)
        {
            return GetOrCreateElementImpl(index);
        }

        internal IElementFactoryShim ItemTemplateShim { get; private set; }

        internal ViewManager ViewManager { get; private set; }

        internal AnimationManager AnimationManager { get; private set; }

        internal UIElement GetElementImpl(int index, bool forceCreate, bool suppressAutoRecycle)
        {
            UIElement element = ViewManager.GetElement(index, forceCreate, suppressAutoRecycle);
            return element;
        }

        internal void ClearElementImpl(UIElement element)
        {
            // Clearing an element due to a collection change
            // is more strict in that pinned elements will be forcibly
            // unpinned and sent back to the view generator.
            bool isClearedDueToCollectionChange =
                IsProcessingCollectionChange &&
                (m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Remove ||
                    m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Replace ||
                    m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Reset);

            ViewManager.ClearElement(element, isClearedDueToCollectionChange);
            m_viewportManager.OnElementCleared(element);
        }

        // Mapping APIs (exception based)
        internal int GetElementIndexImpl(UIElement element)
        {
            // Verify that element is actually a child of this ItemsRepeater
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == this)
            {
                VirtualizationInfo virtInfo = TryGetVirtualizationInfo(element);
                return ViewManager.GetElementIndex(virtInfo);
            }
            return -1;
        }

        internal UIElement GetElementFromIndexImpl(int index)
        {
            UIElement result = null;

            UIElementCollection children = Children;
            for (int i = 0; i < children.Count && result == null; ++i)
            {
                UIElement element = children[i];
                VirtualizationInfo virtInfo = TryGetVirtualizationInfo(element);
                if (virtInfo != null && virtInfo.IsRealized && virtInfo.Index == index)
                {
                    result = element;
                }
            }

            return result;
        }

        internal UIElement GetOrCreateElementImpl(int index)
        {
            if (index >= 0 && index >= ItemsSourceView.Count)
            {
                throw new ArgumentException("Argument index is invalid.");
            }

            if (m_isLayoutInProgress)
            {
                throw new Exception("GetOrCreateElement invocation is not allowed during layout.");
            }

            UIElement element = GetElementFromIndexImpl(index);
            bool isAnchorOutsideRealizedRange = element == null;

            if (isAnchorOutsideRealizedRange)
            {
                if (Layout == null)
                {
                    throw new Exception("Cannot make an Anchor when there is no attached layout.");
                }

                element = GetLayoutContext().GetOrCreateElementAt(index);
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            m_viewportManager.OnMakeAnchor(element, isAnchorOutsideRealizedRange);
            InvalidateMeasure();

            return element;
        }

        /*static*/
        internal static VirtualizationInfo TryGetVirtualizationInfo(UIElement element)
        {
            object value = element.GetValue(VirtualizationInfoProperty);
            return value as VirtualizationInfo;
        }

        /*static*/
        internal static VirtualizationInfo GetVirtualizationInfo(UIElement element)
        {
            VirtualizationInfo result = TryGetVirtualizationInfo(element);

            if (result == null)
            {
                result = CreateAndInitializeVirtualizationInfo(element);
            }

            return result;
        }

        /* static */
        internal static VirtualizationInfo CreateAndInitializeVirtualizationInfo(UIElement element)
        {
            // MUX_ASSERT(!TryGetVirtualizationInfo(element));
            VirtualizationInfo result = new VirtualizationInfo();
            element.SetValue(VirtualizationInfoProperty, result);
            return result;
        }

        internal object LayoutState { get; set; }

        internal Rect VisibleWindow
        {
            get
            {
                return m_viewportManager.GetLayoutVisibleWindow();
            }
        }

        internal Rect RealizationWindow
        {
            get
            {
                return m_viewportManager.GetLayoutRealizationWindow();
            }
        }

        internal UIElement SuggestedAnchor
        {
            get
            {
                return m_viewportManager.SuggestedAnchor;
            }
        }

        internal UIElement MadeAnchor
        {
            get
            {
                return m_viewportManager.MadeAnchor;
            }
        }

        internal Point LayoutOrigin { get; set; }

        // Pinning APIs
        internal void PinElement(UIElement element)
        {
            ViewManager.UpdatePin(element, true /* addPin */);
        }

        internal void UnpinElement(UIElement element)
        {
            ViewManager.UpdatePin(element, false /* addPin */);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        internal void OnElementPrepared(UIElement element, int index)
        {
            m_viewportManager.OnElementPrepared(element);
            if (ElementPrepared != null)
            {
                if (m_elementPreparedArgs == null)
                {
                    m_elementPreparedArgs = new ItemsRepeaterElementPreparedEventArgs(element, index);
                }
                else
                {
                    m_elementPreparedArgs.Update(element, index);
                }

                ElementPrepared?.Invoke(this, m_elementPreparedArgs);
            }
        }

        internal void OnElementClearing(UIElement element)
        {
            if (ElementClearing != null)
            {
                if (m_elementClearingArgs == null)
                {
                    m_elementClearingArgs = new ItemsRepeaterElementClearingEventArgs(element);
                }
                else
                {
                    m_elementClearingArgs.Update(element);
                }

                ElementClearing?.Invoke(this, m_elementClearingArgs);
            }
        }

        internal void OnElementIndexChanged(UIElement element, int oldIndex, int newIndex)
        {
            if (ElementIndexChanged != null)
            {
                if (m_elementIndexChangedArgs == null)
                {
                    m_elementIndexChangedArgs = new ItemsRepeaterElementIndexChangedEventArgs(element, oldIndex, newIndex);
                }
                else
                {
                    m_elementIndexChangedArgs.Update(element, oldIndex, newIndex);
                }
                ElementIndexChanged?.Invoke(this, m_elementIndexChangedArgs);
            }
        }

        internal static readonly DependencyProperty VirtualizationInfoProperty =
            DependencyProperty.RegisterAttached(
                "VirtualizationInfo",
                typeof(VirtualizationInfo),
                typeof(ItemsRepeater),
                null);

        internal int Indent()
        {
            return 4;
        }

        private void OnLoaded(object sender /*sender*/, RoutedEventArgs args /*args*/)
        {
            // If we skipped an unload event, reset the scrollers now and invalidate measure so that we get a new
            // layout pass during which we will hookup new scrollers.
            if (_loadedCounter > _unloadedCounter)
            {
                InvalidateMeasure();
                m_viewportManager.ResetScrollers();
            }
            ++_loadedCounter;
        }

        private void OnUnloaded(object sender /*sender*/, RoutedEventArgs args /*args*/)
        {
            ++_unloadedCounter;
            // Only reset the scrollers if this unload event is in-sync.
            if (_unloadedCounter == _loadedCounter)
            {
                m_viewportManager.ResetScrollers();
            }
        }

        private void OnDataSourcePropertyChanged(ItemsSourceView oldValue, ItemsSourceView newValue)
        {
            if (m_isLayoutInProgress)
            {
                throw new Exception("Cannot set ItemsSourceView during layout.");
            }

            ItemsSourceView = newValue;

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnItemsSourceViewChanged;
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += OnItemsSourceViewChanged;
            }

            bool processingChange = false;

            try
            {
                if (Layout is Layout layout)
                {
                    NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

                    m_processingItemsSourceChange = args;
                    processingChange = true;

                    if (layout is VirtualizingLayout virtualLayout)
                    {
                        ((IVirtualizingLayoutOverrides)virtualLayout).OnItemsChangedCore(GetLayoutContext(), newValue, args);
                    }
                    else if (layout is NonVirtualizingLayout nonVirtualLayout)
                    {
                        // Walk through all the elements and make sure they are cleared for
                        // non-virtualizing layouts.
                        foreach (UIElement element in Children)
                        {
                            if (GetVirtualizationInfo(element).IsRealized)
                            {
                                ClearElementImpl(element);
                            }
                        }

                        Children.Clear();
                    }

                    InvalidateMeasure();
                }
            }
            finally
            {
                if (processingChange)
                {
                    m_processingItemsSourceChange = null;
                }
            }
        }

        private void OnItemTemplateChanged(object oldValue, object newValue)
        {
            if (m_isLayoutInProgress && oldValue != null)
            {
                throw new Exception("ItemTemplate cannot be changed during layout.");
            }

            bool processingChange = false;

            // Since the ItemTemplate has changed, we need to re-evaluate all the items that
            // have already been created and are now in the tree. The easiest way to do that
            // would be to do a reset.. Note that this has to be done before we change the template
            // so that the cleared elements go back into the old template.
            try
            {
                if (Layout is Layout layout)
                {
                    NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

                    m_processingItemsSourceChange = args;
                    processingChange = true;

                    if (layout is VirtualizingLayout virtualLayout)
                    {
                        ((IVirtualizingLayoutOverrides)virtualLayout).OnItemsChangedCore(GetLayoutContext(), newValue, args);
                    }
                    else if (layout is NonVirtualizingLayout nonVirtualLayout)
                    {
                        // Walk through all the elements and make sure they are cleared for
                        // non-virtualizing layouts.
                        foreach (UIElement child in Children)
                        {
                            if (GetVirtualizationInfo(child).IsRealized)
                            {
                                ClearElementImpl(child);
                            }
                        }
                    }
                }

                m_itemTemplate = newValue;

                // Clear flag for bug #776
                m_isItemTemplateEmpty = false;
                ItemTemplateShim = newValue as IElementFactoryShim;

                if (ItemTemplateShim == null)
                {
                    // ItemTemplate set does not implement IElementFactoryShim. We also 
                    // want to support DataTemplate and DataTemplateSelectors automagically.
                    if (newValue is DataTemplate dataTemplate)
                    {
                        ItemTemplateShim = new ItemTemplateWrapper(dataTemplate);

                        if (dataTemplate.LoadContent() is FrameworkElement content)
                        {
                            // Due to bug https://github.com/microsoft/microsoft-ui-xaml/issues/3057, we need to get the framework
                            // to take ownership of the extra implicit ref that was returned by LoadContent. The simplest way to do
                            // this is to add it to a Children collection and immediately remove it.
                            //auto children = Children();
                            //children.Append(content);
                            //children.RemoveAtEnd();
                        }
                        else
                        {
                            // We have a DataTemplate which is empty, so we need to set it to true
                            m_isItemTemplateEmpty = true;
                        }
                    }
                    else if (newValue is DataTemplateSelector selector)
                    {
                        ItemTemplateShim = new ItemTemplateWrapper(selector);
                    }
                    else
                    {
                        throw new ArgumentException("ItemTemplate");
                    }
                }

                InvalidateMeasure();
            }
            finally
            {
                if (processingChange)
                {
                    m_processingItemsSourceChange = null;
                }
            }
        }

        private void OnLayoutChanged(Layout oldValue, Layout newValue)
        {
            if (m_isLayoutInProgress)
            {
                throw new Exception("Layout cannot be changed during layout.");
            }

            ViewManager.OnLayoutChanging();
            AnimationManager.OnLayoutChanging();

            if (oldValue != null)
            {
                oldValue.UninitializeForContext(GetLayoutContext());
                oldValue.MeasureInvalidated -= InvalidateMeasureForLayout;
                oldValue.ArrangeInvalidated -= InvalidateArrangeForLayout;

                // Walk through all the elements and make sure they are cleared
                UIElementCollection children = Children;
                for (int i = 0; i < children.Count; ++i)
                {
                    UIElement element = children[i];
                    if (GetVirtualizationInfo(element).IsRealized)
                    {
                        ClearElementImpl(element);
                    }
                }

                LayoutState = null;
            }

            // Bug in framework's reference tracking causes crash during
            // UIAffinityQueue cleanup. To avoid that bug, take a strong ref
            m_layout = newValue;

            if (newValue != null)
            {
                newValue.InitializeForContext(GetLayoutContext());
                newValue.MeasureInvalidated += InvalidateMeasureForLayout;
                newValue.ArrangeInvalidated += InvalidateArrangeForLayout;
            }

            bool isVirtualizingLayout = newValue != null && newValue as VirtualizingLayout != null;
            m_viewportManager.OnLayoutChanged(isVirtualizingLayout);
            InvalidateMeasure();
        }

        private void OnAnimatorChanged(ElementAnimator oldValue, ElementAnimator newValue)
        {
            AnimationManager.OnAnimatorChanged(newValue);

            m_animator = newValue;
        }

        private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (m_isLayoutInProgress)
            {
                // Bad things will follow if the data changes while we are in the middle of a layout pass.
                throw new Exception("Changes in data source are not allowed during layout.");
            }

            if (IsProcessingCollectionChange)
            {
                throw new Exception("Changes in the data source are not allowed during another change in the data source.");
            }

            m_processingItemsSourceChange = args;

            try
            {
                AnimationManager.OnItemsSourceChanged(sender, args);
                ViewManager.OnItemsSourceChanged(sender, args);

                if (Layout is Layout layout)
                {
                    if (layout is VirtualizingLayout virtualLayout)
                    {
                        ((IVirtualizingLayoutOverrides)virtualLayout).OnItemsChangedCore(GetLayoutContext(), sender, args);
                    }
                    else
                    {
                        // NonVirtualizingLayout
                        InvalidateMeasure();
                    }
                }
            }
            finally
            {
                m_processingItemsSourceChange = null;
            }
        }

        private void InvalidateMeasureForLayout(object sender, object args)
        {
            InvalidateMeasure();
        }

        private void InvalidateArrangeForLayout(object sender, object args)
        {
            InvalidateArrange();
        }

        private VirtualizingLayoutContext GetLayoutContext()
        {
            if (m_layoutContext == null)
            {
                m_layoutContext = new RepeaterLayoutContext(this);
            }
            return m_layoutContext;
        }

        private bool IsProcessingCollectionChange
        {
            get
            {
                return m_processingItemsSourceChange != null;
            }
        }

        #endregion

        private VirtualizingLayoutContext m_layoutContext = null;
        private ViewportManager m_viewportManager;

        // Value is different from null only while we are on the OnItemsSourceChanged call stack.
        private NotifyCollectionChangedEventArgs m_processingItemsSourceChange;

        private Size m_lastAvailableSize;
        private bool m_isLayoutInProgress = false;

        // Cached Event args to avoid creation cost every time
        private ItemsRepeaterElementPreparedEventArgs m_elementPreparedArgs;
        private ItemsRepeaterElementClearingEventArgs m_elementClearingArgs;
        private ItemsRepeaterElementIndexChangedEventArgs m_elementIndexChangedArgs;

        // Loaded events fire on the first tick after an element is put into the tree 
        // while unloaded is posted on the UI tree and may be processed out of sync with subsequent loaded
        // events. We keep these counters to detect out-of-sync unloaded events and take action to rectify.
        private int _loadedCounter;
        private int _unloadedCounter;

        private object m_itemTemplate = null;
        private Layout m_layout = null;
        private ElementAnimator m_animator = null;

        // Bug where DataTemplate with no content causes a crash.
        // See: https://github.com/microsoft/microsoft-ui-xaml/issues/776
        // Solution: Have flag that is only true when DataTemplate exists but it is empty.
        private bool m_isItemTemplateEmpty = false;
    }
}
