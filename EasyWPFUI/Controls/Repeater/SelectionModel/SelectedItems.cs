using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class SelectedItems<T> : IReadOnlyList<T>
    {
        public SelectedItems(List<SelectedItemInfo> infos, Func<List<SelectedItemInfo>, int, T> getAtImpl)
        {
            m_infos = infos;
            m_getAtImpl = getAtImpl;

            foreach (SelectedItemInfo info in infos)
            {
                if (info.Node.TryGetTarget(out SelectionNode node))
                {
                    m_totalCount += node.SelectedCount;
                }
                else
                {
                    throw new Exception("Selection changed after the SelectedIndices/Items property was read.");
                }
            }
        }

        ~SelectedItems()
        {
            m_infos.Clear();
        }

        #region IReadOnlyList<T>

        public T this[int index]
        {
            get
            {
                return m_getAtImpl(m_infos, index);
            }
        }

        public int Count
        {
            get
            {
                return m_totalCount;
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
            public Iterator(IReadOnlyList<T> selectedItems)
            {
                m_selectedItems = selectedItems;
            }

            public T Current
            {
                get
                {
                    IReadOnlyList<T> items = m_selectedItems;

                    if(m_currentIndex < items.Count)
                    {
                        return items[m_currentIndex];
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (m_currentIndex < m_selectedItems.Count)
                {
                    ++m_currentIndex;
                    return (m_currentIndex < m_selectedItems.Count);
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }

            public void Reset()
            {
                m_currentIndex = 1;
            }

            IReadOnlyList<T> m_selectedItems = null;
            int m_currentIndex = -1;
        }

        private List<SelectedItemInfo> m_infos;
        private int m_totalCount = 0;
        private Func<List<SelectedItemInfo>, int /*index*/, T> m_getAtImpl;
    }
}
