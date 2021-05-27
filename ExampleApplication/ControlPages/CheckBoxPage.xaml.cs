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

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для CheckBoxPage.xaml
    /// </summary>
    public partial class CheckBoxPage : Page
    {
        public CheckBoxPage()
        {
            InitializeComponent();

            Loaded += CheckBoxPage_Loaded;
        }

        private void CheckBoxPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetCheckedState();
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxExample1Output.Text = "You checked the box.";
        }

        private void CheckBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxExample1Output.Text = "You unchecked the box.";
        }

        /* *** */

        private void CheckBox2_Indeterminate(object sender, RoutedEventArgs e)
        {
            checkBoxExample2Output.Text = "CheckBox state is indeterminate.";
        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxExample2Output.Text = "CheckBox is checked.";
        }

        private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxExample2Output.Text = "CheckBox is unchecked.";
        }

        /* *** */

        private void OptionsAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Option1CheckBox.IsChecked = Option2CheckBox.IsChecked = Option3CheckBox.IsChecked = true;
        }

        private void OptionsAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Option1CheckBox.IsChecked = Option2CheckBox.IsChecked = Option3CheckBox.IsChecked = false;
        }

        private void OptionsAllCheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            // If the SelectAll box is checked (all options are selected),
            // clicking the box will change it to its indeterminate state.
            // Instead, we want to uncheck all the boxes,
            // so we do this programatically. The indeterminate state should
            // only be set programatically, not by the user.

            if (Option1CheckBox.IsChecked == true &&
                Option2CheckBox.IsChecked == true &&
                Option3CheckBox.IsChecked == true)
            {
                // This will cause SelectAll_Unchecked to be executed, so
                // we don't need to uncheck the other boxes here.
                OptionsAllCheckBox.IsChecked = false;
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            SetCheckedState();
        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {
            SetCheckedState();
        }

        private void SetCheckedState()
        {
            // Controls are null the first time this is called, so we just
            // need to perform a null check on any one of the controls.
            if (Option1CheckBox != null)
            {
                if (Option1CheckBox.IsChecked == true &&
                    Option2CheckBox.IsChecked == true &&
                    Option3CheckBox.IsChecked == true)
                {
                    OptionsAllCheckBox.IsChecked = true;
                }
                else if (Option1CheckBox.IsChecked == false &&
                    Option2CheckBox.IsChecked == false &&
                    Option3CheckBox.IsChecked == false)
                {
                    OptionsAllCheckBox.IsChecked = false;
                }
                else
                {
                    // Set third state (indeterminate) by setting IsChecked to null.
                    OptionsAllCheckBox.IsChecked = null;
                }
            }
        }
    }
}
