// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class ElementFactoryRecycleArgs
    {
        private UIElement m_element;
        private UIElement m_parent;

        #region IElementFactoryRecycleArgs

        public UIElement Element
        {
            get
            {
                return m_element;
            }
            set
            {
                m_element = value;
            }
        }

        public UIElement Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        #endregion
    }
}
