// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class TeachingTipTemplateSettings : DependencyObject
    {
        #region TopRightHighlightMargin Property

        public static readonly DependencyPropertyKey TopRightHighlightMarginPropertyKey = DependencyProperty.RegisterReadOnly("TopRightHighlightMargin", typeof(Thickness), typeof(TeachingTipTemplateSettings), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty TopRightHighlightMarginProperty = TopRightHighlightMarginPropertyKey.DependencyProperty;

        public Thickness TopRightHighlightMargin
        {
            get
            {
                return (Thickness)GetValue(TopRightHighlightMarginProperty);
            }
            internal set
            {
                SetValue(TopRightHighlightMarginPropertyKey, value);
            }
        }

        #endregion

        #region TopLeftHighlightMargin Property

        public static readonly DependencyPropertyKey TopLeftHighlightMarginPropertyKey = DependencyProperty.RegisterReadOnly("TopLeftHighlightMargin", typeof(Thickness), typeof(TeachingTipTemplateSettings), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty TopLeftHighlightMarginProperty = TopLeftHighlightMarginPropertyKey.DependencyProperty;

        public Thickness TopLeftHighlightMargin
        {
            get
            {
                return (Thickness)GetValue(TopLeftHighlightMarginProperty);
            }
            internal set
            {
                SetValue(TopLeftHighlightMarginPropertyKey, value);
            }
        }

        #endregion

        #region IconElement Property

        public static readonly DependencyPropertyKey IconElementPropertyKey = DependencyProperty.RegisterReadOnly("IconElement", typeof(object), typeof(TeachingTipTemplateSettings), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IconElementProperty = IconElementPropertyKey.DependencyProperty;

        public object IconElement
        {
            get
            {
                return GetValue(IconElementProperty);
            }
            internal set
            {
                SetValue(IconElementPropertyKey, value);
            }
        }

        #endregion
    }
}
