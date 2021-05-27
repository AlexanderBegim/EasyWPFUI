// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EasyWPFUI.Converters
{
    public enum CornerRadiusFilterKind
    {
        None,
        Top,
        Right,
        Bottom,
        Left,
        TopLeftValue,
        BottomRightValue
    }

    public class CornerRadiusFilterConverter : IValueConverter
    {
        public CornerRadiusFilterKind Filter { get; set; } = CornerRadiusFilterKind.None;

        public double Scale { get; set; } = 1.0;

        public CornerRadius Convert(CornerRadius radius, CornerRadiusFilterKind filterKind)
        {
            CornerRadius result = radius;

            switch(filterKind)
            {
                case CornerRadiusFilterKind.Top:
                    {
                        result.BottomLeft = 0;
                        result.BottomRight = 0;
                        break;
                    }
                case CornerRadiusFilterKind.Right:
                    {
                        result.TopLeft = 0;
                        result.BottomLeft = 0;
                        break;
                    }
                case CornerRadiusFilterKind.Bottom:
                    {
                        result.TopLeft = 0;
                        result.TopRight = 0;
                        break;
                    }
                case CornerRadiusFilterKind.Left:
                    {
                        result.TopRight = 0;
                        result.BottomRight = 0;
                        break;
                    }
            }

            return result;
        }

        private double GetDoubleValue(CornerRadius radius, CornerRadiusFilterKind filterKind)
        {
            switch (filterKind)
            {
                case CornerRadiusFilterKind.TopLeftValue:
                    {
                        return radius.TopLeft;
                    }
                case CornerRadiusFilterKind.BottomRightValue:
                    {
                        return radius.BottomRight;
                    }
            }

            return 0;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius cornerRadius = (CornerRadius)value;

            double scale = Scale;

            if (double.IsNaN(scale))
            {
                cornerRadius.TopLeft *= scale;
                cornerRadius.TopRight *= scale;
                cornerRadius.BottomRight *= scale;
                cornerRadius.BottomLeft *= scale;
            }

            CornerRadiusFilterKind filterType = Filter;

            if(filterType == CornerRadiusFilterKind.TopLeftValue || filterType == CornerRadiusFilterKind.BottomRightValue)
            {
                return GetDoubleValue(cornerRadius, Filter);
            }

            return Convert(cornerRadius, Filter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
