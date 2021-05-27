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

namespace EasyWPFUI.Controls
{
    public class UniformGridLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
    {
        #region Orientation property

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformGridLayout), new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

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
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            Orientation orientation = (Orientation)e.NewValue;

            ScrollOrientation scrollOrientation = (orientation == Orientation.Horizontal) ? ScrollOrientation.Vertical : ScrollOrientation.Horizontal;
            uniformGridLayout.m_orientationBasedMeasures.ScrollOrientation = scrollOrientation;
        }

        #endregion

        #region MinItemWidth property

        public static readonly DependencyProperty MinItemWidthProperty = DependencyProperty.Register("MinItemWidth", typeof(double), typeof(UniformGridLayout), new FrameworkPropertyMetadata(0.0, OnMinItemWidthPropertyChanged));

        public double MinItemWidth
        {
            get
            {
                return (double)GetValue(MinItemWidthProperty);
            }
            set
            {
                SetValue(MinItemWidthProperty, value);
            }
        }

        private static void OnMinItemWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_minItemWidth = (double)e.NewValue;
        }

        #endregion

        #region MinItemHeight property

        public static readonly DependencyProperty MinItemHeightProperty = DependencyProperty.Register("MinItemHeight", typeof(double), typeof(UniformGridLayout), new FrameworkPropertyMetadata(0.0, OnMinItemHeightPropertyChanged));

        public double MinItemHeight
        {
            get
            {
                return (double)GetValue(MinItemHeightProperty);
            }
            set
            {
                SetValue(MinItemHeightProperty, value);
            }
        }

        private static void OnMinItemHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_minItemHeight = (double)e.NewValue;
        }

        #endregion

        #region MinRowSpacing property

        public static readonly DependencyProperty MinRowSpacingProperty = DependencyProperty.Register("MinRowSpacing", typeof(double), typeof(UniformGridLayout), new FrameworkPropertyMetadata(0.0, OnMinRowSpacingPropertyChanged));

        public double MinRowSpacing
        {
            get
            {
                return (double)GetValue(MinRowSpacingProperty);
            }
            set
            {
                SetValue(MinRowSpacingProperty, value);
            }
        }

        private static void OnMinRowSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_minRowSpacing = (double)e.NewValue;
        }

        #endregion

        #region MinColumnSpacing property

        public static readonly DependencyProperty MinColumnSpacingProperty = DependencyProperty.Register("MinColumnSpacing", typeof(double), typeof(UniformGridLayout), new FrameworkPropertyMetadata(0.0, OnMinColumnSpacingPropertyChanged));

        public double MinColumnSpacing
        {
            get
            {
                return (double)GetValue(MinColumnSpacingProperty);
            }
            set
            {
                SetValue(MinColumnSpacingProperty, value);
            }
        }

        private static void OnMinColumnSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_minColumnSpacing = (double)e.NewValue;
        }

        #endregion

        #region ItemsJustification property

        public static readonly DependencyProperty ItemsJustificationProperty = DependencyProperty.Register("ItemsJustification", typeof(UniformGridLayoutItemsJustification), typeof(UniformGridLayout), new FrameworkPropertyMetadata(UniformGridLayoutItemsJustification.Start, OnItemsJustificationPropertyChanged));

        public UniformGridLayoutItemsJustification ItemsJustification
        {
            get
            {
                return (UniformGridLayoutItemsJustification)GetValue(ItemsJustificationProperty);
            }
            set
            {
                SetValue(ItemsJustificationProperty, value);
            }
        }

        private static void OnItemsJustificationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_itemsJustification = (UniformGridLayoutItemsJustification)e.NewValue;
        }

        #endregion

        #region ItemsStretch property

        public static readonly DependencyProperty ItemsStretchProperty = DependencyProperty.Register("ItemsStretch", typeof(UniformGridLayoutItemsStretch), typeof(UniformGridLayout), new FrameworkPropertyMetadata(UniformGridLayoutItemsStretch.None, OnItemsStretchPropertyChanged));

        public UniformGridLayoutItemsStretch ItemsStretch
        {
            get
            {
                return (UniformGridLayoutItemsStretch)GetValue(ItemsStretchProperty);
            }
            set
            {
                SetValue(ItemsStretchProperty, value);
            }
        }

        private static void OnItemsStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_itemsStretch = (UniformGridLayoutItemsStretch)e.NewValue;
        }

        #endregion

        #region MaximumRowsOrColumns property

        public static readonly DependencyProperty MaximumRowsOrColumnsProperty = DependencyProperty.Register("MaximumRowsOrColumns", typeof(int), typeof(UniformGridLayout), new FrameworkPropertyMetadata(-1, OnMaximumRowsOrColumnsPropertyChanged));

        public int MaximumRowsOrColumns
        {
            get
            {
                return (int)GetValue(MaximumRowsOrColumnsProperty);
            }
            set
            {
                SetValue(MaximumRowsOrColumnsProperty, value);
            }
        }

        private static void OnMaximumRowsOrColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformGridLayout uniformGridLayout = d as UniformGridLayout;

            if (d == null)
                return;

            uniformGridLayout.m_maximumRowsOrColumns = (uint)e.NewValue;
        }

        #endregion

        #region Methods

        public UniformGridLayout()
        {
            m_orientationBasedMeasures = new OrientationBasedMeasures();
            LayoutId = "UniformGridLayout";
        }

        #region IVirtualizingLayoutOverrides

        protected override void InitializeForContextCore(VirtualizingLayoutContext context)
        {
            object state = context.LayoutState;
            UniformGridLayoutState gridState = null;
            if (state != null)
            {
                gridState = GetAsGridState(state);
            }

            if (gridState == null)
            {
                if (state != null)
                {
                    throw new Exception("LayoutState must derive from UniformGridLayoutState.");
                }

                // Custom deriving layouts could potentially be stateful.
                // If that is the case, we will just create the base state required by UniformGridLayout ourselves.
                gridState = new UniformGridLayoutState();
            }

            gridState.InitializeForContext(context, this);
        }

        protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
        {
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            gridState.UninitializeForContext(context);
        }

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            // Set the width and height on the grid state. If the user already set them then use the preset.
            // If not, we have to measure the first element and get back a size which we're going to be using for the rest of the items.
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            gridState.EnsureElementSize(availableSize, context, m_minItemWidth, m_minItemHeight, m_itemsStretch, Orientation, MinRowSpacing, MinColumnSpacing, m_maximumRowsOrColumns);

            Size desiredSize = GetFlowAlgorithm(context).Measure(
                availableSize,
                context,
                true, /* isWrapping*/
                MinItemSpacing,
                LineSpacing,
                m_maximumRowsOrColumns /* maxItemsPerLine */,
                m_orientationBasedMeasures.ScrollOrientation,
                false /* disableVirtualization */,
                LayoutId);

            return desiredSize;
        }

        protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        {
            Size value = GetFlowAlgorithm(context).Arrange(
                finalSize,
                context,
                true /* isWrapping */,
                (FlowLayoutAlgorithm.LineAlignment)m_itemsJustification,
                LayoutId);
            return value;
        }

        protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
        {
            GetFlowAlgorithm(context).OnItemsSourceChanged(source, args, context);
            // Always invalidate layout to keep the view accurate.
            InvalidateLayout();
        }

        #endregion

        #region IFlowLayoutAlgorithmDelegates

        public Size Algorithm_GetMeasureSize(int index, Size availableSize, VirtualizingLayoutContext context)
        {
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            return new Size(gridState.EffectiveItemWidth, gridState.EffectiveItemHeight);
        }

        public Size Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
        {
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            return new Size(gridState.EffectiveItemWidth,gridState.EffectiveItemHeight);
        }

        public bool Algorithm_ShouldBreakLine(int index, double remainingSpace)
        {
            return remainingSpace < 0;
        }

        public FlowLayoutAnchorInfo Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
        {
            Rect bounds = new Rect(double.NaN, double.NaN, double.NaN, double.NaN);
            int anchorIndex = -1;

            int itemsCount = context.ItemCount;
            Rect realizationRect = context.RealizationRect;
            if (itemsCount > 0 && m_orientationBasedMeasures.MajorSize(realizationRect) > 0)
            {
                UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
                Rect lastExtent = gridState.FlowAlgorithm.LastExtent();
                int itemsPerLine = (int)Math.Min( // note use of unsigned ints
                    Math.Max(1u, (uint)(m_orientationBasedMeasures.Minor(availableSize) / GetMinorSizeWithSpacing(context))),
                    Math.Max(1u, m_maximumRowsOrColumns));
                double majorSize = (itemsCount / itemsPerLine) * (double)(GetMajorSizeWithSpacing(context));
                double realizationWindowStartWithinExtent = (double)(m_orientationBasedMeasures.MajorStart(realizationRect) - m_orientationBasedMeasures.MajorStart(lastExtent));
                if ((realizationWindowStartWithinExtent + m_orientationBasedMeasures.MajorSize(realizationRect)) >= 0 && realizationWindowStartWithinExtent <= majorSize)
                {
                    double offset = Math.Max(0.0f, m_orientationBasedMeasures.MajorStart(realizationRect) - m_orientationBasedMeasures.MajorStart(lastExtent));
                    int anchorRowIndex = (int)(offset / GetMajorSizeWithSpacing(context));

                    anchorIndex = Math.Max(0, Math.Min(itemsCount - 1, anchorRowIndex * itemsPerLine));
                    bounds = GetLayoutRectForDataIndex(availableSize, anchorIndex, lastExtent, context);
                }
            }

            return new FlowLayoutAnchorInfo()
            {
                Index = anchorIndex,
                Offset = m_orientationBasedMeasures.MajorStart(bounds)
            };
        }

        public FlowLayoutAnchorInfo Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
        {
            int index = -1;
            double offset = double.NaN;
            int count = context.ItemCount;
            if (targetIndex >= 0 && targetIndex < count)
            {
                int itemsPerLine = (int)Math.Min( // note use of unsigned ints
                    Math.Max(1u, (uint)(m_orientationBasedMeasures.Minor(availableSize) / GetMinorSizeWithSpacing(context))),
                    Math.Max(1u, m_maximumRowsOrColumns));
                int indexOfFirstInLine = (targetIndex / itemsPerLine) * itemsPerLine;
                index = indexOfFirstInLine;
                UniformGridLayoutState state = GetAsGridState(context.LayoutState);
                offset = m_orientationBasedMeasures.MajorStart(GetLayoutRectForDataIndex(availableSize, indexOfFirstInLine, state.FlowAlgorithm.LastExtent(), context));
            }

            return new FlowLayoutAnchorInfo()
            {
                Index = index,
                Offset = offset
            };
        }

        public Rect Algorithm_GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
        {
            //UNREFERENCED_PARAMETER(lastRealized);

            Rect extent = new Rect();


            // Constants
            int itemsCount = context.ItemCount;
            double availableSizeMinor = m_orientationBasedMeasures.Minor(availableSize);
            int itemsPerLine =
                (int)Math.Min( // note use of unsigned ints
                    Math.Max(1u, !double.IsInfinity(availableSizeMinor) ? (uint)(availableSizeMinor / GetMinorSizeWithSpacing(context)) : (uint)itemsCount),
                    Math.Max(1u, m_maximumRowsOrColumns));

            double lineSize = GetMajorSizeWithSpacing(context);

            if (itemsCount > 0)
            {
                // Only use all of the space if item stretch is fill, otherwise size layout according to items placed
                m_orientationBasedMeasures.SetMinorSize(ref extent, !double.IsInfinity(availableSizeMinor) && m_itemsStretch == UniformGridLayoutItemsStretch.Fill ? availableSizeMinor : Math.Max(0.0, itemsPerLine * GetMinorSizeWithSpacing(context) - MinItemSpacing));
                m_orientationBasedMeasures.SetMajorSize(ref extent, Math.Max(0.0, (itemsCount / itemsPerLine) * lineSize - LineSpacing));

                if (firstRealized != null)
                {
                    //MUX_ASSERT(lastRealized);

                    m_orientationBasedMeasures.SetMajorStart(ref extent, m_orientationBasedMeasures.MajorStart(firstRealizedLayoutBounds) - (firstRealizedItemIndex / itemsPerLine) * lineSize);
                    int remainingItems = itemsCount - lastRealizedItemIndex - 1;
                    m_orientationBasedMeasures.SetMajorSize(ref extent, m_orientationBasedMeasures.MajorEnd(lastRealizedLayoutBounds) - m_orientationBasedMeasures.MajorStart(extent) + (remainingItems / itemsPerLine) * lineSize);
                }
                else
                {
                    //REPEATER_TRACE_INFO(L"%ls: \tEstimating extent with no realized elements. \n", LayoutId().data());
                }
            }
            else
            {
                //MUX_ASSERT(firstRealizedItemIndex == -1);
                //MUX_ASSERT(lastRealizedItemIndex == -1);
            }

            //REPEATER_TRACE_INFO(L"%ls: \tExtent is (%.0f,%.0f). Based on lineSize %.0f and items per line %.0f. \n", LayoutId().data(), extent.Width, extent.Height, lineSize, itemsPerLine);
            return extent;
        }

        public void Algorithm_OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
        {

        }

        public void Algorithm_OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
        {

        }

        #endregion

        // Methods
        private double GetMinorSizeWithSpacing(VirtualizingLayoutContext context)
        {
            double minItemSpacing = MinItemSpacing;
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            return m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? gridState.EffectiveItemWidth + minItemSpacing : gridState.EffectiveItemHeight + minItemSpacing;
        }

        private double GetMajorSizeWithSpacing(VirtualizingLayoutContext context)
        {
            double lineSpacing = LineSpacing;
            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            return m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? gridState.EffectiveItemHeight + lineSpacing : gridState.EffectiveItemWidth + lineSpacing;
        }

        private Rect GetLayoutRectForDataIndex(Size availableSize, int index, Rect lastExtent, VirtualizingLayoutContext context)
        {
            int itemsPerLine = (int)Math.Min( //note use of unsigned ints
                Math.Max(1u, (uint)(m_orientationBasedMeasures.Minor(availableSize) / GetMinorSizeWithSpacing(context))),
                Math.Max(1u, m_maximumRowsOrColumns));

            int rowIndex = (int)(index / itemsPerLine);
            int indexInRow = index - (rowIndex * itemsPerLine);

            UniformGridLayoutState gridState = GetAsGridState(context.LayoutState);
            Rect bounds = m_orientationBasedMeasures.MinorMajorRect(
                indexInRow * GetMinorSizeWithSpacing(context) + m_orientationBasedMeasures.MinorStart(lastExtent),
                rowIndex * GetMajorSizeWithSpacing(context) + m_orientationBasedMeasures.MajorStart(lastExtent),
                m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? gridState.EffectiveItemWidth : gridState.EffectiveItemHeight,
                m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? gridState.EffectiveItemHeight : gridState.EffectiveItemWidth);

            return bounds;
        }

        private UniformGridLayoutState GetAsGridState(object state)
        {
            return state as UniformGridLayoutState;
        }

        private FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
        {
            return GetAsGridState(context.LayoutState).FlowAlgorithm;
        }

        private void InvalidateLayout()
        {
            InvalidateMeasure();
        }

        private double LineSpacing
        {
            get
            {
                return Orientation == Orientation.Horizontal ? m_minRowSpacing : m_minColumnSpacing;
            }
        }

        private double MinItemSpacing
        {
            get
            {
                return Orientation == Orientation.Horizontal ? m_minColumnSpacing : m_minRowSpacing;
            }
        }


        #endregion

        // Fields
        private double m_minItemWidth = double.NaN;
        private double m_minItemHeight = double.NaN;
        private double m_minRowSpacing = double.NaN;
        private double m_minColumnSpacing = double.NaN;
        private UniformGridLayoutItemsJustification m_itemsJustification = UniformGridLayoutItemsJustification.Start;
        private UniformGridLayoutItemsStretch m_itemsStretch = UniformGridLayoutItemsStretch.None;
        private uint m_maximumRowsOrColumns = uint.MaxValue;

        private OrientationBasedMeasures m_orientationBasedMeasures;
        // !!! WARNING !!!
        // Any storage here needs to be related to layout configuration.
        // layout specific state needs to be stored in UniformGridLayoutState.
    }
}
