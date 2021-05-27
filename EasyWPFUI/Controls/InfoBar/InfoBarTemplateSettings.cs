// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class InfoBarTemplateSettings : DependencyObject
    {
        #region IconElement

        public static readonly DependencyPropertyKey IconElementPropertyKey = DependencyProperty.RegisterReadOnly("iconElement", typeof(IconElement), typeof(InfoBarTemplateSettings), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IconElementProperty = IconElementPropertyKey.DependencyProperty;

        public IconElement IconElement
        {
            get
            {
                return (IconElement)GetValue(IconElementProperty);
            }
            set
            {
                SetValue(IconElementPropertyKey, value);
            }
        }

        #endregion
    }
}
