// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls.Primitives
{
    public class InfoBarPanel : Panel
    {
        private bool m_isVertical = false;

        #region HorizontalOrientationPadding Property

        public static readonly DependencyProperty HorizontalOrientationPaddingProperty = DependencyProperty.Register("HorizontalOrientationPadding", typeof(Thickness), typeof(InfoBarTemplateSettings), new FrameworkPropertyMetadata());

        public Thickness HorizontalOrientationPadding
        {
            get
            {
                return (Thickness)GetValue(HorizontalOrientationPaddingProperty);
            }
            set
            {
                SetValue(HorizontalOrientationPaddingProperty, value);
            }
        }

        #endregion

        #region VerticalOrientationPadding Property

        public static readonly DependencyProperty VerticalOrientationPaddingProperty = DependencyProperty.Register("VerticalOrientationPadding", typeof(Thickness), typeof(InfoBarTemplateSettings), new FrameworkPropertyMetadata());

        public Thickness VerticalOrientationPadding
        {
            get
            {
                return (Thickness)GetValue(VerticalOrientationPaddingProperty);
            }
            set
            {
                SetValue(VerticalOrientationPaddingProperty, value);
            }
        }

        #endregion

        #region HorizontalOrientationMargin Property

        public static readonly DependencyProperty HorizontalOrientationMarginProperty = DependencyProperty.RegisterAttached("HorizontalOrientationMargin", typeof(Thickness), typeof(InfoBarTemplateSettings), new FrameworkPropertyMetadata(OnHorizontalOrientationMarginPropertyChanged));
        
        public static Thickness GetHorizontalOrientationMargin(UIElement ui)
        {
            return (Thickness)ui.GetValue(HorizontalOrientationMarginProperty);
        }

        public static void SetHorizontalOrientationMargin(UIElement ui, Thickness value)
        {
            ui.SetValue(HorizontalOrientationMarginProperty, value);
        }

        private static void OnHorizontalOrientationMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region VerticalOrientationMargin Property

        public static readonly DependencyProperty VerticalOrientationMarginProperty = DependencyProperty.RegisterAttached("VerticalOrientationMargin", typeof(Thickness), typeof(InfoBarTemplateSettings), new FrameworkPropertyMetadata(OnVerticalOrientationMarginPropertyChanged));

        public static Thickness GetVerticalOrientationMargin(UIElement ui)
        {
            return (Thickness)ui.GetValue(VerticalOrientationMarginProperty);
        }

        public static void SetVerticalOrientationMargin(UIElement ui, Thickness value)
        {
            ui.SetValue(VerticalOrientationMarginProperty, value);
        }

        private static void OnVerticalOrientationMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion


        #region Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();

            double totalWidth = 0;
            double totalHeight = 0;
            double widthOfWidest = 0;
            double heightOfTallest = 0;
            double heightOfTallestInHorizontal = 0;
            int nItems = 0;

            FrameworkElement parent = Parent as FrameworkElement;
            double minHeight = parent == null ? 0.0f : (double)(parent.MinHeight - (Margin.Top + Margin.Bottom));

            UIElementCollection children = Children;
            int childCount = (int)children.Count;

            foreach (UIElement child in children)
            {
                child.Measure(availableSize);
                Size childDesiredSize = child.DesiredSize;

                if (childDesiredSize.Width != 0 && childDesiredSize.Height != 0)
                {
                    // Add up the width of all items if they were laid out horizontally
                    Thickness horizontalMargin = InfoBarPanel.GetHorizontalOrientationMargin(child);

                    // Ignore left margin of first and right margin of last child
                    totalWidth += childDesiredSize.Width + (nItems > 0 ? (double)horizontalMargin.Left : 0) + (nItems < childCount - 1 ? (double)horizontalMargin.Right : 0);

                    // Add up the height of all items if they were laid out vertically
                    Thickness verticalMargin = InfoBarPanel.GetVerticalOrientationMargin(child);

                    // Ignore top margin of first and bottom margin of last child
                    totalHeight += childDesiredSize.Height + (nItems > 0 ? verticalMargin.Top : 0) + (nItems < childCount - 1 ? verticalMargin.Bottom : 0);

                    if (childDesiredSize.Width > widthOfWidest)
                    {
                        widthOfWidest = childDesiredSize.Width;
                    }

                    if (childDesiredSize.Height > heightOfTallest)
                    {
                        heightOfTallest = childDesiredSize.Height;
                    }

                    double childHeightInHorizontal = childDesiredSize.Height + horizontalMargin.Top + horizontalMargin.Bottom;
                    if (childHeightInHorizontal > heightOfTallestInHorizontal)
                    {
                        heightOfTallestInHorizontal = childHeightInHorizontal;
                    }

                    nItems++;
                }
            }

            // Since this panel is inside a *-sized grid column, availableSize.Width should not be infinite
            // If there is only one item inside the panel, we will count it as vertical (the margins work out better that way)
            // Also, if the height of any item is taller than the desired min height of the InfoBar,
            // the items should be laid out vertically even though they may seem to fit due to text wrapping.
            if (nItems == 1 || totalWidth > availableSize.Width || (minHeight > 0 && heightOfTallestInHorizontal > minHeight))
            {
                m_isVertical = true;
                Thickness verticalPadding = VerticalOrientationPadding;

                desiredSize.Width = widthOfWidest + verticalPadding.Left + verticalPadding.Right;
                desiredSize.Height = totalHeight + verticalPadding.Top + verticalPadding.Bottom;
            }
            else
            {
                m_isVertical = false;
                Thickness horizontalPadding = HorizontalOrientationPadding;

                desiredSize.Width = totalWidth + horizontalPadding.Left + horizontalPadding.Right;
                desiredSize.Height = heightOfTallest + horizontalPadding.Top + horizontalPadding.Bottom;
            }

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = finalSize;

            if (m_isVertical)
            {
                // Layout elements vertically
                Thickness verticalOrientationPadding = VerticalOrientationPadding;
                double verticalOffset = verticalOrientationPadding.Top;

                bool hasPreviousElement = false;
                foreach (UIElement child in Children)
                {
                    if (child is FrameworkElement childAsFe)
                    {
                        Size desiredSize = child.DesiredSize;
                        if (desiredSize.Width != 0 && desiredSize.Height != 0)
                        {
                            Thickness verticalMargin = InfoBarPanel.GetVerticalOrientationMargin(child);

                            verticalOffset += hasPreviousElement ? verticalMargin.Top : 0;
                            child.Arrange(new Rect(verticalOrientationPadding.Left + verticalMargin.Left, verticalOffset, desiredSize.Width, desiredSize.Height));
                            verticalOffset += desiredSize.Height + verticalMargin.Bottom;

                            hasPreviousElement = true;
                        }
                    }
                }
            }
            else
            {
                // Layout elements horizontally
                Thickness horizontalOrientationPadding = HorizontalOrientationPadding;
                double horizontalOffset = horizontalOrientationPadding.Left;
                bool hasPreviousElement = false;
                foreach (UIElement child in Children)
                {
                    if (child is FrameworkElement childAsFe)
                    {
                        Size desiredSize = child.DesiredSize;
                        if (desiredSize.Width != 0 && desiredSize.Height != 0)
                        {
                            Thickness horizontalMargin = InfoBarPanel.GetHorizontalOrientationMargin(child);

                            horizontalOffset += hasPreviousElement ? horizontalMargin.Left : 0;
                            child.Arrange(new Rect(horizontalOffset, horizontalOrientationPadding.Top + horizontalMargin.Top, desiredSize.Width, desiredSize.Height));
                            horizontalOffset += desiredSize.Width + horizontalMargin.Right;

                            hasPreviousElement = true;
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
