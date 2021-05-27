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
    public delegate void ConfigurationChangedEventHandler(IRepeaterScrollingSurface sender);

    public delegate void PostArrangeEventHandler(IRepeaterScrollingSurface sender);

    public delegate void ViewportChangedEventHandler(IRepeaterScrollingSurface sender, bool isFinal);

    public interface IRepeaterScrollingSurface
    {
        bool IsHorizontallyScrollable { get; }
        bool IsVerticallyScrollable { get; }
        UIElement AnchorElement { get; }
        event ConfigurationChangedEventHandler ConfigurationChanged;
        event PostArrangeEventHandler PostArrange;
        event ViewportChangedEventHandler ViewportChanged;
        void RegisterAnchorCandidate(UIElement element);
        void UnregisterAnchorCandidate(UIElement element);
        Rect GetRelativeViewport(UIElement child);
    }
}
