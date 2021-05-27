// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace EasyWPFUI.Controls
{
    public class CachedVisualTreeHelpers
    {
        public static Rect GetLayoutSlot(FrameworkElement element)
        {
            return LayoutInformation.GetLayoutSlot(element);
        }

        public static DependencyObject GetParent(DependencyObject child)
        {
            if (child is Visual || child is Visual3D)
            {
                return VisualTreeHelper.GetParent(child);
            }
            else if (child is FrameworkContentElement fce)
            {
                return fce.Parent;
            }

            return null;
        }
    }
}
