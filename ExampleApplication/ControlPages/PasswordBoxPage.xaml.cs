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
    /// Логика взаимодействия для PasswordBoxPage.xaml
    /// </summary>
    public partial class PasswordBoxPage : Page
    {
        public PasswordBoxPage()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                if (string.IsNullOrEmpty(pb.Password) || pb.Password == "Password")
                {
                    Control1Output.Visibility = Visibility.Visible;
                    Control1Output.Text = "'Password' is not allowed.";
                    pb.Password = string.Empty;
                }
                else
                {
                    Control1Output.Text = string.Empty;
                    Control1Output.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
