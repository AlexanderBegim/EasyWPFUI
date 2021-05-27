//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Controls.Helpers;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для ScrollViewerPage.xaml
    /// </summary>
    public partial class ScrollViewerPage : Page
    {
        public ScrollViewerPage()
        {
            InitializeComponent();
        }

        private void example1HorizontalSBVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(example1HorizontalSBVisibilitySubstitution != null)
            {
                example1HorizontalSBVisibilitySubstitution.Value = scrollViewerExample1.HorizontalScrollBarVisibility.ToString();
            }
        }

        private void example1VerticalSBVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(example1VerticalSBVisibilitySubstitution != null)
            {
                example1VerticalSBVisibilitySubstitution.Value = scrollViewerExample1.VerticalScrollBarVisibility.ToString();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (example1allowAutoHideSubstitution != null)
            {
                example1allowAutoHideSubstitution.Value = ((bool)scrollViewerExample1.GetValue(ScrollViewerHelper.AllowAutoHideProperty)).ToString();
            }
        }
    }
}
