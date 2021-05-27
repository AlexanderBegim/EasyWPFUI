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
    public class StackLayout : VirtualizingLayout, IFlowLayoutAlgorithmDelegates
    {
        #region Orientation property

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackLayout), new FrameworkPropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged));

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
            StackLayout stackLayout = d as StackLayout;

            if (stackLayout == null)
                return;

            Orientation orientation = (Orientation)e.NewValue;

            //Note: For StackLayout Vertical Orientation means we have a Vertical ScrollOrientation.
            //Horizontal Orientation means we have a Horizontal ScrollOrientation.
            ScrollOrientation scrollOrientation = (orientation == Orientation.Horizontal) ? ScrollOrientation.Horizontal : ScrollOrientation.Vertical;
            stackLayout.m_orientationBasedMeasures.ScrollOrientation = scrollOrientation;

            stackLayout.InvalidateLayout();
        }

        #endregion

        #region Spacing property

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register("Spacing", typeof(double), typeof(StackLayout), new FrameworkPropertyMetadata(0.0, OnSpacingPropertyChanged));

        public double Spacing
        {
            get
            {
                return (double)GetValue(SpacingProperty);
            }
            set
            {
                SetValue(SpacingProperty, value);
            }
        }

        private static void OnSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StackLayout stackLayout = d as StackLayout;

            if (stackLayout == null)
                return;

            stackLayout.m_itemSpacing = (double)e.NewValue;

            stackLayout.InvalidateLayout();
        }

        #endregion

        #region DisableVirtualization property

        public static readonly DependencyPropertyKey DisableVirtualizationPropertyKey = DependencyProperty.RegisterReadOnly("DisableVirtualization", typeof(bool), typeof(StackLayout), new FrameworkPropertyMetadata(OnDisableVirtualizationPropertyChanged));

        public static readonly DependencyProperty DisableVirtualizationProperty = DisableVirtualizationPropertyKey.DependencyProperty;

        public bool DisableVirtualization
        {
            get
            {
                return (bool)GetValue(DisableVirtualizationProperty);
            }
            set
            {
                SetValue(DisableVirtualizationPropertyKey, value);
            }
        }

        private static void OnDisableVirtualizationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        #endregion


        #region Methods

        public StackLayout()
        {
            m_orientationBasedMeasures = new OrientationBasedMeasures();
            LayoutId = "StackLayout";
        }

        private FlowLayoutAnchorInfo GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
        {
            int anchorIndex = -1;
            double offset = double.NaN;

            // Constants
            int itemsCount = context.ItemCount;
            if (itemsCount > 0)
            {
                Rect realizationRect = context.RealizationRect;
                StackLayoutState state = GetAsStackState(context.LayoutState);
                Rect lastExtent = state.FlowAlgorithm.LastExtent();

                double averageElementSize = GetAverageElementSize(availableSize, context, state) + m_itemSpacing;
                double realizationWindowOffsetInExtent = m_orientationBasedMeasures.MajorStart(realizationRect) - m_orientationBasedMeasures.MajorStart(lastExtent);
                double majorSize = m_orientationBasedMeasures.MajorSize(lastExtent) == 0 ? Math.Max(0.0, averageElementSize * itemsCount - m_itemSpacing) : m_orientationBasedMeasures.MajorSize(lastExtent);
                if (itemsCount > 0 &&
                    m_orientationBasedMeasures.MajorSize(realizationRect) >= 0 &&
                    // MajorSize = 0 will account for when a nested repeater is outside the realization rect but still being measured. Also,
                    // note that if we are measuring this repeater, then we are already realizing an element to figure out the size, so we could
                    // just keep that element alive. It also helps in XYFocus scenarios to have an element realized for XYFocus to find a candidate
                    // in the navigating direction.
                    realizationWindowOffsetInExtent + m_orientationBasedMeasures.MajorSize(realizationRect) >= 0 && realizationWindowOffsetInExtent <= majorSize)
                {
                    anchorIndex = (int)(realizationWindowOffsetInExtent / averageElementSize);
                    offset = anchorIndex * averageElementSize + m_orientationBasedMeasures.MajorStart(lastExtent);
                    anchorIndex = Math.Max(0, Math.Min(itemsCount - 1, anchorIndex));
                }
            }

            return new FlowLayoutAnchorInfo()
            {
                Index = anchorIndex,
                Offset = offset
            };
        }

        private Rect GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
        {
            // UNREFERENCED_PARAMETER(lastRealized);

            Rect extent = new Rect();

            // Constants
            int itemsCount = context.ItemCount;
            StackLayoutState stackState = GetAsStackState(context.LayoutState);
            double averageElementSize = GetAverageElementSize(availableSize, context, stackState) + m_itemSpacing;

            m_orientationBasedMeasures.SetMinorSize(ref extent, stackState.MaxArrangeBounds);
            m_orientationBasedMeasures.SetMajorSize(ref extent, Math.Max(0.0, itemsCount * averageElementSize - m_itemSpacing));
            if (itemsCount > 0)
            {
                if (firstRealized != null)
                {
                    // MUX_ASSERT(lastRealized);
                    m_orientationBasedMeasures.SetMajorStart(ref extent, m_orientationBasedMeasures.MajorStart(firstRealizedLayoutBounds) - (firstRealizedItemIndex * averageElementSize));
                    int remainingItems = itemsCount - lastRealizedItemIndex - 1;
                    m_orientationBasedMeasures.SetMajorSize(ref extent, m_orientationBasedMeasures.MajorEnd(lastRealizedLayoutBounds) - m_orientationBasedMeasures.MajorStart(extent) + (remainingItems * averageElementSize));
                }
                else
                {
                    // REPEATER_TRACE_INFO(L"%*s: \tEstimating extent with no realized elements.  \n", winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data());
                }
            }
            else
            {
                // MUX_ASSERT(firstRealizedItemIndex == -1);
                //MUX_ASSERT(lastRealizedItemIndex == -1);
            }

            // REPEATER_TRACE_INFO(L"%*s: \tExtent is (%.0f,%.0f). Based on average %.0f. \n", winrt::get_self<VirtualizingLayoutContext>(context)->Indent(), LayoutId().data(), extent.Width, extent.Height, averageElementSize);
            return extent;
        }

        private void OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSizeSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
        {
            VirtualizingLayoutContext virtualContext = context as VirtualizingLayoutContext;
            if (virtualContext != null)
            {
                StackLayoutState stackState = GetAsStackState(virtualContext.LayoutState);
                Size provisionalArrangeSizeWinRt = provisionalArrangeSize;
                stackState.OnElementMeasured(index, m_orientationBasedMeasures.Major(provisionalArrangeSizeWinRt), m_orientationBasedMeasures.Minor(provisionalArrangeSizeWinRt));
            }
        }

        #region IVirtualizingLayoutOverrides

        protected override void InitializeForContextCore(VirtualizingLayoutContext context)
        {
            object state = context.LayoutState;
            StackLayoutState stackState = null;
            if (state != null)
            {
                stackState = GetAsStackState(state);
            }

            if (stackState == null)
            {
                if (state != null)
                {
                    throw new Exception("LayoutState must derive from StackLayoutState.");
                }

                // Custom deriving layouts could potentially be stateful.
                // If that is the case, we will just create the base state required by UniformGridLayout ourselves.
                stackState = new StackLayoutState();
            }

            stackState.InitializeForContext(context, this);
        }

        protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
        {
            StackLayoutState stackState = GetAsStackState(context.LayoutState);
            stackState.UninitializeForContext(context);
        }

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            GetAsStackState(context.LayoutState).OnMeasureStart();

            Size desiredSize = GetFlowAlgorithm(context).Measure(
                availableSize,
                context,
                false, /* isWrapping*/
                0 /* minItemSpacing */,
                m_itemSpacing,
                uint.MaxValue /* maxItemsPerLine */,
                m_orientationBasedMeasures.ScrollOrientation,
                DisableVirtualization,
                LayoutId);

            return desiredSize;
        }

        protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        {
            Size value = GetFlowAlgorithm(context).Arrange(
                finalSize,
                context,
                false, /* isWraping */
                FlowLayoutAlgorithm.LineAlignment.Start,
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
            return availableSize;
        }

        public Size Algorithm_GetProvisionalArrangeSize(int index, Size measureSize, Size desiredSize, VirtualizingLayoutContext context)
        {
            double measureSizeMinor = m_orientationBasedMeasures.Minor(measureSize);

            return m_orientationBasedMeasures.MinorMajorSize(
                 !double.IsInfinity(measureSizeMinor) ?
                     Math.Max(measureSizeMinor, m_orientationBasedMeasures.Minor(desiredSize)) :
                     m_orientationBasedMeasures.Minor(desiredSize),
                 m_orientationBasedMeasures.Major(desiredSize));
        }

        public bool Algorithm_ShouldBreakLine(int index, double remainingSpace)
        {
            return true;
        }

        public FlowLayoutAnchorInfo Algorithm_GetAnchorForRealizationRect(Size availableSize, VirtualizingLayoutContext context)
        {
            return GetAnchorForRealizationRect(availableSize, context);
        }

        public FlowLayoutAnchorInfo Algorithm_GetAnchorForTargetElement(int targetIndex, Size availableSize, VirtualizingLayoutContext context)
        {
            double offset = double.NaN;
            int index = -1;
            int itemsCount = context.ItemCount;

            if (targetIndex >= 0 && targetIndex < itemsCount)
            {
                index = targetIndex;
                StackLayoutState state = GetAsStackState(context.LayoutState);
                double averageElementSize = GetAverageElementSize(availableSize, context, state) + m_itemSpacing;
                offset = index * averageElementSize + m_orientationBasedMeasures.MajorStart(state.FlowAlgorithm.LastExtent());
            }

            return new FlowLayoutAnchorInfo()
            {
                Index = index,
                Offset = offset
            };
        }

        public Rect Algorithm_GetExtent(Size availableSize, VirtualizingLayoutContext context, UIElement firstRealized, int firstRealizedItemIndex, Rect firstRealizedLayoutBounds, UIElement lastRealized, int lastRealizedItemIndex, Rect lastRealizedLayoutBounds)
        {
            return GetExtent(
                availableSize,
                context,
                firstRealized,
                firstRealizedItemIndex,
                firstRealizedLayoutBounds,
                lastRealized,
                lastRealizedItemIndex,
                lastRealizedLayoutBounds);
        }

        public void Algorithm_OnElementMeasured(UIElement element, int index, Size availableSize, Size measureSize, Size desiredSize, Size provisionalArrangeSize, VirtualizingLayoutContext context)
        {
            OnElementMeasured(
                element,
                index,
                availableSize,
                measureSize,
                desiredSize,
                provisionalArrangeSize,
                context);
        }

        public void Algorithm_OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
        {

        }

        #endregion

        #region private helpers

        private double GetAverageElementSize(Size availableSize, VirtualizingLayoutContext context, StackLayoutState stackLayoutState)
        {
            double averageElementSize = 0;

            if (context.ItemCount > 0)
            {
                if (stackLayoutState.TotalElementsMeasured == 0)
                {
                    UIElement tmpElement = context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
                    stackLayoutState.FlowAlgorithm.MeasureElement(tmpElement, 0, availableSize, context);
                    context.RecycleElement(tmpElement);
                }

                // MUX_ASSERT(stackLayoutState->TotalElementsMeasured() > 0);
                averageElementSize = Math.Round(stackLayoutState.TotalElementSize / stackLayoutState.TotalElementsMeasured);
            }

            return averageElementSize;
        }

        #endregion

        private StackLayoutState GetAsStackState(object state)
        {
            return state as StackLayoutState;
        }

        private void InvalidateLayout()
        {
            InvalidateMeasure();
        }

        FlowLayoutAlgorithm GetFlowAlgorithm(VirtualizingLayoutContext context)
        {
            return GetAsStackState(context.LayoutState).FlowAlgorithm;
        }

        // Fields
        private double m_itemSpacing;
        private OrientationBasedMeasures m_orientationBasedMeasures;

        // !!! WARNING !!!
        // Any storage here needs to be related to layout configuration. 
        // layout specific state needs to be stored in StackLayoutState.

        #endregion
    }
}
