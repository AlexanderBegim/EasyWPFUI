// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class InspectingDataSource : ItemsSourceView
    {
        public InspectingDataSource(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is IList vector)
            {
                m_vector = vector;
                ListenToCollectionChanges();
            }
            else if (source is IEnumerable iterable)
            {
                m_vector = WrapIterable(iterable);
            }
            else
            {
                throw new ArgumentException("Argument 'source' is not a supported vector.");
            }

            m_uniqueIdMaping = source as IKeyIndexMapping;
        }

        ~InspectingDataSource()
        {
            UnListenToCollectionChanges();
        }

        #region ItemsSourceViewOverrides

        internal override int GetSizeCore()
        {
            return m_vector.Count;
        }

        internal override object GetAtCore(int index)
        {
            return m_vector[index];
        }

        internal override bool HasKeyIndexMappingCore()
        {
            return m_uniqueIdMaping != null;
        }

        internal override string KeyFromIndexCore(int index)
        {
            if (m_uniqueIdMaping != null)
            {
                return m_uniqueIdMaping.KeyFromIndex(index);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal override int IndexFromKeyCore(string id)
        {
            if (m_uniqueIdMaping != null)
            {
                return m_uniqueIdMaping.IndexFromKey(id);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal override int IndexOfCore(object value)
        {
            int index = -1;

            if (m_vector != null)
            {
                int v;
                if ((v = m_vector.IndexOf(value)) >= 0)
                {
                    index = v;
                }
            }
            return index;
        }

        #endregion


        private IList WrapIterable(IEnumerable iterable)
        {
            IList vector = new List<object>();
            IEnumerator iterator = iterable.GetEnumerator();
            while (iterator.MoveNext())
            {
                vector.Add(iterator.Current);
            }

            return vector;
        }

        private void UnListenToCollectionChanges()
        {
            if (m_vector is INotifyCollectionChanged incc)
            {
                CollectionChangedEventManager.RemoveHandler(incc, OnCollectionChanged);
            }
        }

        private void ListenToCollectionChanges()
        {
            // MUX_ASSERT(m_vector);
            if (m_vector is INotifyCollectionChanged incc)
            {
                CollectionChangedEventManager.AddHandler(incc, OnCollectionChanged);
            }
        }

        private void OnCollectionChanged(object sender /*sender*/, NotifyCollectionChangedEventArgs e)
        {
            OnItemsSourceChanged(e);
        }


        private IList m_vector;

        // To unhook event from data source
        private IKeyIndexMapping m_uniqueIdMaping = null;
    }
}
