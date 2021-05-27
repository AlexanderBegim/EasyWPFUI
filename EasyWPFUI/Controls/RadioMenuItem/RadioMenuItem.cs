// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class RadioMenuItem : MenuItem
    {
        #region GroupNameProperty

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(RadioMenuItem), new FrameworkPropertyMetadata(OnGroupNamePropertyChanged));

        public string GroupName
        {
            get
            {
                return (string)GetValue(GroupNameProperty);
            }
            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        private static void OnGroupNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadioMenuItem)d).UpdateSiblings();
        }

        #endregion


        #region Methods

        static RadioMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioMenuItem), new FrameworkPropertyMetadata(typeof(RadioMenuItem)));
        }

        public RadioMenuItem()
        {
            IsCheckable = true;
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if(m_isIgnoreCheck)
            {
                e.Handled = true;
            }
            else
            {
                UpdateSiblings();
            }

            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if(!m_isSafeUncheck)
            {
                m_isIgnoreCheck = true;
                IsChecked = true;
                m_isIgnoreCheck = false;

                e.Handled = true;
            }

            base.OnUnchecked(e);
        }


        private void UpdateSiblings()
        {
            if (IsChecked)
            {
                // Since this item is checked, uncheck all siblings
                if (ItemsControlFromItemContainer(this) is ItemsControl parent)
                {
                    int childrenCount = parent.Items.Count;
                    for (int i = 0; i < childrenCount; i++)
                    {
                        object child = parent.Items[i];
                        if (child is RadioMenuItem radioItem)
                        {
                            if (radioItem != this && radioItem.GroupName == GroupName)
                            {
                                radioItem.m_isSafeUncheck = true;
                                radioItem.IsChecked = false;
                                radioItem.m_isSafeUncheck = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        bool m_isSafeUncheck = false;
        bool m_isIgnoreCheck = false;
    }
}
