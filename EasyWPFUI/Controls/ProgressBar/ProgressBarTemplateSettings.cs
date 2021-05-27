// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls.Primitives
{
    public class ProgressBarTemplateSettings : DependencyObject
    {
        #region ContainerAnimationStartPosition Property

        private static readonly DependencyPropertyKey ContainerAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationStartPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty ContainerAnimationStartPositionProperty = ContainerAnimationStartPositionPropertyKey.DependencyProperty;

        public double ContainerAnimationStartPosition
        {
            get
            {
                return (double)GetValue(ContainerAnimationStartPositionProperty);
            }
            set
            {
                SetValue(ContainerAnimationStartPositionPropertyKey, value);
            }
        }

        #endregion

        #region ContainerAnimationEndPosition Property

        private static readonly DependencyPropertyKey ContainerAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty ContainerAnimationEndPositionProperty = ContainerAnimationEndPositionPropertyKey.DependencyProperty;

        public double ContainerAnimationEndPosition
        {
            get
            {
                return (double)GetValue(ContainerAnimationEndPositionProperty);
            }
            set
            {
                SetValue(ContainerAnimationEndPositionPropertyKey, value);
            }
        }

        #endregion

        #region Container2AnimationStartPosition Property

        private static readonly DependencyPropertyKey Container2AnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("Container2AnimationStartPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty Container2AnimationStartPositionProperty = Container2AnimationStartPositionPropertyKey.DependencyProperty;

        public double Container2AnimationStartPosition
        {
            get
            {
                return (double)GetValue(Container2AnimationStartPositionProperty);
            }
            set
            {
                SetValue(Container2AnimationStartPositionPropertyKey, value);
            }
        }

        #endregion

        #region Container2AnimationEndPosition Property

        private static readonly DependencyPropertyKey Container2AnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("Container2AnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty Container2AnimationEndPositionProperty = Container2AnimationEndPositionPropertyKey.DependencyProperty;

        public double Container2AnimationEndPosition
        {
            get
            {
                return (double)GetValue(Container2AnimationEndPositionProperty);
            }
            set
            {
                SetValue(Container2AnimationEndPositionPropertyKey, value);
            }
        }

        #endregion

        #region ContainerAnimationMidPosition Property

        private static readonly DependencyPropertyKey ContainerAnimationMidPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationMidPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty ContainerAnimationMidPositionProperty = ContainerAnimationMidPositionPropertyKey.DependencyProperty;

        public double ContainerAnimationMidPosition
        {
            get
            {
                return (double)GetValue(ContainerAnimationMidPositionProperty);
            }
            set
            {
                SetValue(ContainerAnimationMidPositionPropertyKey, value);
            }
        }

        #endregion

        #region IndicatorLengthDelta Property

        private static readonly DependencyPropertyKey IndicatorLengthDeltaPropertyKey = DependencyProperty.RegisterReadOnly("IndicatorLengthDelta", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty IndicatorLengthDeltaProperty = IndicatorLengthDeltaPropertyKey.DependencyProperty;

        public double IndicatorLengthDelta
        {
            get
            {
                return (double)GetValue(IndicatorLengthDeltaProperty);
            }
            set
            {
                SetValue(IndicatorLengthDeltaPropertyKey, value);
            }
        }

        #endregion

        #region ClipRect Property

        private static readonly DependencyPropertyKey ClipRectPropertyKey = DependencyProperty.RegisterReadOnly("ClipRect", typeof(RectangleGeometry), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty ClipRectProperty = ClipRectPropertyKey.DependencyProperty;

        public RectangleGeometry ClipRect
        {
            get
            {
                return (RectangleGeometry)GetValue(ClipRectProperty);
            }
            set
            {
                SetValue(ClipRectPropertyKey, value);
            }
        }

        #endregion

        #region EllipseAnimationEndPosition Property

        private static readonly DependencyPropertyKey EllipseAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("EllipseAnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty EllipseAnimationEndPositionProperty = EllipseAnimationEndPositionPropertyKey.DependencyProperty;

        public double EllipseAnimationEndPosition
        {
            get
            {
                return (double)GetValue(EllipseAnimationEndPositionProperty);
            }
            set
            {
                SetValue(EllipseAnimationEndPositionPropertyKey, value);
            }
        }

        #endregion

        #region EllipseAnimationWellPosition Property

        private static readonly DependencyPropertyKey EllipseAnimationWellPositionPropertyKey = DependencyProperty.RegisterReadOnly("EllipseAnimationWellPosition", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty EllipseAnimationWellPositionProperty = EllipseAnimationWellPositionPropertyKey.DependencyProperty;

        public double EllipseAnimationWellPosition
        {
            get
            {
                return (double)GetValue(EllipseAnimationWellPositionProperty);
            }
            set
            {
                SetValue(EllipseAnimationWellPositionPropertyKey, value);
            }
        }

        #endregion

        #region EllipseDiameter Property

        private static readonly DependencyPropertyKey EllipseDiameterPropertyKey = DependencyProperty.RegisterReadOnly("EllipseDiameter", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;

        public double EllipseDiameter
        {
            get
            {
                return (double)GetValue(EllipseDiameterProperty);
            }
            set
            {
                SetValue(EllipseDiameterPropertyKey, value);
            }
        }

        #endregion

        #region EllipseOffset Property

        private static readonly DependencyPropertyKey EllipseOffsetPropertyKey = DependencyProperty.RegisterReadOnly("EllipseOffset", typeof(double), typeof(ProgressBarTemplateSettings), null);

        public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;

        public double EllipseOffset
        {
            get
            {
                return (double)GetValue(EllipseOffsetProperty);
            }
            set
            {
                SetValue(EllipseOffsetPropertyKey, value);
            }
        }

        #endregion


        #region Methods

        public ProgressBarTemplateSettings()
        {

        }

        #endregion
    }
}
