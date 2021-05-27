// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls
{
    public class FlowLayoutState
    {
        public FlowLayoutState()
        {
            FlowAlgorithm = new FlowLayoutAlgorithm();

            m_lineSizeEstimationBuffer = new List<double>();
            m_itemsPerLineEstimationBuffer = new List<double>();
        }

        internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
        {
            FlowAlgorithm.InitializeForContext(context, callbacks);

            if (m_lineSizeEstimationBuffer.Count == 0)
            {
                m_lineSizeEstimationBuffer.Resize(BufferSize, 0.0);
                m_itemsPerLineEstimationBuffer.Resize(BufferSize, 0.0);
            }

            context.LayoutState = this;
        }

        internal void UninitializeForContext(VirtualizingLayoutContext context)
        {
            FlowAlgorithm.UninitializeForContext(context);
        }

        internal void OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
        {
            // If we do not have any estimation information, use the line for estimation. 
            // If we do have some estimation information, don't account for the last line which is quite likely
            // different from the rest of the lines and can throw off estimation.
            if (TotalLinesMeasured == 0 || startIndex + countInLine != context.ItemCount)
            {
                int estimationBufferIndex = startIndex % m_lineSizeEstimationBuffer.Count;
                bool alreadyMeasured = m_lineSizeEstimationBuffer[estimationBufferIndex] != 0;

                if (!alreadyMeasured)
                {
                    ++TotalLinesMeasured;
                }

                TotalLineSize -= m_lineSizeEstimationBuffer[estimationBufferIndex];
                TotalLineSize += lineSize;
                m_lineSizeEstimationBuffer[estimationBufferIndex] = lineSize;

                TotalItemsPerLine -= m_itemsPerLineEstimationBuffer[estimationBufferIndex];
                TotalItemsPerLine += countInLine;
                m_itemsPerLineEstimationBuffer[estimationBufferIndex] = countInLine;
            }
        }


        internal FlowLayoutAlgorithm FlowAlgorithm { get; private set; }

        internal double TotalLineSize { get; private set; }

        internal int TotalLinesMeasured { get; private set; }

        internal double TotalItemsPerLine { get; private set; }

        internal Size SpecialElementDesiredSize { get; set; }


        private List<double> m_lineSizeEstimationBuffer;
        private List<double> m_itemsPerLineEstimationBuffer;

        private const int BufferSize = 100;
    }
}
