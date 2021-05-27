// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    internal class UniqueIdElementPool : IEnumerable<KeyValuePair<string, UIElement>>
    {
        public UniqueIdElementPool(ItemsRepeater owner)
        {
            m_owner = owner;

            m_elementMap = new Dictionary<string, UIElement>();
        }

        public void Add(UIElement element)
        {
            //MUX_ASSERT(m_owner->ItemsSourceView().HasKeyIndexMapping());

            VirtualizationInfo virtInfo = ItemsRepeater.GetVirtualizationInfo(element);
            string key = virtInfo.UniqueId;

            if (m_elementMap.ContainsKey(key))
            {
                throw new Exception($"The unique id provided ({virtInfo.UniqueId}) is not unique.");
            }

            m_elementMap.Add(key, element);
        }

        public UIElement Remove(int index)
        {
            //MUX_ASSERT(m_owner->ItemsSourceView().HasKeyIndexMapping());

            // Check if there is already a element in the mapping and if so, use it.
            UIElement element = null;
            string key = m_owner.ItemsSourceView.KeyFromIndex(index);

            if (m_elementMap.ContainsKey(key))
            {
                element = m_elementMap[key];
                m_elementMap.Remove(key);
            }

            return element;
        }

        public void Clear()
        {

        }

        public bool IsEmpty
        {
            get
            {
                return m_elementMap.Count == 0;
            }
        }


        public IEnumerator<KeyValuePair<string, UIElement>> GetEnumerator()
        {
            return m_elementMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ItemsRepeater m_owner = null;
        private Dictionary<string, UIElement> m_elementMap;
    }
}
