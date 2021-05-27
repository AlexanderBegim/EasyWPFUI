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
    public class ElementFactoryGetArgs
    {
        private int m_index;
        private object m_data;
        private UIElement m_parent;

        #region IElementFactoryGetArgs

        public object Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
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

        public int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }
    }
}
