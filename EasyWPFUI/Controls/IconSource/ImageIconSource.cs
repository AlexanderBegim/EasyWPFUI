// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class ImageIconSource : IconSource
    {
        #region ImageSource Property

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageIconSource), new FrameworkPropertyMetadata());

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        #endregion
    }
}
