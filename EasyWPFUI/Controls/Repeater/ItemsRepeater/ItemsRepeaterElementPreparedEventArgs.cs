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
    public class ItemsRepeaterElementPreparedEventArgs : EventArgs
    {
        public ItemsRepeaterElementPreparedEventArgs(UIElement element, int index)
        {
            Update(element, index);
        }

        public void Update(UIElement element, int index)
        {
            Element = element;
            Index = index;
        }

        public UIElement Element { get; private set; }
        public int Index { get; private set; }
    }
}
