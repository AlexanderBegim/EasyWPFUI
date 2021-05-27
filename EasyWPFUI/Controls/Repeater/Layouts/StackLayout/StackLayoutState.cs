// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls
{
    public class StackLayoutState
    {
        internal StackLayoutState()
        {
            FlowAlgorithm = new FlowLayoutAlgorithm();

            m_estimationBuffer = new List<double>();
        }

        internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
        {
            FlowAlgorithm.InitializeForContext(context, callbacks);
            if (m_estimationBuffer.Count == 0)
            {
                m_estimationBuffer.Resize(BufferSize, 0.0f);
            }

            context.LayoutState = this;
        }

        internal void UninitializeForContext(VirtualizingLayoutContext context)
        {
            FlowAlgorithm.UninitializeForContext(context);
        }

        internal void OnElementMeasured(int elementIndex, double majorSize, double minorSize)
        {
            int estimationBufferIndex = elementIndex % m_estimationBuffer.Count;
            bool alreadyMeasured = m_estimationBuffer[estimationBufferIndex] != 0;
            if (!alreadyMeasured)
            {
                TotalElementsMeasured++;
            }

            TotalElementSize -= m_estimationBuffer[estimationBufferIndex];
            TotalElementSize += majorSize;
            m_estimationBuffer[estimationBufferIndex] = majorSize;

            MaxArrangeBounds = Math.Max(MaxArrangeBounds, minorSize);
        }

        internal void OnMeasureStart()
        {
            MaxArrangeBounds = 0.0;
        }

        internal FlowLayoutAlgorithm FlowAlgorithm { get; private set; }

        internal double TotalElementSize { get; private set; }

        internal double MaxArrangeBounds { get; private set; }

        internal int TotalElementsMeasured { get; private set; }


        private List<double> m_estimationBuffer;

        private const int BufferSize = 100;
    }
}
