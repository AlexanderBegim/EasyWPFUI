// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using EasyWPFUI.Common;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class IconSource : DependencyObject
    {
        #region Foreground Property

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("ForegroundProperty", typeof(Brush), typeof(IconSource), new FrameworkPropertyMetadata());

        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        #endregion

        #region Methods

        public static IconElement MakeIconElementFrom(IconSource iconSource)
        {
            return SharedHelpers.MakeIconElementFrom(iconSource);
        }

        public virtual DependencyProperty GetIconElementPropertyCore(DependencyProperty sourceProperty)
        {
            if (sourceProperty == ForegroundProperty)
            {
                return SymbolIcon.SymbolProperty;
            }

            return null;
        }

        #endregion
    }
}
