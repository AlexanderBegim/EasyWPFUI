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
    public interface INonVirtualizingLayoutOverrides
    {
        void InitializeForContextCore(LayoutContext context);
        void UninitializeForContextCore(LayoutContext context);
        Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize);
        Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize);
    }

    public class NonVirtualizingLayout : Layout, INonVirtualizingLayoutOverrides
    {
        protected virtual void InitializeForContextCore(LayoutContext context)
        {

        }

        protected virtual void UninitializeForContextCore(LayoutContext context)
        {

        }

        protected virtual Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
        {
            throw new NotImplementedException();
        }

        protected virtual Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
        {
            throw new NotImplementedException();
        }

        void INonVirtualizingLayoutOverrides.InitializeForContextCore(LayoutContext context)
        {
            InitializeForContextCore(context);
        }

        void INonVirtualizingLayoutOverrides.UninitializeForContextCore(LayoutContext context)
        {
            UninitializeForContextCore(context);
        }

        Size INonVirtualizingLayoutOverrides.MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
        {
            return MeasureOverride(context, availableSize);
        }

        Size INonVirtualizingLayoutOverrides.ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
        {
            return ArrangeOverride(context, finalSize);
        }
    }
}
