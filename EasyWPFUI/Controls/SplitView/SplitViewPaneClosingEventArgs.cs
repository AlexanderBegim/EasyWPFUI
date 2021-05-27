// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace EasyWPFUI.Controls
{
    public sealed class SplitViewPaneClosingEventArgs : EventArgs
    {
        internal SplitViewPaneClosingEventArgs()
        {

        }

        public bool Cancel { get; set; }
    }
}
