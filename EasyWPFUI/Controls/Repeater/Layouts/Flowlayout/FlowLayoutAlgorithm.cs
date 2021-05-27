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
using System.Windows.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    internal class FlowLayoutAlgorithm
    {
        // Types
        public enum LineAlignment
        {
            Start,
            Center,
            End,
            SpaceAround,
            SpaceBetween,
            SpaceEvenly
        }

        public FlowLayoutAlgorithm()
        {
            m_elementManager = new ElementManager();
            m_orientationBasedMeasure = new OrientationBasedMeasures();
        }


        public Rect LastExtent()
        {
            return m_lastExtent;
        }

        public void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
        {
            m_algorithmCallbacks = callbacks;
            m_context = context;
            m_elementManager.SetContext(context);
        }

        public void UninitializeForContext(VirtualizingLayoutContext context)
        {
            if (IsVirtualizingContext())
            {
                // This layout is about to be detached. Let go of all elements
                // being held and remove the layout state from the context.
                m_elementManager.ClearRealizedRange();
            }
            context.LayoutState = null;
        }

        public Size Measure(Size availableSize, VirtualizingLayoutContext context, bool isWrapping, double minItemSpacing, double lineSpacing, uint maxItemsPerLine, ScrollOrientation orientation, bool disableVirtualization, string layoutId)
        {
            m_orientationBasedMeasure.ScrollOrientation = orientation;

            // If minor size is infinity, there is only one line and no need to align that line.
            m_scrollOrientationSameAsFlow =  double.IsInfinity(m_orientationBasedMeasure.Minor(availableSize));

            Rect realizationRect = RealizationRect();

            //REPEATER_TRACE_INFO(L"%*s: \tMeasureLayout Realization(%.0f,%.0f,%.0f,%.0f)\n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data(), realizationRect.X, realizationRect.Y, realizationRect.Width, realizationRect.Height);

            int suggestedAnchorIndex = m_context.RecommendedAnchorIndex;

            if (m_elementManager.IsIndexValidInData(suggestedAnchorIndex))
            {
                bool anchorRealized = m_elementManager.IsDataIndexRealized(suggestedAnchorIndex);
                if (!anchorRealized)
                {
                    MakeAnchor(m_context, suggestedAnchorIndex, availableSize);
                }
            }

            m_elementManager.OnBeginMeasure(orientation);

            int anchorIndex = GetAnchorIndex(availableSize, isWrapping, minItemSpacing, layoutId);
            Generate(GenerateDirection.Forward, anchorIndex, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
            Generate(GenerateDirection.Backward, anchorIndex, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
            if (isWrapping && IsReflowRequired())
            {
                //REPEATER_TRACE_INFO(L"%*s: \tReflow Pass \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                Rect firstElementBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
                m_orientationBasedMeasure.SetMinorStart(ref firstElementBounds, 0);
                m_elementManager.SetLayoutBoundsForRealizedIndex(0, firstElementBounds);
                Generate(GenerateDirection.Forward, 0 /*anchorIndex*/, availableSize, minItemSpacing, lineSpacing, maxItemsPerLine, disableVirtualization, layoutId);
            }

            RaiseLineArranged();
            m_collectionChangePending = false;
            m_lastExtent = EstimateExtent(availableSize, layoutId);
            SetLayoutOrigin();

            return new Size(m_lastExtent.Width, m_lastExtent.Height);
        }

        public Size Arrange(Size finalSize, VirtualizingLayoutContext context, bool isWrapping, FlowLayoutAlgorithm.LineAlignment lineAlignment, string layoutId)
        {
            //REPEATER_TRACE_INFO("%*s: \tArrangeLayout \n", ((VirtualizingLayoutContext)context).Indent(), layoutId.data());
            ArrangeVirtualizingLayout(finalSize, lineAlignment, isWrapping, layoutId);

            return new Size(Math.Max(finalSize.Width, m_lastExtent.Width), Math.Max(finalSize.Height, m_lastExtent.Height));
        }

        public void OnItemsSourceChanged(object source, NotifyCollectionChangedEventArgs args, VirtualizingLayoutContext context)
        {
            m_elementManager.DataSourceChanged(source, args);
            m_collectionChangePending = true;
        }

        public Size MeasureElement(UIElement element, int index, Size availableSize, VirtualizingLayoutContext context)
        {
            var measureSize = m_algorithmCallbacks.Algorithm_GetMeasureSize(index, availableSize, context);
            element.Measure(measureSize);
            var provisionalArrangeSize = m_algorithmCallbacks.Algorithm_GetProvisionalArrangeSize(index, measureSize, element.DesiredSize, context);
            m_algorithmCallbacks.Algorithm_OnElementMeasured(element, index, availableSize, measureSize, element.DesiredSize, provisionalArrangeSize, context);

            return provisionalArrangeSize;
        }

        public UIElement GetElementIfRealized(int dataIndex)
        {
            if (m_elementManager.IsDataIndexRealized(dataIndex))
            {
                return m_elementManager.GetRealizedElement(dataIndex);
            }

            return null;
        }

        public bool TryAddElement0(UIElement element)
        {
            if (m_elementManager.GetRealizedElementCount() == 0)
            {
                m_elementManager.Add(element, 0);
                return true;
            }

            return false;
        }

        /* Private */

        // Types
        private enum GenerateDirection
        {
            Forward,
            Backward
        }


        // Methods
        #region Measure related private methods

        private int GetAnchorIndex(Size availableSize, bool isWrapping, double minItemSpacing, string layoutId)
        {
            int anchorIndex = -1;
            Point anchorPosition = new Point();
            VirtualizingLayoutContext context = m_context;

            if (!IsVirtualizingContext())
            {
                // Non virtualizing host, start generating from the element 0
                anchorIndex = context.ItemCount > 0 ? 0 : -1;
            }
            else
            {
                bool isRealizationWindowConnected = m_elementManager.IsWindowConnected(RealizationRect(), m_orientationBasedMeasure.ScrollOrientation, m_scrollOrientationSameAsFlow);
                // Item spacing and size in non-virtualizing direction change can cause elements to reflow
                // and get a new column position. In that case we need the anchor to be positioned in the
                // correct column.
                bool needAnchorColumnRevaluation = isWrapping && (m_orientationBasedMeasure.Minor(m_lastAvailableSize) != m_orientationBasedMeasure.Minor(availableSize) || m_lastItemSpacing != minItemSpacing || m_collectionChangePending);

                int suggestedAnchorIndex = m_context.RecommendedAnchorIndex;

                bool isAnchorSuggestionValid = suggestedAnchorIndex >= 0 && m_elementManager.IsDataIndexRealized(suggestedAnchorIndex);

                if (isAnchorSuggestionValid)
                {
                    //REPEATER_TRACE_INFO(L"%*s: \tUsing suggested anchor %d\n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data(), suggestedAnchorIndex);
                    anchorIndex = m_algorithmCallbacks.Algorithm_GetAnchorForTargetElement(suggestedAnchorIndex, availableSize, context).Index;

                    if (m_elementManager.IsDataIndexRealized(anchorIndex))
                    {
                        Rect anchorBounds = m_elementManager.GetLayoutBoundsForDataIndex(anchorIndex);
                        if (needAnchorColumnRevaluation)
                        {
                            // We were provided a valid anchor, but its position might be incorrect because for example it is in
                            // the wrong column. We do know that the anchor is the first element in the row, so we can force the minor position
                            // to start at 0.
                            anchorPosition = m_orientationBasedMeasure.MinorMajorPoint(0, m_orientationBasedMeasure.MajorStart(anchorBounds));
                        }
                        else
                        {
                            anchorPosition = new Point(anchorBounds.X, anchorBounds.Y);
                        }
                    }
                    else
                    {
                        // It is possible to end up in a situation during a collection change where GetAnchorForTargetElement returns an index
                        // which is not in the realized range. Eg. insert one item at index 0 for a grid layout.
                        // SuggestedAnchor will be 1 (used to be 0) and GetAnchorForTargetElement will return 0 (left most item in row). However 0 is not in the
                        // realized range yet. In this case we realize the gap between the target anchor and the suggested anchor.
                        int firstRealizedDataIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(0);
                        //MUX_ASSERT(anchorIndex < firstRealizedDataIndex);
                        for (int i = firstRealizedDataIndex - 1; i >= anchorIndex; --i)
                        {
                            m_elementManager.EnsureElementRealized(false /*forward*/, i, layoutId);
                        }

                        Rect anchorBounds = m_elementManager.GetLayoutBoundsForDataIndex(suggestedAnchorIndex);
                        anchorPosition = m_orientationBasedMeasure.MinorMajorPoint(0, m_orientationBasedMeasure.MajorStart(anchorBounds));
                    }
                }
                else if (needAnchorColumnRevaluation || !isRealizationWindowConnected)
                {
                    //if (needAnchorColumnRevaluation)
                    //{
                    //    REPEATER_TRACE_INFO(L"%*s: \tNeedAnchorColumnReevaluation \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                    //}
                    //if (!isRealizationWindowConnected)
                    //{
                    //    REPEATER_TRACE_INFO(L"%*s: \tDisconnected Window \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                    //}

                    // The anchor is based on the realization window because a connected ItemsRepeater might intersect the realization window
                    // but not the visible window. In that situation, we still need to produce a valid anchor.
                    FlowLayoutAnchorInfo anchorInfo = m_algorithmCallbacks.Algorithm_GetAnchorForRealizationRect(availableSize, context);
                    anchorIndex = anchorInfo.Index;
                    anchorPosition = m_orientationBasedMeasure.MinorMajorPoint(0, anchorInfo.Offset);
                }
                else
                {
                    //REPEATER_TRACE_INFO(L"%*s: \tConnected Window - picking first realized element as anchor \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                    // No suggestion - just pick first in realized range
                    anchorIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(0);
                    Rect firstElementBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
                    anchorPosition = new Point(firstElementBounds.X, firstElementBounds.Y);
                }
            }

            //REPEATER_TRACE_INFO(L"%*s: \tPicked anchor:%d \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data(), anchorIndex);
            //MUX_ASSERT(anchorIndex == -1 || m_elementManager.IsIndexValidInData(anchorIndex));
            m_firstRealizedDataIndexInsideRealizationWindow = m_lastRealizedDataIndexInsideRealizationWindow = anchorIndex;
            if (m_elementManager.IsIndexValidInData(anchorIndex))
            {
                if (!m_elementManager.IsDataIndexRealized(anchorIndex))
                {
                    // Disconnected, throw everything and create new anchor
                    //REPEATER_TRACE_INFO(L"%*s Disconnected Window - throwing away all realized elements \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                    m_elementManager.ClearRealizedRange();

                    UIElement anchor = m_context.GetOrCreateElementAt(anchorIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
                    m_elementManager.Add(anchor, anchorIndex);
                }

                UIElement anchorElement = m_elementManager.GetRealizedElement(anchorIndex);
                Size desiredSize = MeasureElement(anchorElement, anchorIndex, availableSize, m_context);
                Rect layoutBounds = new Rect(anchorPosition.X, anchorPosition.Y, desiredSize.Width, desiredSize.Height);
                m_elementManager.SetLayoutBoundsForDataIndex(anchorIndex, layoutBounds);

                //REPEATER_TRACE_INFO(L"%*s: \tLayout bounds of anchor %d are (%.0f,%.0f,%.0f,%.0f). \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data(), anchorIndex, layoutBounds.X, layoutBounds.Y, layoutBounds.Width, layoutBounds.Height);
            }
            else
            {
                // Throw everything away
                //REPEATER_TRACE_INFO(L"%*s \tAnchor index is not valid - throwing away all realized elements \n", get_self<VirtualizingLayoutContext>(context)->Indent(), layoutId.data());
                m_elementManager.ClearRealizedRange();
            }

            // TODO: Perhaps we can track changes in the property setter
            m_lastAvailableSize = availableSize;
            m_lastItemSpacing = minItemSpacing;

            return anchorIndex;
        }

        private void Generate(GenerateDirection direction, int anchorIndex, Size availableSize, double minItemSpacing, double lineSpacing, uint maxItemsPerLine, bool disableVirtualization, string layoutId)
        {
            if (anchorIndex != -1)
            {
                int step = (direction == GenerateDirection.Forward) ? 1 : -1;

                //REPEATER_TRACE_INFO(L"%*s: \tGenerating %ls from anchor %d. \n", get_self<VirtualizingLayoutContext>(m_context.get())->Indent(), layoutId.data(), direction == GenerateDirection::Forward ? L"forward" : L"backward", anchorIndex);

                int previousIndex = anchorIndex;
                int currentIndex = anchorIndex + step;
                Rect anchorBounds = m_elementManager.GetLayoutBoundsForDataIndex(anchorIndex);
                double lineOffset = m_orientationBasedMeasure.MajorStart(anchorBounds);
                double lineMajorSize = m_orientationBasedMeasure.MajorSize(anchorBounds);
                uint countInLine = 1;
                bool lineNeedsReposition = false;

                while (m_elementManager.IsIndexValidInData(currentIndex) && (disableVirtualization || ShouldContinueFillingUpSpace(previousIndex, direction)))
                {
                    // Ensure layout element.
                    m_elementManager.EnsureElementRealized(direction == GenerateDirection.Forward, currentIndex, layoutId);
                    UIElement currentElement = m_elementManager.GetRealizedElement(currentIndex);
                    Size desiredSize = MeasureElement(currentElement, currentIndex, availableSize, m_context);

                    // Lay it out.
                    UIElement previousElement = m_elementManager.GetRealizedElement(previousIndex);
                    Rect currentBounds = new Rect(0, 0, desiredSize.Width, desiredSize.Height);
                    Rect previousElementBounds = m_elementManager.GetLayoutBoundsForDataIndex(previousIndex);

                    if (direction == GenerateDirection.Forward)
                    {
                        double remainingSpace = m_orientationBasedMeasure.Minor(availableSize) - (m_orientationBasedMeasure.MinorStart(previousElementBounds) + m_orientationBasedMeasure.MinorSize(previousElementBounds) + minItemSpacing + m_orientationBasedMeasure.Minor(desiredSize));
                        if (countInLine >= maxItemsPerLine || m_algorithmCallbacks.Algorithm_ShouldBreakLine(currentIndex, remainingSpace))
                        {
                            // No more space in this row. wrap to next row.
                            m_orientationBasedMeasure.SetMinorStart(ref currentBounds, 0);
                            m_orientationBasedMeasure.SetMajorStart(ref currentBounds, m_orientationBasedMeasure.MajorStart(previousElementBounds) + lineMajorSize + lineSpacing);

                            if (lineNeedsReposition)
                            {
                                // reposition the previous line (countInLine items)
                                for (uint i = 0; i < countInLine; i++)
                                {
                                    var dataIndex = currentIndex - 1 - i;
                                    Rect bounds = m_elementManager.GetLayoutBoundsForDataIndex((int)dataIndex);
                                    m_orientationBasedMeasure.SetMajorSize(ref bounds, lineMajorSize);
                                    m_elementManager.SetLayoutBoundsForDataIndex((int)dataIndex, bounds);
                                }
                            }

                            // Setup for next line.
                            lineMajorSize = m_orientationBasedMeasure.MajorSize(currentBounds);
                            lineOffset = m_orientationBasedMeasure.MajorStart(currentBounds);
                            lineNeedsReposition = false;
                            countInLine = 1;
                        }
                        else
                        {
                            // More space is available in this row.
                            m_orientationBasedMeasure.SetMinorStart(ref currentBounds, m_orientationBasedMeasure.MinorStart(previousElementBounds) + m_orientationBasedMeasure.MinorSize(previousElementBounds) + minItemSpacing);
                            m_orientationBasedMeasure.SetMajorStart(ref currentBounds, lineOffset);
                            lineMajorSize = Math.Max(lineMajorSize, m_orientationBasedMeasure.MajorSize(currentBounds));
                            lineNeedsReposition = m_orientationBasedMeasure.MajorSize(previousElementBounds) != m_orientationBasedMeasure.MajorSize(currentBounds);
                            countInLine++;
                        }
                    }
                    else
                    {
                        // Backward
                        double remainingSpace = m_orientationBasedMeasure.MinorStart(previousElementBounds) - (m_orientationBasedMeasure.Minor(desiredSize) + minItemSpacing);
                        if (countInLine >= maxItemsPerLine || m_algorithmCallbacks.Algorithm_ShouldBreakLine(currentIndex, remainingSpace))
                        {
                            // Does not fit, wrap to the previous row
                            double availableSizeMinor = m_orientationBasedMeasure.Minor(availableSize);
                            m_orientationBasedMeasure.SetMinorStart(ref currentBounds, double.IsInfinity(availableSizeMinor) ? availableSizeMinor - m_orientationBasedMeasure.Minor(desiredSize) : 0.0);
                            m_orientationBasedMeasure.SetMajorStart(ref currentBounds, lineOffset - m_orientationBasedMeasure.Major(desiredSize) - lineSpacing);

                            if (lineNeedsReposition)
                            {
                                double previousLineOffset = m_orientationBasedMeasure.MajorStart(m_elementManager.GetLayoutBoundsForDataIndex((int)(currentIndex + countInLine + 1)));
                                // reposition the previous line (countInLine items)
                                for (int i = 0; i < countInLine; i++)
                                {
                                    int dataIndex = currentIndex + 1 + (int)i;
                                    if (dataIndex != anchorIndex)
                                    {
                                        Rect bounds = m_elementManager.GetLayoutBoundsForDataIndex(dataIndex);
                                        m_orientationBasedMeasure.SetMajorStart(ref bounds, previousLineOffset - lineMajorSize - lineSpacing);
                                        m_orientationBasedMeasure.SetMajorSize(ref bounds, lineMajorSize);
                                        m_elementManager.SetLayoutBoundsForDataIndex(dataIndex, bounds);
                                        //REPEATER_TRACE_INFO(L"%*s: \t Corrected Layout bounds of element %d are (%.0f,%.0f,%.0f,%.0f). \n", get_self<VirtualizingLayoutContext>(m_context.get())->Indent(), layoutId.data(), dataIndex, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                                    }
                                }
                            }

                            // Setup for next line.
                            lineMajorSize = m_orientationBasedMeasure.MajorSize(currentBounds);
                            lineOffset = m_orientationBasedMeasure.MajorStart(currentBounds);
                            lineNeedsReposition = false;
                            countInLine = 1;
                        }
                        else
                        {
                            // Fits in this row. put it in the previous position
                            m_orientationBasedMeasure.SetMinorStart(ref currentBounds, m_orientationBasedMeasure.MinorStart(previousElementBounds) - m_orientationBasedMeasure.Minor(desiredSize) - minItemSpacing);
                            m_orientationBasedMeasure.SetMajorStart(ref currentBounds, lineOffset);
                            lineMajorSize = Math.Max(lineMajorSize, m_orientationBasedMeasure.MajorSize(currentBounds));
                            lineNeedsReposition = m_orientationBasedMeasure.MajorSize(previousElementBounds) != m_orientationBasedMeasure.MajorSize(currentBounds);
                            countInLine++;
                        }
                    }

                    m_elementManager.SetLayoutBoundsForDataIndex(currentIndex, currentBounds);

                    //REPEATER_TRACE_INFO(L"%*s: \tLayout bounds of element %d are (%.0f,%.0f,%.0f,%.0f). \n", get_self<VirtualizingLayoutContext>(m_context.get())->Indent(), layoutId.data(), currentIndex, currentBounds.X, currentBounds.Y, currentBounds.Width, currentBounds.Height);
                    previousIndex = currentIndex;
                    currentIndex += step;
                }

                // If we did not reach the top or bottom of the extent, we realized one
                // extra item before we knew we were outside the realization window. Do not
                // account for that element in the indicies inside the realization window.
                if (direction == GenerateDirection.Forward)
                {
                    int dataCount = m_context.ItemCount;
                    m_lastRealizedDataIndexInsideRealizationWindow = previousIndex == dataCount - 1 ? dataCount - 1 : previousIndex - 1;
                    m_lastRealizedDataIndexInsideRealizationWindow = Math.Max(0, m_lastRealizedDataIndexInsideRealizationWindow);
                }
                else
                {
                    int dataCount = m_context.ItemCount;
                    m_firstRealizedDataIndexInsideRealizationWindow = previousIndex == 0 ? 0 : previousIndex + 1;
                    m_firstRealizedDataIndexInsideRealizationWindow = Math.Min(dataCount - 1, m_firstRealizedDataIndexInsideRealizationWindow);
                }

                m_elementManager.DiscardElementsOutsideWindow(direction == GenerateDirection.Forward, currentIndex);
            }
        }

        private void MakeAnchor(VirtualizingLayoutContext context, int index, Size availableSize)
        {
            m_elementManager.ClearRealizedRange();
            // FlowLayout requires that the anchor is the first element in the row.
            FlowLayoutAnchorInfo internalAnchor = m_algorithmCallbacks.Algorithm_GetAnchorForTargetElement(index, availableSize, context);
            //MUX_ASSERT(internalAnchor.Index <= index);

            // No need to set the position of the anchor.
            // (0,0) is fine for now since the extent can
            // grow in any direction.
            for (int dataIndex = internalAnchor.Index; dataIndex < index + 1; ++dataIndex)
            {
                UIElement element = context.GetOrCreateElementAt(dataIndex, ElementRealizationOptions.ForceCreate | ElementRealizationOptions.SuppressAutoRecycle);
                element.Measure(m_algorithmCallbacks.Algorithm_GetMeasureSize(dataIndex, availableSize, context));
                m_elementManager.Add(element, dataIndex);
            }
        }

        private bool IsReflowRequired()
        {
            // If first element is realized and is not at the very beginning we need to reflow.
            return
                m_elementManager.GetRealizedElementCount() > 0 &&
                m_elementManager.GetDataIndexFromRealizedRangeIndex(0) == 0 &&
                (m_orientationBasedMeasure.ScrollOrientation == ScrollOrientation.Vertical ?
                m_elementManager.GetLayoutBoundsForRealizedIndex(0).X != 0 :
                m_elementManager.GetLayoutBoundsForRealizedIndex(0).Y != 0);
        }

        private bool ShouldContinueFillingUpSpace(int index, GenerateDirection direction)
        {
            bool shouldContinue = false;
            if (!IsVirtualizingContext())
            {
                shouldContinue = true;
            }
            else
            {
                Rect realizationRect = m_context.RealizationRect;
                Rect elementBounds = m_elementManager.GetLayoutBoundsForDataIndex(index);

                double elementMajorStart = m_orientationBasedMeasure.MajorStart(elementBounds);
                double elementMajorEnd = m_orientationBasedMeasure.MajorEnd(elementBounds);
                double rectMajorStart = m_orientationBasedMeasure.MajorStart(realizationRect);
                double rectMajorEnd = m_orientationBasedMeasure.MajorEnd(realizationRect);

                double elementMinorStart = m_orientationBasedMeasure.MinorStart(elementBounds);
                double elementMinorEnd = m_orientationBasedMeasure.MinorEnd(elementBounds);
                double rectMinorStart = m_orientationBasedMeasure.MinorStart(realizationRect);
                double rectMinorEnd = m_orientationBasedMeasure.MinorEnd(realizationRect);

                // Ensure that both minor and major directions are taken into consideration so that if the scrolling direction
                // is the same as the flow direction we still stop at the end of the viewport rectangle.
                shouldContinue =
                    (direction == GenerateDirection.Forward && elementMajorStart < rectMajorEnd && elementMinorStart < rectMinorEnd) ||
                    (direction == GenerateDirection.Backward && elementMajorEnd > rectMajorStart && elementMinorEnd > rectMinorStart);
            }

            return shouldContinue;
        }

        private Rect EstimateExtent(Size availableSize, string layoutId)
        {
            UIElement firstRealizedElement = null;
            Rect firstBounds = default;
            UIElement lastRealizedElement = null;
            Rect lastBounds = default;
            int firstDataIndex = -1;
            int lastDataIndex = -1;

            if (m_elementManager.GetRealizedElementCount() > 0)
            {
                firstRealizedElement = m_elementManager.GetAt(0);
                firstBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
                firstDataIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(0);

                int last = m_elementManager.GetRealizedElementCount() - 1;
                lastRealizedElement = m_elementManager.GetAt(last);
                lastDataIndex = m_elementManager.GetDataIndexFromRealizedRangeIndex(last);
                lastBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(last);
            }

            Rect extent = m_algorithmCallbacks.Algorithm_GetExtent(availableSize, m_context, firstRealizedElement, firstDataIndex, firstBounds, lastRealizedElement, lastDataIndex, lastBounds);

            // REPEATER_TRACE_INFO(L"%*s Extent: (%.0f,%.0f,%.0f,%.0f). \n", get_self<VirtualizingLayoutContext>(m_context.get())->Indent(), layoutId.data(), extent.X, extent.Y, extent.Width, extent.Height);

            return extent;
        }

        private void RaiseLineArranged()
        {
            Rect realizationRect = RealizationRect();
            if (realizationRect.Width != 0.0f || realizationRect.Height != 0.0f)
            {
                int realizedElementCount = m_elementManager.GetRealizedElementCount();
                if (realizedElementCount > 0)
                {
                    // MUX_ASSERT(m_firstRealizedDataIndexInsideRealizationWindow != -1 && m_lastRealizedDataIndexInsideRealizationWindow != -1);
                    int countInLine = 0;
                    Rect previousElementBounds = m_elementManager.GetLayoutBoundsForDataIndex(m_firstRealizedDataIndexInsideRealizationWindow);
                    double currentLineOffset = m_orientationBasedMeasure.MajorStart(previousElementBounds);
                    double currentLineSize = m_orientationBasedMeasure.MajorSize(previousElementBounds);
                    for (int currentDataIndex = m_firstRealizedDataIndexInsideRealizationWindow; currentDataIndex <= m_lastRealizedDataIndexInsideRealizationWindow; currentDataIndex++)
                    {
                        Rect currentBounds = m_elementManager.GetLayoutBoundsForDataIndex(currentDataIndex);
                        if (m_orientationBasedMeasure.MajorStart(currentBounds) != currentLineOffset)
                        {
                            // Staring a new line
                            m_algorithmCallbacks.Algorithm_OnLineArranged(currentDataIndex - countInLine, countInLine, currentLineSize, m_context);
                            countInLine = 0;
                            currentLineOffset = m_orientationBasedMeasure.MajorStart(currentBounds);
                            currentLineSize = 0;
                        }

                        currentLineSize = Math.Max(currentLineSize, m_orientationBasedMeasure.MajorSize(currentBounds));
                        countInLine++;
                        previousElementBounds = currentBounds;
                    }

                    // Raise for the last line.
                    m_algorithmCallbacks.Algorithm_OnLineArranged(m_lastRealizedDataIndexInsideRealizationWindow - countInLine + 1, countInLine, currentLineSize, m_context);
                }
            }
        }

        #endregion


        #region Arrange related private methods

        private void ArrangeVirtualizingLayout(Size finalSize, LineAlignment lineAlignment, bool isWrapping, string layoutId)
        {
            // Walk through the realized elements one line at a time and
            // align them, Then call element.Arrange with the arranged bounds.
            int realizedElementCount = m_elementManager.GetRealizedElementCount();
            if (realizedElementCount > 0)
            {
                int countInLine = 1;
                Rect previousElementBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(0);
                double currentLineOffset = m_orientationBasedMeasure.MajorStart(previousElementBounds);
                double spaceAtLineStart = m_orientationBasedMeasure.MinorStart(previousElementBounds);
                double spaceAtLineEnd = 0;
                double currentLineSize = m_orientationBasedMeasure.MajorSize(previousElementBounds);
                for (int i = 1; i < realizedElementCount; i++)
                {
                    Rect currentBounds = m_elementManager.GetLayoutBoundsForRealizedIndex(i);
                    if (m_orientationBasedMeasure.MajorStart(currentBounds) != currentLineOffset)
                    {
                        spaceAtLineEnd = m_orientationBasedMeasure.Minor(finalSize) - m_orientationBasedMeasure.MinorStart(previousElementBounds) - m_orientationBasedMeasure.MinorSize(previousElementBounds);
                        PerformLineAlignment(i - countInLine, countInLine, spaceAtLineStart, spaceAtLineEnd, currentLineSize, lineAlignment, isWrapping, finalSize, layoutId);
                        spaceAtLineStart = m_orientationBasedMeasure.MinorStart(currentBounds);
                        countInLine = 0;
                        currentLineOffset = m_orientationBasedMeasure.MajorStart(currentBounds);
                        currentLineSize = 0;
                    }

                    countInLine++; // for current element
                    currentLineSize = Math.Max(currentLineSize, m_orientationBasedMeasure.MajorSize(currentBounds));
                    previousElementBounds = currentBounds;
                }

                // Last line - potentially have a property to customize
                // aligning the last line or not.
                if (countInLine > 0)
                {
                    double spaceAtEnd = m_orientationBasedMeasure.Minor(finalSize) - m_orientationBasedMeasure.MinorStart(previousElementBounds) - m_orientationBasedMeasure.MinorSize(previousElementBounds);
                    PerformLineAlignment(realizedElementCount - countInLine, countInLine, spaceAtLineStart, spaceAtEnd, currentLineSize, lineAlignment, isWrapping, finalSize, layoutId);
                }
            }
        }

        private void PerformLineAlignment(int lineStartIndex, int countInLine, double spaceAtLineStart, double spaceAtLineEnd, double lineSize, FlowLayoutAlgorithm.LineAlignment lineAlignment, bool isWrapping, Size finalSize, string layoutId)
        {
            for (int rangeIndex = lineStartIndex; rangeIndex < lineStartIndex + countInLine; ++rangeIndex)
            {
                Rect bounds = m_elementManager.GetLayoutBoundsForRealizedIndex(rangeIndex);
                m_orientationBasedMeasure.SetMajorSize(ref bounds, lineSize);

                if (!m_scrollOrientationSameAsFlow)
                {
                    // Note: Space at start could potentially be negative
                    if (spaceAtLineStart != 0 || spaceAtLineEnd != 0)
                    {
                        double totalSpace = spaceAtLineStart + spaceAtLineEnd;
                        switch (lineAlignment)
                        {
                            case LineAlignment.Start:
                                {
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) - spaceAtLineStart);
                                    break;
                                }

                            case LineAlignment.End:
                                {
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) + spaceAtLineEnd);
                                    break;
                                }

                            case LineAlignment.Center:
                                {
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) - spaceAtLineStart);
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) + totalSpace / 2);
                                    break;
                                }

                            case LineAlignment.SpaceAround:
                                {
                                    double interItemSpace = countInLine >= 1 ? totalSpace / (countInLine * 2) : 0;
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) - spaceAtLineStart);
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) + interItemSpace * ((rangeIndex - lineStartIndex + 1) * 2 - 1));
                                    break;
                                }

                            case LineAlignment.SpaceBetween:
                                {
                                    double interItemSpace = countInLine > 1 ? totalSpace / (countInLine - 1) : 0;
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) - spaceAtLineStart);
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) + interItemSpace * (rangeIndex - lineStartIndex));
                                    break;
                                }

                            case LineAlignment.SpaceEvenly:
                                {
                                    double interItemSpace = countInLine >= 1 ? totalSpace / (countInLine + 1) : 0;
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) - spaceAtLineStart);
                                    m_orientationBasedMeasure.SetMinorStart(ref bounds, m_orientationBasedMeasure.MinorStart(bounds) + interItemSpace * (rangeIndex - lineStartIndex + 1));
                                    break;
                                }
                        }
                    }
                }

                bounds.X -= m_lastExtent.X;
                bounds.Y -= m_lastExtent.Y;

                if (!isWrapping)
                {
                    m_orientationBasedMeasure.SetMinorSize(ref bounds, Math.Max(m_orientationBasedMeasure.MinorSize(bounds), m_orientationBasedMeasure.Minor(finalSize)));
                }

                UIElement element = m_elementManager.GetAt(rangeIndex);

                //REPEATER_TRACE_INFO(L"%*s: \tArranging element %d at (%.0f,%.0f,%.0f,%.0f). \n", winrt::get_self<VirtualizingLayoutContext>(m_context.get())->Indent(), layoutId.data(), m_elementManager.GetDataIndexFromRealizedRangeIndex(rangeIndex), bounds.X, bounds.Y, bounds.Width, bounds.Height);
                element.Arrange(bounds);
            }
        }

        #endregion

        #region Layout Context Helpers

        private Rect RealizationRect()
        {
            return IsVirtualizingContext() ? m_context.RealizationRect : new Rect(0, 0, double.PositiveInfinity, double.PositiveInfinity);
        }

        private void SetLayoutOrigin()
        {
            if (IsVirtualizingContext())
            {
                m_context.LayoutOrigin = new Point(m_lastExtent.X, m_lastExtent.Y);
            }
            else
            {
                // Should have 0 origin for non-virtualizing layout since we always start from
                // the first item
                // MUX_ASSERT(m_lastExtent.X == 0 && m_lastExtent.Y == 0);
            }
        }

        #endregion

        private bool IsVirtualizingContext()
        {
            if (m_context != null)
            {
                Rect rect = m_context.RealizationRect;
                bool hasInfiniteSize = (double.IsInfinity(rect.Height) || double.IsInfinity(rect.Width));
                return !hasInfiniteSize;
            }
            return false;
        }

        // Fields
        private ElementManager m_elementManager;
        private Size m_lastAvailableSize;
        private double m_lastItemSpacing;
        private bool m_collectionChangePending;
        private VirtualizingLayoutContext m_context;
        private IFlowLayoutAlgorithmDelegates m_algorithmCallbacks = null;
        private Rect m_lastExtent;
        private int m_firstRealizedDataIndexInsideRealizationWindow = -1;
        private int m_lastRealizedDataIndexInsideRealizationWindow = -1;

        // If the scroll orientation is the same as the folow orientation
        // we will only have one line since we will never wrap. In that case
        // we do not want to align the line. We could potentially switch the
        // meaning of line alignment in this case, but I'll hold off on that
        // feature until someone asks for it - This is not a common scenario
        // anyway.
        bool m_scrollOrientationSameAsFlow = false;

        private OrientationBasedMeasures m_orientationBasedMeasure;
    }
}
