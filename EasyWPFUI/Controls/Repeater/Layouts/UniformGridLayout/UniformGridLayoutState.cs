// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class UniformGridLayoutState
    {
        public UniformGridLayoutState()
        {
            FlowAlgorithm = new FlowLayoutAlgorithm();

            EffectiveItemWidth = 0;
            EffectiveItemHeight = 0;
        }

        internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
        {
            FlowAlgorithm.InitializeForContext(context, callbacks);
            context.LayoutState = this; // TODO
        }

        internal void UninitializeForContext(VirtualizingLayoutContext context)
        {
            FlowAlgorithm.UninitializeForContext(context);
        }

        internal FlowLayoutAlgorithm FlowAlgorithm { get; }

        internal double EffectiveItemWidth { get; private set; }

        internal double EffectiveItemHeight { get; private set; }

        internal void EnsureElementSize(Size availableSize, VirtualizingLayoutContext context, double layoutItemWidth, double LayoutItemHeight, UniformGridLayoutItemsStretch stretch, Orientation orientation, double minRowSpacing, double minColumnSpacing, uint maxItemsPerLine)
        {
            if (maxItemsPerLine == 0)
            {
                maxItemsPerLine = 1;
            }

            if (context.ItemCount > 0)
            {
                // If the first element is realized we don't need to get it from the context
                if (FlowAlgorithm.GetElementIfRealized(0) is UIElement realizedElement)
                {
                    realizedElement.Measure(availableSize);
                    SetSize(realizedElement.DesiredSize, layoutItemWidth, LayoutItemHeight, availableSize, stretch, orientation, minRowSpacing, minColumnSpacing, maxItemsPerLine);
                }
                else
                {
                    // Not realized by flowlayout, so do this now!
                    if (context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate) is UIElement firstElement)
                    {
                        firstElement.Measure(availableSize);
                        SetSize(firstElement.DesiredSize, layoutItemWidth, LayoutItemHeight, availableSize, stretch, orientation, minRowSpacing, minColumnSpacing, maxItemsPerLine);
                        context.RecycleElement(firstElement);
                    }
                }
            }
        }




        private void SetSize(Size desiredItemSize, double layoutItemWidth, double LayoutItemHeight, Size availableSize, UniformGridLayoutItemsStretch stretch, Orientation orientation, double minRowSpacing, double minColumnSpacing, uint maxItemsPerLine)
        {
            if (maxItemsPerLine == 0)
            {
                maxItemsPerLine = 1;
            }

            EffectiveItemWidth = (double.IsNaN(layoutItemWidth) ? desiredItemSize.Width : layoutItemWidth);
            EffectiveItemHeight = (double.IsNaN(LayoutItemHeight) ? desiredItemSize.Height : LayoutItemHeight);

            double availableSizeMinor = orientation == Orientation.Horizontal ? availableSize.Width : availableSize.Height;
            double minorItemSpacing = orientation == Orientation.Vertical ? minRowSpacing : minColumnSpacing;

            double itemSizeMinor = orientation == Orientation.Horizontal ? EffectiveItemWidth : EffectiveItemHeight;

            double extraMinorPixelsForEachItem = 0.0;
            if (double.IsInfinity(availableSizeMinor))
            {
                double numItemsPerColumn = Math.Min(maxItemsPerLine, Math.Max(1.0, availableSizeMinor / (itemSizeMinor + minorItemSpacing)));
                double usedSpace = (numItemsPerColumn * (itemSizeMinor + minorItemSpacing)) - minorItemSpacing;
                int remainingSpace = ((int)(availableSizeMinor - usedSpace));
                extraMinorPixelsForEachItem = remainingSpace / ((int)numItemsPerColumn);
            }

            if (stretch == UniformGridLayoutItemsStretch.Fill)
            {
                if (orientation == Orientation.Horizontal)
                {
                    EffectiveItemWidth += extraMinorPixelsForEachItem;
                }
                else
                {
                    EffectiveItemHeight+= extraMinorPixelsForEachItem;
                }
            }
            else if (stretch == UniformGridLayoutItemsStretch.Uniform)
            {
                double itemSizeMajor = orientation == Orientation.Horizontal ? EffectiveItemHeight : EffectiveItemWidth;
                double extraMajorPixelsForEachItem = itemSizeMajor * (extraMinorPixelsForEachItem / itemSizeMinor);
                if (orientation == Orientation.Horizontal)
                {
                    EffectiveItemWidth += extraMinorPixelsForEachItem;
                    EffectiveItemHeight += extraMajorPixelsForEachItem;
                }
                else
                {
                    EffectiveItemHeight += extraMinorPixelsForEachItem;
                    EffectiveItemWidth += extraMajorPixelsForEachItem;
                }
            }
        }
    }
}
