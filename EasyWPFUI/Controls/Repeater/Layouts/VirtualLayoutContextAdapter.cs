// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class VirtualLayoutContextAdapter : NonVirtualizingLayoutContext
    {
        private WeakReference<VirtualizingLayoutContext> m_virtualizingContext = null;
        private IReadOnlyList<UIElement> m_children = null;

        public VirtualLayoutContextAdapter(VirtualizingLayoutContext virtualizingContext)
        {
            m_virtualizingContext = new WeakReference<VirtualizingLayoutContext>(virtualizingContext);
        }

        #region ILayoutContextOverrides

        protected override object LayoutStateCore
        {
            get
            {
                if(m_virtualizingContext.TryGetTarget(out VirtualizingLayoutContext target))
                {
                    return target.LayoutState;
                }

                return null;
            }
            set
            {
                if(m_virtualizingContext.TryGetTarget(out VirtualizingLayoutContext target))
                {
                    target.LayoutState = value;
                }
            }
        }

        #endregion

        #region INonVirtualizingLayoutContextOverrides

        protected override IReadOnlyList<UIElement> ChildrenCore
        {
            get
            {
                if(m_children == null)
                {
                    m_virtualizingContext.TryGetTarget(out var context);
                    m_children = new ChildrenCollection<UIElement>(context);
                }

                return m_children;
            }
        }

        #endregion

        private class ChildrenCollection<T> : IReadOnlyList<T>, IEnumerable<T> where T : UIElement
        {
            private VirtualizingLayoutContext m_context;

            public ChildrenCollection(VirtualizingLayoutContext context)
            {
                m_context = context;
            }

            #region IReadOnlyList<T>

            public T this[int index]
            {
                get
                {
                    return (T)m_context.GetOrCreateElementAt(index, ElementRealizationOptions.None);
                }
            }

            public int Count
            {
                get
                {
                    return m_context.ItemCount;
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Iterator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            private class Iterator : IEnumerator<T>
            {
                private  IReadOnlyList<T> m_childCollection;
                private int m_currentIndex = 0;

                public Iterator(IReadOnlyList<T> childCollection)
                {
                    m_childCollection = childCollection;
                }

                ~Iterator()
                {

                }

                public T Current
                {
                    get
                    {
                        if (m_currentIndex < m_childCollection.Count)
                        {
                            return m_childCollection[m_currentIndex];
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }

                object IEnumerator.Current => Current;

                public void Dispose()
                {

                }

                public bool MoveNext()
                {
                    if (m_currentIndex < m_childCollection.Count)
                    {
                        ++m_currentIndex;
                        return (m_currentIndex < m_childCollection.Count);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                public void Reset()
                {

                }
            }
        }
    }
}
