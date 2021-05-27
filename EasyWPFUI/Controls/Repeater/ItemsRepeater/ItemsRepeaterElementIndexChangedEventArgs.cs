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
    public class ItemsRepeaterElementIndexChangedEventArgs : EventArgs
    {
        public ItemsRepeaterElementIndexChangedEventArgs(UIElement element, int oldIndex, int newIndex)
        {
            Update(element, oldIndex, newIndex);
        }

        public void Update(UIElement element, int oldIndex, int newIndex)
        {
            Element = element;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public UIElement Element { get; private set; }
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
    }
}
