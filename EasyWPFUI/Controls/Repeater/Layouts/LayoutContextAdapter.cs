// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class LayoutContextAdapter : VirtualizingLayoutContext
    {
        private WeakReference<NonVirtualizingLayoutContext> m_nonVirtualizingContext = null;

        public LayoutContextAdapter(NonVirtualizingLayoutContext nonVirtualizingContext)
        {
            m_nonVirtualizingContext = new WeakReference<NonVirtualizingLayoutContext>(nonVirtualizingContext);
        }

        #region ILayoutContextOverrides

        protected override object LayoutStateCore
        {
            get
            {
                if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
                {
                    return context.LayoutState;
                }

                return null;
            }
            set
            {
                if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
                {
                    context.LayoutState = value;
                }
            }
        }

        #endregion

        #region IVirtualizingLayoutContextOverrides

        protected override int ItemCountCore()
        {
            if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
            {
                return context.Children.Count;
            }

            return 0;
        }

        protected override object GetItemAtCore(int index)
        {
            if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
            {
                return context.Children[index];
            }

            return null;
        }

        protected override UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
        {
            if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
            {
                return context.Children[index];
            }

            return null;
        }

        protected override void RecycleElementCore(UIElement element)
        {

        }

        public int GetElementIndexCore(UIElement element)
        {
            if (m_nonVirtualizingContext.TryGetTarget(out NonVirtualizingLayoutContext context))
            {
                IReadOnlyList<UIElement> children = context.Children;

                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == element)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        protected override Rect RealizationRectCore()
        {
            return new Rect(0, 0, double.PositiveInfinity, double.PositiveInfinity);
        }

        protected override int RecommendedAnchorIndexCore
        {
            get
            {
                return -1;
            }
        }

        protected override Point LayoutOriginCore
        {
            get
            {
                return new Point(0, 0);
            }
            set
            {
                if (value != new Point(0, 0))
                {
                    throw new ArgumentOutOfRangeException("LayoutOrigin must be at (0,0) when RealizationRect is infinite sized.");
                }
            }
        }

        #endregion
    }
}
