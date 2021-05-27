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
    public struct FlowLayoutAnchorInfo
    {
        public int Index;
        public double Offset;
    }

    public class FlowLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
    {
        #region Orientation property

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowLayout), new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

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
            FlowLayout flowLayout = d as FlowLayout;

            if (flowLayout == null)
                return;

            Orientation orientation = (Orientation)e.NewValue;

            //Note: For FlowLayout Vertical Orientation means we have a Horizontal ScrollOrientation. Horizontal Orientation means we have a Vertical ScrollOrientation.
            //i.e. the properties are the inverse of each other.
            ScrollOrientation scrollOrientation = (orientation == Orientation.Horizontal) ? ScrollOrientation.Vertical : ScrollOrientation.Horizontal;
            flowLayout.m_orientationBasedMeasures.ScrollOrientation = scrollOrientation;
        }

        #endregion

        #region MinRowSpacing property

        public static readonly DependencyProperty MinRowSpacingProperty = DependencyProperty.Register("MinRowSpacing", typeof(double), typeof(FlowLayout), new FrameworkPropertyMetadata(0.0, OnMinRowSpacingPropertyChanged));

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
            FlowLayout flowLayout = d as FlowLayout;

            if (flowLayout == null)
                return;

            flowLayout.m_minRowSpacing = (double)e.NewValue;
        }

        #endregion

        #region MinColumnSpacing property

        public static readonly DependencyProperty MinColumnSpacingProperty = DependencyProperty.Register("MinColumnSpacing", typeof(double), typeof(FlowLayout), new FrameworkPropertyMetadata(0.0, OnMinColumnSpacingPropertyChanged));

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
            FlowLayout flowLayout = d as FlowLayout;

            if (flowLayout == null)
                return;

            flowLayout.m_minColumnSpacing = (double)e.NewValue;
        }

        #endregion

        #region LineAlignment property

        public static readonly DependencyProperty LineAlignmentProperty = DependencyProperty.Register("LineAlignment", typeof(FlowLayoutLineAlignment), typeof(FlowLayout), new FrameworkPropertyMetadata(FlowLayoutLineAlignment.Start, OnLineAlignmentPropertyChanged));

        public FlowLayoutLineAlignment LineAlignment
        {
            get
            {
                return (FlowLayoutLineAlignment)GetValue(LineAlignmentProperty);
            }
            set
            {
                SetValue(LineAlignmentProperty, value);
            }
        }

        private static void OnLineAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlowLayout flowLayout = d as FlowLayout;

            if (flowLayout == null)
                return;

            flowLayout.m_lineAlignment = (FlowLayoutLineAlignment)e.NewValue;
        }

        #endregion


        #region Methods

        public FlowLayout()
        {
            LayoutId = "FlowLayout";

            m_orientationBasedMeasures = new OrientationBasedMeasures();
        }

        #region IVirtualizingLayoutOverrides

        protected override void InitializeForContextCore(VirtualizingLayoutContext context)
        {
            object state = context.LayoutState;
            FlowLayoutState flowState = null;
            if (state != null)
            {
                flowState = GetAsFlowState(state);
            }

            if (flowState == null)
            {
                if (state != null)
                {
                    throw new Exception("LayoutState must derive from FlowLayoutState.");
                }

                // Custom deriving layouts could potentially be stateful.
                // If that is the case, we will just create the base state required by FlowLayout ourselves.
                flowState = new FlowLayoutState();
            }

            flowState.InitializeForContext(context, this);
        }

        protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
        {
            FlowLayoutState flowState = GetAsFlowState(context.LayoutState);
            flowState.UninitializeForContext(context);
        }

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            Size desiredSize = GetFlowAlgorithm(context).Measure(
                availableSize,
                context,
                true, /* isWrapping*/
                MinItemSpacing,
                LineSpacing,
                int.MaxValue/* maxItemsPerLine */,
                m_orientationBasedMeasures.ScrollOrientation,
                false /* disableVirtualization */,
                LayoutId);
            return desiredSize;
        }

        protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        {
            var value = GetFlowAlgorithm(context).Arrange(
                finalSize,
                context,
                true, /* isWrapping */
                (FlowLayoutAlgorithm.LineAlignment)m_lineAlignment,
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

        #region IFlowLayoutOverrides

        protected virtual Size GetMeasureSize(int index, Size availableSize)
        {
            return availableSize;
        }

        protected virtual Size GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize)
        {
            return desiredSize;
        }

        protected virtual bool ShouldBreakLine(int index, double remainingSpace)
        {
            return remainingSpace < 0;
        }

        protected virtual FlowLayoutAnchorInfo GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
        {
            int anchorIndex = -1;
            double offset = double.NaN;

            // Constants
            int itemsCount = context.ItemCount;
            if (itemsCount > 0)
            {
                Rect realizationRect = context.RealizationRect;
                object state = context.LayoutState;
                FlowLayoutState flowState = GetAsFlowState(state);
                Rect lastExtent = flowState.FlowAlgorithm.LastExtent();

                double averageItemsPerLine = 0;
                double averageLineSize = GetAverageLineInfo(availableSize, context, flowState, ref averageItemsPerLine) + LineSpacing;
                // MUX_ASSERT(averageItemsPerLine != 0);

                double extentMajorSize = m_orientationBasedMeasures.MajorSize(lastExtent) == 0 ? (itemsCount / averageItemsPerLine) * averageLineSize : m_orientationBasedMeasures.MajorSize(lastExtent);
                if (itemsCount > 0 &&
                    m_orientationBasedMeasures.MajorSize(realizationRect) > 0 &&
                    DoesRealizationWindowOverlapExtent(realizationRect, m_orientationBasedMeasures.MinorMajorRect(m_orientationBasedMeasures.MinorStart(lastExtent), m_orientationBasedMeasures.MajorStart(lastExtent), m_orientationBasedMeasures.Minor(availableSize), extentMajorSize)))
                {
                    double realizationWindowStartWithinExtent = m_orientationBasedMeasures.MajorStart(realizationRect) - m_orientationBasedMeasures.MajorStart(lastExtent);
                    int lineIndex = Math.Max(0, (int)(realizationWindowStartWithinExtent / averageLineSize));
                    anchorIndex = (int)(lineIndex * averageItemsPerLine);

                    // Clamp it to be within valid range
                    anchorIndex = Math.Max(0, Math.Min(itemsCount - 1, anchorIndex));
                    offset = lineIndex * averageLineSize + m_orientationBasedMeasures.MajorStart(lastExtent);
                }
            }

            return new FlowLayoutAnchorInfo
            {
                Index = anchorIndex,
                Offset = offset
            };
        }

        protected virtual FlowLayoutAnchorInfo GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
        {
            double offset = double.NaN;
            int index = -1;
            int itemsCount = context.ItemCount;

            if (targetIndex >= 0 && targetIndex < itemsCount)
            {
                index = targetIndex;
                object state = context.LayoutState;
                FlowLayoutState flowState = GetAsFlowState(state);
                double averageItemsPerLine = 0;
                double averageLineSize = GetAverageLineInfo(availableSize, context, flowState, ref averageItemsPerLine) + LineSpacing;
                int lineIndex = (int)(targetIndex / averageItemsPerLine);
                offset = lineIndex * averageLineSize + m_orientationBasedMeasures.MajorStart(flowState.FlowAlgorithm.LastExtent());
            }

            return new FlowLayoutAnchorInfo()
            {
                Index = index,
                Offset = offset
            };
        }

        protected virtual Rect GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
        {
            // UNREFERENCED_PARAMETER(lastRealized);

            Rect extent = new Rect();

            int itemsCount = context.ItemCount;

            if (itemsCount > 0)
            {
                double availableSizeMinor = m_orientationBasedMeasures.Minor(availableSize);
                object state = context.LayoutState;
                FlowLayoutState flowState = GetAsFlowState(state);
                double averageItemsPerLine = 0;
                double averageLineSize = GetAverageLineInfo(availableSize, context, flowState, ref averageItemsPerLine) + LineSpacing;

                // MUX_ASSERT(averageItemsPerLine != 0);
                if (firstRealized != null)
                {
                    // MUX_ASSERT(lastRealized);
                    int linesBeforeFirst = (int)(firstRealizedItemIndex / averageItemsPerLine);
                    double extentMajorStart = m_orientationBasedMeasures.MajorStart(firstRealizedLayoutBounds) - linesBeforeFirst * averageLineSize;
                    m_orientationBasedMeasures.SetMajorStart(ref extent, extentMajorStart);
                    int remainingItems = itemsCount - lastRealizedItemIndex - 1;
                    int remainingLinesAfterLast = (int)(remainingItems / averageItemsPerLine);
                    double extentMajorSize = m_orientationBasedMeasures.MajorEnd(lastRealizedLayoutBounds) - m_orientationBasedMeasures.MajorStart(extent) + remainingLinesAfterLast * averageLineSize;
                    m_orientationBasedMeasures.SetMajorSize(ref extent, extentMajorSize);

                    // If the available size is infinite, we will have realized all the items in one line.
                    // In that case, the extent in the non virtualizing direction should be based on the
                    // right/bottom of the last realized element.
                    m_orientationBasedMeasures.SetMinorSize(ref extent, !double.IsInfinity(availableSizeMinor) ? availableSizeMinor : Math.Max(0.0, m_orientationBasedMeasures.MinorEnd(lastRealizedLayoutBounds)));
                }
                else
                {
                    double lineSpacing = LineSpacing;
                    double minItemSpacing = MinItemSpacing;
                    // We dont have anything realized. make an educated guess.
                    int numLines = (int)Math.Ceiling(itemsCount / averageItemsPerLine);
                    extent = !double.IsInfinity(availableSizeMinor) ?
                        m_orientationBasedMeasures.MinorMajorRect(0, 0, availableSizeMinor, Math.Max(0.0, numLines * averageLineSize - lineSpacing)) :
                        m_orientationBasedMeasures.MinorMajorRect(
                            0,
                            0,
                            Math.Max(0.0, (m_orientationBasedMeasures.Minor(flowState.SpecialElementDesiredSize) + minItemSpacing) * itemsCount - minItemSpacing),
                            Math.Max(0.0, (averageLineSize - lineSpacing)));

                    // REPEATER_TRACE_INFO(L"%*s: \tEstimating extent with no realized elements. \n", winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data());
                }

                // REPEATER_TRACE_INFO(L"%*s: \tExtent is {%.0f,%.0f}. Based on average line size {%.0f} and average items per line {%.0f}. \n",winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data(), extent.Width, extent.Height, averageLineSize, averageItemsPerLine);
            }
            else
            {
                // MUX_ASSERT(firstRealizedItemIndex == -1);
                // MUX_ASSERT(lastRealizedItemIndex == -1);

                // REPEATER_TRACE_INFO(L"%*s: \tExtent is {%.0f,%.0f}. ItemCount is 0 \n",winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data(), extent.Width, extent.Height);
            }

            return extent;
        }

        protected virtual void OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
        {

        }

        protected virtual void OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
        {
            // REPEATER_TRACE_INFO(L"%*s: \tOnLineArranged startIndex:%d Count:%d LineHeight:%d \n",winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data(), startIndex, countInLine, lineSize);

            FlowLayoutState flowState = GetAsFlowState(context.LayoutState);
            flowState.OnLineArranged(startIndex, countInLine, lineSize, context);
        }

        #endregion

        #region IFlowLayoutAlgorithmDelegates

        Size IFlowLayoutAlgorithmDelegates.Algorithm_GetMeasureSize(int index, Size availableSize, VirtualizingLayoutContext context)
        {
            return GetMeasureSize(index, availableSize);
        }

        Size IFlowLayoutAlgorithmDelegates.Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
        {
            return GetProvisionalArrangeSize(index, measureSize, desiredSize);
        }

        bool IFlowLayoutAlgorithmDelegates.Algorithm_ShouldBreakLine(int index, double remainingSpace)
        {
            return ShouldBreakLine(index, remainingSpace);
        }

        FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
        {
            return GetAnchorForRealizationRect(availableSize, context);
        }

        FlowLayoutAnchorInfo IFlowLayoutAlgorithmDelegates.Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
        {
            return GetAnchorForTargetElement(targetIndex, availableSize, context);
        }

        Rect IFlowLayoutAlgorithmDelegates.Algorithm_GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
        {
            return GetExtent(availableSize, context, firstRealized, firstRealizedItemIndex, firstRealizedLayoutBounds, lastRealized, lastRealizedItemIndex, lastRealizedLayoutBounds);
        }

        void IFlowLayoutAlgorithmDelegates.Algorithm_OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
        {
            OnElementMeasured(element, index, availableSize, measureSize, desiredSize, provisionalArrangeSize, context);
        }

        void IFlowLayoutAlgorithmDelegates.Algorithm_OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
        {
            OnLineArranged(startIndex, countInLine, lineSize, context);
        }

        #endregion

        private double GetAverageLineInfo(Size availableSize, VirtualizingLayoutContext context, FlowLayoutState flowState, ref double avgCountInLine)
        {
            // default to 1 item per line with 0 size
            double avgLineSize = 0;
            avgCountInLine = 1;

            // MUX_ASSERT(context.ItemCountCore() > 0);

            if (flowState.TotalLinesMeasured == 0)
            {
                UIElement tmpElement = context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
                Size desiredSize = flowState.FlowAlgorithm.MeasureElement(tmpElement, 0, availableSize, context);
                context.RecycleElement(tmpElement);

                int estimatedCountInLine = Math.Max(1, (int)(m_orientationBasedMeasures.Minor(availableSize) / m_orientationBasedMeasures.Minor(desiredSize)));
                flowState.OnLineArranged(0, estimatedCountInLine, m_orientationBasedMeasures.Major(desiredSize), context);
                flowState.SpecialElementDesiredSize = desiredSize;
            }

            avgCountInLine = Math.Max(1.0, flowState.TotalItemsPerLine / flowState.TotalLinesMeasured);
            avgLineSize = Math.Round(flowState.TotalLineSize / flowState.TotalLinesMeasured);

            return avgLineSize;
        }

        private FlowLayoutState GetAsFlowState(object state)
        {
            return state as FlowLayoutState;
        }

        private void InvalidateLayout()
        {
            InvalidateMeasure();
        }

        private FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
        {
            return GetAsFlowState(context.LayoutState).FlowAlgorithm;
        }

        private bool DoesRealizationWindowOverlapExtent(Rect realizationWindow, Rect extent)
        {
            return m_orientationBasedMeasures.MajorEnd(realizationWindow) >= m_orientationBasedMeasures.MajorStart(extent) && m_orientationBasedMeasures.MajorStart(realizationWindow) <= m_orientationBasedMeasures.MajorEnd(extent);
        }

        private double LineSpacing
        {
            get
            {
                return m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? m_minRowSpacing : m_minColumnSpacing;
            }
        }

        private double MinItemSpacing
        {
            get
            {
                return m_orientationBasedMeasures.ScrollOrientation == ScrollOrientation.Vertical ? m_minColumnSpacing : m_minRowSpacing;
            }
        }

        // Fields
        private double m_minRowSpacing;
        private double m_minColumnSpacing;
        private FlowLayoutLineAlignment m_lineAlignment = FlowLayoutLineAlignment.Start;

        private OrientationBasedMeasures m_orientationBasedMeasures;

        // !!! WARNING !!!
        // Any storage here needs to be related to layout configuration. 
        // layout specific state needs to be stored in FlowLayoutState.

        #endregion
    }
}
