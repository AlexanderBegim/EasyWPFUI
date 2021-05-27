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
    public class RepeaterLayoutContext : VirtualizingLayoutContext
    {
        public RepeaterLayoutContext(ItemsRepeater owner)
        {
            m_owner = new WeakReference<ItemsRepeater>(owner);
        }

        #region ILayoutContextOverrides

        protected override object LayoutStateCore
        {
            get
            {
                return GetOwner().LayoutState;
            }
            set
            {
                GetOwner().LayoutState = value;
            }
        }

        #endregion

        #region IVirtualizingLayoutContextOverrides

        protected override int ItemCountCore()
        {
            var dataSource = GetOwner().ItemsSourceView;
            if (dataSource != null)
            {
                return dataSource.Count;
            }
            return 0;
        }

        protected override UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
        {
            return GetOwner().GetElementImpl(index,
              (options & ElementRealizationOptions.ForceCreate) == ElementRealizationOptions.ForceCreate,
              (options & ElementRealizationOptions.SuppressAutoRecycle) == ElementRealizationOptions.SuppressAutoRecycle);
        }

        protected override object GetItemAtCore(int index)
        {
            return GetOwner().ItemsSourceView.GetAt(index);
        }

        protected override void RecycleElementCore(UIElement element)
        {
            ItemsRepeater owner = GetOwner();
            // REPEATER_TRACE_INFO(L"RepeaterLayout - RecycleElement: %d \n", owner->GetElementIndex(element));
            owner.ClearElementImpl(element);
        }

        protected override Rect RealizationRectCore()
        {
            return GetOwner().RealizationWindow;
        }

        protected override int RecommendedAnchorIndexCore
        {
            get
            {
                int anchorIndex = -1;
                ItemsRepeater repeater = GetOwner();
                UIElement anchor = repeater.SuggestedAnchor;
                if (anchor != null)
                {
                    anchorIndex = repeater.GetElementIndex(anchor);
                }

                return anchorIndex;
            }
        }

        protected override Point LayoutOriginCore
        {
            get
            {
                return GetOwner().LayoutOrigin;
            }
            set
            {
                GetOwner().LayoutOrigin = value;
            }
        }

        #endregion

        private ItemsRepeater GetOwner()
        {
            if(m_owner.TryGetTarget(out ItemsRepeater owner))
            {
                return owner;
            }
            else
            {
                return null;
            }
        }

        // We hold a weak reference to prevent a leaking reference
        // cycle between the ItemsRepeater and its layout.
        private WeakReference<ItemsRepeater> m_owner;
    }
}
