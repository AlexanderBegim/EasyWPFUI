// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class BitmapIconSource : IconSource
    {
        #region UriSource Property

        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapIconSource), new FrameworkPropertyMetadata());

        public Uri UriSource
        {
            get
            {
                return (Uri)GetValue(UriSourceProperty);
            }
            set
            {
                SetValue(UriSourceProperty, value);
            }
        }

        #endregion

        #region ShowAsMonochrome Property

        public static readonly DependencyProperty ShowAsMonochromeProperty = DependencyProperty.Register("ShowAsMonochrome", typeof(bool), typeof(BitmapIconSource), new FrameworkPropertyMetadata(true));

        public bool ShowAsMonochrome
        {
            get
            {
                return (bool)GetValue(ShowAsMonochromeProperty);
            }
            set
            {
                SetValue(ShowAsMonochromeProperty, value);
            }
        }

        #endregion
    }
}
