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
    public class VirtualizingLayoutContext : LayoutContext
    {
        private NonVirtualizingLayoutContext m_contextAdapter = null;

        #region IVirtualizingLayoutContext

        public int ItemCount
        {
            get
            {
                return ItemCountCore();
            }
        }

        public object GetItemAt(int index)
        {
            return GetItemAtCore(index);
        }

        public UIElement GetOrCreateElementAt(int index)
        {
            return GetOrCreateElementAtCore(index, ElementRealizationOptions.None);
        }

        public UIElement GetOrCreateElementAt(int index, ElementRealizationOptions options)
        {
            return GetOrCreateElementAtCore(index, options);
        }

        public void RecycleElement(UIElement element)
        {
            RecycleElementCore(element);
        }

        public Rect RealizationRect
        {
            get
            {
                return RealizationRectCore();
            }
        }

        public int RecommendedAnchorIndex
        {
            get
            {
                return RecommendedAnchorIndexCore;
            }
        }

        public Point LayoutOrigin
        {
            get
            {
                return LayoutOriginCore;
            }
            set
            {
                LayoutOriginCore = value;
            }
        }

        #endregion

        #region IVirtualizingLayoutContextOverrides

        protected virtual int ItemCountCore()
        {
            throw new NotImplementedException();
        }

        protected virtual object GetItemAtCore(int index)
        {
            throw new NotImplementedException();
        }

        protected virtual UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
        {
            throw new NotImplementedException();
        }

        protected virtual void RecycleElementCore(UIElement element)
        {
            throw new NotImplementedException();
        }

        protected virtual Rect RealizationRectCore()
        {
            throw new NotImplementedException();
        }

        protected virtual int RecommendedAnchorIndexCore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected virtual Point LayoutOriginCore
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        public NonVirtualizingLayoutContext GetNonVirtualizingContextAdapter()
        {
            if (m_contextAdapter == null)
            {
                m_contextAdapter = new VirtualLayoutContextAdapter(this);
            }

            return m_contextAdapter;
        }
    }
}
