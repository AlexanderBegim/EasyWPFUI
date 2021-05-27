// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls.Primitives
{
    public class ProgressRingTemplateSettings : DependencyObject
    {
        #region EllipseDiameter Property

        public static readonly DependencyPropertyKey EllipseDiameterPropertyKey = DependencyProperty.RegisterReadOnly("EllipseDiameter", typeof(double), typeof(ProgressRingTemplateSettings), new FrameworkPropertyMetadata(0d));

        public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;

        public double EllipseDiameter
        {
            get
            {
                return (double)GetValue(EllipseDiameterProperty);
            }
            internal set
            {
                SetValue(EllipseDiameterPropertyKey, value);
            }
        }

        #endregion

        #region EllipseOffset Property

        public static readonly DependencyPropertyKey EllipseOffsetPropertyKey = DependencyProperty.RegisterReadOnly("EllipseOffset", typeof(Thickness), typeof(ProgressRingTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;

        public Thickness EllipseOffset
        {
            get
            {
                return (Thickness)GetValue(EllipseOffsetProperty);
            }
            internal set
            {
                SetValue(EllipseOffsetPropertyKey, value);
            }
        }

        #endregion

        #region MaxSideLength Property

        public static readonly DependencyPropertyKey MaxSideLengthPropertyKey = DependencyProperty.RegisterReadOnly("MaxSideLength", typeof(double), typeof(ProgressRingTemplateSettings), new FrameworkPropertyMetadata(0d));

        public static readonly DependencyProperty MaxSideLengthProperty = MaxSideLengthPropertyKey.DependencyProperty;

        public double MaxSideLength
        {
            get
            {
                return (double)GetValue(MaxSideLengthProperty);
            }
            internal set
            {
                SetValue(MaxSideLengthPropertyKey, value);
            }
        }

        #endregion
    }
}
