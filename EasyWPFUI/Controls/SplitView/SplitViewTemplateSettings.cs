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
    public class SplitViewTemplateSettings : DependencyObject
    {
        private SplitView splitView;

        #region OpenPaneLength Property

        private static readonly DependencyPropertyKey OpenPaneLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty OpenPaneLengthProperty = OpenPaneLengthPropertyKey.DependencyProperty;

        public double OpenPaneLength
        {
            get
            {
                return (double)GetValue(OpenPaneLengthProperty);
            }
            set
            {
                SetValue(OpenPaneLengthPropertyKey, value);
            }
        }

        #endregion

        #region OpenPaneGridLength Property

        private static readonly DependencyPropertyKey OpenPaneGridLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty OpenPaneGridLengthProperty = OpenPaneGridLengthPropertyKey.DependencyProperty;

        public GridLength OpenPaneGridLength
        {
            get
            {
                return (GridLength)GetValue(OpenPaneGridLengthProperty);
            }
            internal set => SetValue(OpenPaneGridLengthPropertyKey, value);
        }

        #endregion

        #region OpenPaneLengthMinusCompactLength Property

        private static readonly DependencyPropertyKey OpenPaneLengthMinusCompactLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty OpenPaneLengthMinusCompactLengthProperty = OpenPaneLengthMinusCompactLengthPropertyKey.DependencyProperty;

        public double OpenPaneLengthMinusCompactLength
        {
            get
            {
                return (double)GetValue(OpenPaneLengthMinusCompactLengthProperty);
            }
            set
            {
                SetValue(OpenPaneLengthMinusCompactLengthPropertyKey, value);
            }
        }

        #endregion

        #region CompactPaneGridLength Property

        private static readonly DependencyPropertyKey CompactPaneGridLengthPropertyKey = DependencyProperty.RegisterReadOnly("CompactPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CompactPaneGridLengthProperty = CompactPaneGridLengthPropertyKey.DependencyProperty;

        public GridLength CompactPaneGridLength
        {
            get
            {
                return (GridLength)GetValue(CompactPaneGridLengthProperty);
            }
            set
            {
                SetValue(CompactPaneGridLengthPropertyKey, value);
            }
        }

        #endregion

        #region NegativeOpenPaneLength Property

        private static readonly DependencyPropertyKey NegativeOpenPaneLengthPropertyKey = DependencyProperty.RegisterReadOnly("NegativeOpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty NegativeOpenPaneLengthProperty = NegativeOpenPaneLengthPropertyKey.DependencyProperty;

        public double NegativeOpenPaneLength
        {
            get
            {
                return (double)GetValue(NegativeOpenPaneLengthProperty);
            }
            set
            {
                SetValue(NegativeOpenPaneLengthPropertyKey, value);
            }
        }

        #endregion

        #region NegativeOpenPaneLengthMinusCompactLength Property

        private static readonly DependencyPropertyKey NegativeOpenPaneLengthMinusCompactLengthPropertyKey = DependencyProperty.RegisterReadOnly("NegativeOpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty NegativeOpenPaneLengthMinusCompactLengthProperty = NegativeOpenPaneLengthMinusCompactLengthPropertyKey.DependencyProperty;

        public double NegativeOpenPaneLengthMinusCompactLength
        {
            get
            {
                return (double)GetValue(NegativeOpenPaneLengthMinusCompactLengthProperty);
            }
            set
            {
                SetValue(NegativeOpenPaneLengthMinusCompactLengthPropertyKey, value);
            }
        }

        #endregion

        internal SplitViewTemplateSettings(SplitView splitView)
        {
            this.splitView = splitView;
        }

        internal void Update()
        {
            OpenPaneLength = splitView.OpenPaneLength;
            OpenPaneGridLength = new GridLength(splitView.OpenPaneLength);
            OpenPaneLengthMinusCompactLength = splitView.OpenPaneLength - splitView.CompactPaneLength;
            CompactPaneGridLength = new GridLength(splitView.CompactPaneLength);
            NegativeOpenPaneLength = -splitView.OpenPaneLength;
            NegativeOpenPaneLengthMinusCompactLength = -(splitView.OpenPaneLength - splitView.CompactPaneLength);
        }
    }
}
