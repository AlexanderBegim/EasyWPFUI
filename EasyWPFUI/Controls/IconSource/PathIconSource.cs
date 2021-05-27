// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class PathIconSource : IconSource
    {
        #region Data Property

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(BitmapIconSource), new FrameworkPropertyMetadata());

        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        #endregion
    }
}
