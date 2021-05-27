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
    /// Логика взаимодействия для ListBoxPage.xaml
    /// </summary>
    public partial class ListBoxPage : Page
    {
        public List<Tuple<string, FontFamily>> Fonts { get; } = new List<Tuple<string, FontFamily>>()
        {
            new Tuple<string, FontFamily>("Arial", new FontFamily("Arial")),
            new Tuple<string, FontFamily>("Comic Sans MS", new FontFamily("Comic Sans MS")),
            new Tuple<string, FontFamily>("Courier New", new FontFamily("Courier New")),
            new Tuple<string, FontFamily>("Segoe UI", new FontFamily("Segoe UI")),
            new Tuple<string, FontFamily>("Times New Roman", new FontFamily("Times New Roman"))
        };

        public ListBoxPage()
        {
            InitializeComponent();
        }

        private void listBoxExample1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorName = e.AddedItems[0].ToString();
            switch (colorName)
            {
                case "Yellow":
                    listBoxExample1Rectangle.Fill = new SolidColorBrush(Colors.Yellow);
                    break;
                case "Green":
                    listBoxExample1Rectangle.Fill = new SolidColorBrush(Colors.Green);
                    break;
                case "Blue":
                    listBoxExample1Rectangle.Fill = new SolidColorBrush(Colors.Blue);
                    break;
                case "Red":
                    listBoxExample1Rectangle.Fill = new SolidColorBrush(Colors.Red);
                    break;
            }
        }

        /* *** */

        private void listBoxExample2_Loaded(object sender, RoutedEventArgs e)
        {
            listBoxExample2.SelectedIndex = 2;
        }

        private void listBoxExample2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxExample2Output.FontFamily = ((Tuple<string, FontFamily>)(((ListBox)sender).SelectedItem)).Item2;
        }
    }
}
