// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class Layout : DependencyObject
    {
        public event EventHandler<object> MeasureInvalidated;
        public event EventHandler<object> ArrangeInvalidated;

        #region ILayout

        // For debugging purposes only.
        public string LayoutId { get; set; }

        public void InitializeForContext(LayoutContext context)
        {
            if (this is IVirtualizingLayoutOverrides virtualizingLayout)
            {
                VirtualizingLayoutContext virtualizingContext = GetVirtualizingLayoutContext(context);
                virtualizingLayout.InitializeForContextCore(virtualizingContext);
            }
            else if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayout)
            {
                NonVirtualizingLayoutContext nonVirtualizingContext = GetNonVirtualizingLayoutContext(context);
                nonVirtualizingLayout.InitializeForContextCore(nonVirtualizingContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void UninitializeForContext(LayoutContext context)
        {
            if (this is IVirtualizingLayoutOverrides virtualizingLayout)
            {
                VirtualizingLayoutContext virtualizingContext = GetVirtualizingLayoutContext(context);
                virtualizingLayout.UninitializeForContextCore(virtualizingContext);
            }
            else if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayout)
            {
                NonVirtualizingLayoutContext nonVirtualizingContext = GetNonVirtualizingLayoutContext(context);
                nonVirtualizingLayout.UninitializeForContextCore(nonVirtualizingContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private VirtualizingLayoutContext GetVirtualizingLayoutContext(LayoutContext context)
        {
            if (context is VirtualizingLayoutContext virtualizingContext)
            {
                return virtualizingContext;
            }
            else if (context is NonVirtualizingLayoutContext nonVirtualizingContext)
            {
                VirtualizingLayoutContext adapter = nonVirtualizingContext.GetVirtualizingContextAdapter();
                return adapter;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private NonVirtualizingLayoutContext GetNonVirtualizingLayoutContext(LayoutContext context)
        {
            if (context is NonVirtualizingLayoutContext nonVirtualizingContext)
            {
                return nonVirtualizingContext;
            }
            else if (context is VirtualizingLayoutContext virtualizingContext)
            {
                NonVirtualizingLayoutContext adapter = virtualizingContext.GetNonVirtualizingContextAdapter();
                return adapter;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Size Measure(LayoutContext context, Size availableSize)
        {
            if (this is IVirtualizingLayoutOverrides virtualizingLayout)
            {
                VirtualizingLayoutContext virtualizingContext = GetVirtualizingLayoutContext(context);
                return virtualizingLayout.MeasureOverride(virtualizingContext, availableSize);
            }
            else if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayout)
            {
                NonVirtualizingLayoutContext nonVirtualizingContext = GetNonVirtualizingLayoutContext(context);
                return nonVirtualizingLayout.MeasureOverride(nonVirtualizingContext, availableSize);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Size Arrange(LayoutContext context, Size finalSize)
        {
            if (this is IVirtualizingLayoutOverrides virtualizingLayout)
            {
                VirtualizingLayoutContext virtualizingContext = GetVirtualizingLayoutContext(context);
                return virtualizingLayout.ArrangeOverride(virtualizingContext, finalSize);
            }
            else if (this is INonVirtualizingLayoutOverrides nonVirtualizingLayout)
            {
                NonVirtualizingLayoutContext nonVirtualizingContext = GetNonVirtualizingLayoutContext(context);
                return nonVirtualizingLayout.ArrangeOverride(nonVirtualizingContext, finalSize);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ILayoutProtected

        public void InvalidateMeasure()
        {
            MeasureInvalidated?.Invoke(this, EventArgs.Empty);
        }

        public void InvalidateArrange()
        {
            ArrangeInvalidated?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }
}
