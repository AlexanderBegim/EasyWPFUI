// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EasyWPFUI.Converters
{
    public enum CornerRadiusToThicknessConverterKind
    {
        FilterTopAndBottomFromLeft,
        FilterTopAndBottomFromRight,
        FilterLeftAndRightFromTop,
        FilterLeftAndRightFromBottom,
        FilterTopFromTopLeft,
        FilterTopFromTopRight,
        FilterRightFromTopRight,
        FilterRightFromBottomRight,
        FilterBottomFromBottomRight,
        FilterBottomFromBottomLeft,
        FilterLeftFromBottomLeft,
        FilterLeftFromTopLeft,
    }

    public class CornerRadiusToThicknessConverter : IValueConverter
    {
        public CornerRadiusToThicknessConverterKind ConversionKind { get; set; } = CornerRadiusToThicknessConverterKind.FilterLeftAndRightFromTop;

        public double Multiplier { get; set; } = 1.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius radius = (CornerRadius)value;

            return Convert(radius, ConversionKind, Multiplier);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Thickness Convert(CornerRadius radius, CornerRadiusToThicknessConverterKind filterKind, double multiplier)
        {
            Thickness result = new Thickness();

            switch (filterKind)
            {
                case CornerRadiusToThicknessConverterKind.FilterLeftAndRightFromTop:
                    {
                        result.Left = radius.TopLeft * multiplier;
                        result.Right = radius.TopRight * multiplier;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterLeftAndRightFromBottom:
                    {
                        result.Left = radius.BottomLeft * multiplier;
                        result.Right = radius.BottomRight * multiplier;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterTopAndBottomFromLeft:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = radius.TopLeft * multiplier;
                        result.Bottom = radius.BottomLeft * multiplier;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterTopAndBottomFromRight:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = radius.TopRight * multiplier;
                        result.Bottom = radius.BottomRight * multiplier;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterTopFromTopLeft:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = radius.TopLeft * multiplier;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterTopFromTopRight:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = radius.TopRight * multiplier;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterRightFromTopRight:
                    {
                        result.Left = 0;
                        result.Right = radius.TopRight * multiplier;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterRightFromBottomRight:
                    {
                        result.Left = 0;
                        result.Right = radius.BottomRight * multiplier;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterBottomFromBottomRight:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = 0;
                        result.Bottom = radius.BottomRight * multiplier;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterBottomFromBottomLeft:
                    {
                        result.Left = 0;
                        result.Right = 0;
                        result.Top = 0;
                        result.Bottom = radius.BottomLeft * multiplier;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterLeftFromBottomLeft:
                    {
                        result.Left = radius.BottomLeft * multiplier;
                        result.Right = 0;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
                case CornerRadiusToThicknessConverterKind.FilterLeftFromTopLeft:
                    {
                        result.Left = radius.TopLeft * multiplier;
                        result.Right = 0;
                        result.Top = 0;
                        result.Bottom = 0;
                        break;
                    }
            }

            return result;
        }
    }
}
