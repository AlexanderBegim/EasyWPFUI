﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EasyWPFUI.Controls
{
    internal enum ScrollOrientation
    {
        Vertical,
        Horizontal
    }

    internal class OrientationBasedMeasures
    {
        public ScrollOrientation ScrollOrientation { get; set; } = ScrollOrientation.Vertical;

        // Major - Scrolling/virtualizing direction
        // Minor - Opposite direction
        public double Major(Size size)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? size.Height : size.Width;
        }

        public double Minor(Size size)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? size.Width : size.Height;
        }

        public double MajorSize(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.Height : rect.Width;
        }

        public void SetMajorSize(ref Rect rect, double value)
        {
            if (ScrollOrientation == ScrollOrientation.Vertical)
            {
                rect.Height = value;
            }
            else
            {
                rect.Width = value;
            }
        }

        public double MinorSize(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.Width : rect.Height;
        }

        public void SetMinorSize(ref Rect rect, double value)
        {
            if (ScrollOrientation == ScrollOrientation.Vertical)
            {
                rect.Width = value;
            }
            else
            {
                rect.Height = value;
            }
        }

        public double MajorStart(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.Y : rect.X;
        }

        public void SetMajorStart(ref Rect rect, double value)
        {
            if (ScrollOrientation == ScrollOrientation.Vertical)
            {
                rect.Y = value;
            }
            else
            {
                rect.X = value;
            }
        }

        public double MajorEnd(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.Y + rect.Height : rect.X + rect.Width;
        }

        public double MinorStart(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.X : rect.Y;
        }

        public void SetMinorStart(ref Rect rect, double value)
        {
            if (ScrollOrientation == ScrollOrientation.Vertical)
            {
                rect.X = value;
            }
            else
            {
                rect.Y = value;
            }
        }

        public double MinorEnd(Rect rect)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? rect.X + rect.Width : rect.Y + rect.Height;
        }

        public Rect MinorMajorRect(double minor, double major, double minorSize, double majorSize)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? new Rect(minor, major, minorSize, majorSize) : new Rect(major, minor, majorSize, minorSize);
        }

        public Point MinorMajorPoint(double minor, double major)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? new Point(minor, major) : new Point(major, minor);
        }

        public Size MinorMajorSize(double minor, double major)
        {
            return ScrollOrientation == ScrollOrientation.Vertical ? new Size(minor, major) : new Size(major, minor);
        }
    }
}