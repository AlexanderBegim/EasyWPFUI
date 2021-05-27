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
    public class ItemsRepeaterElementClearingEventArgs : EventArgs
    {
        public ItemsRepeaterElementClearingEventArgs(UIElement element)
        {
            Update(element);
        }

        public void Update(UIElement element)
        {
            Element = element;
        }

        public UIElement Element { get; private set; }
    }
}
