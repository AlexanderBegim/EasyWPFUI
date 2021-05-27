// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class IndexRange
    {
        public IndexRange()
        {

        }

        public IndexRange(int begin, int end)
        {
            // Accept out of order begin/end pairs, just swap them.
            if (begin > end)
            {
                int temp = begin;
                begin = end;
                end = temp;
            }

            // MUX_ASSERT(begin <= end);

            m_begin = begin;
            m_end = end;
        }

        public int Begin
        {
            get
            {
                return m_begin;
            }
        }

        public int End
        {
            get
            {
                return m_end;
            }
        }

        public bool Contains(int index)
        {
            return index >= m_begin && index <= m_end;
        }

        public bool Split(int splitIndex, ref IndexRange before, ref IndexRange after)
        {
            // MUX_ASSERT(Contains(splitIndex));

            bool afterIsValid;

            before = new IndexRange(m_begin, splitIndex);
            if (splitIndex < m_end)
            {
                after = new IndexRange(splitIndex + 1, m_end);
                afterIsValid = true;
            }
            else
            {
                after = new IndexRange();
                afterIsValid = false;
            }

            return afterIsValid;
        }

        public bool Intersects(IndexRange other)
        {
            return ((m_begin <= other.End) && (m_end >= other.Begin));
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(IndexRange lhs, IndexRange rhs)
        {
            return lhs.m_begin == rhs.m_begin && lhs.m_end == rhs.m_end;
        }

        public static bool operator !=(IndexRange lhs, IndexRange rhs)
        {
            return !(lhs == rhs);
        }

        private int m_begin = -1;
        private int m_end = -1;
    }
}
