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
    public class NonVirtualizingLayoutContext : LayoutContext
    {
        private VirtualizingLayoutContext m_contextAdapter = null;

        #region INonVirtualizingLayoutContext

        public IReadOnlyList<UIElement> Children
        {
            get
            {
                return ChildrenCore;
            }
        }

        #endregion

        #region INonVirtualizingLayoutContextOverrides

        protected virtual IReadOnlyList<UIElement> ChildrenCore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        public VirtualizingLayoutContext GetVirtualizingContextAdapter()
        {
            if(m_contextAdapter == null)
            {
                m_contextAdapter = new LayoutContextAdapter(this);
            }

            return m_contextAdapter;
        }
    }
}
