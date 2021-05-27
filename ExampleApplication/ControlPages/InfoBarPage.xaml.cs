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

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для InfoBarPage.xaml
    /// </summary>
    public partial class InfoBarPage : Page
    {
        public InfoBarPage()
        {
            InitializeComponent();
        }

        private void infoBarExample1IsOpen_Click(object sender, RoutedEventArgs e)
        {
            if(infoBarExample1IsOpenSubstitution != null)
            {
                infoBarExample1IsOpenSubstitution.Value = infoBarExample1.IsOpen.ToString();
            }
        }

        private void infoBarExample1Severity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(infoBarExample1SeveritySubstitution != null)
            {
                infoBarExample1SeveritySubstitution.Value = infoBarExample1.Severity.ToString();
            }
        }


        private void infoBarExample2IsOpen_Click(object sender, RoutedEventArgs e)
        {
            if (infoBarExample2IsOpenSubstitution != null)
            {
                infoBarExample2IsOpenSubstitution.Value = infoBarExample2.IsOpen.ToString();
            }
        }

        private void infoBarExample2MessageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(infoBarExample2DisplayMessageSubstitution != null && infoBarExample2 != null)
            {
                if (infoBarExample2MessageComboBox.SelectedIndex == 0) // short
                {
                    string shortMessage = "A short essential app message.";
                    infoBarExample2.Message = shortMessage;
                    infoBarExample2DisplayMessageSubstitution.Value = shortMessage;
                }
                else if (infoBarExample2MessageComboBox.SelectedIndex == 1) //long
                {
                    infoBarExample2.Message = @"A long essential app message for your users to be informed of, acknowledge, or take action on. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin dapibus dolor vitae justo rutrum, ut lobortis nibh mattis. Aenean id elit commodo, semper felis nec.";
                    infoBarExample2DisplayMessageSubstitution.Value = "A long essential app message...";
                }
            }
        }

        private void infoBarExample2ActionButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(infoBarExample2DisplayButtonSubstitution != null && infoBarExample2 != null)
            {
                if (infoBarExample2ActionButtonComboBox.SelectedIndex == 0) // none
                {
                    infoBarExample2.ActionButton = null;
                    infoBarExample2DisplayButtonSubstitution.Value = string.Empty;
                }
                else if (infoBarExample2ActionButtonComboBox.SelectedIndex == 1) // button
                {
                    var button = new Button();
                    button.Content = "Action";
                    infoBarExample2.ActionButton = button;
                    infoBarExample2DisplayButtonSubstitution.Value = @"<ui:InfoBar.ActionButton>
        <Button Content=""Action"" Click=""InfoBarButton_Click"" />
    </ui:InfoBar.ActionButton> ";

                }
                //            else if (infoBarExample2ActionButtonComboBox.SelectedIndex == 2) // hyperlink
                //            {
                //                var link = new HyperlinkButton();
                //                link.NavigateUri = new Uri("http://www.microsoft.com/");
                //                link.Content = "Informational link";
                //                infoBarExample2.ActionButton = link;
                //                infoBarExample2DisplayButtonSubstitution.Value = @"<ui:InfoBar.ActionButton>
                //    <HyperlinkButton Content=""Informational link"" NavigateUri=""https://www.example.com"" />
                //</ui:InfoBar.ActionButton>";
                //            }
            }
        }


        private void IsOpenCheckBox3_Click(object sender, RoutedEventArgs e)
        {
            if (infoBarExample3IsOpenSubstitution != null)
            {
                infoBarExample3IsOpenSubstitution.Value = infoBarExample3.IsOpen.ToString();
            }
        }

        private void IsIconVisibleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (infoBarExample3IsIconVisibleSubstitution != null)
            {
                infoBarExample3IsIconVisibleSubstitution.Value = infoBarExample3.IsIconVisible.ToString();
            }
        }

        private void IsClosableCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(infoBarExample3IsClosableSubstitution != null)
            {
                infoBarExample3IsClosableSubstitution.Value = infoBarExample3.IsClosable.ToString();
            }
        }
    }
}
