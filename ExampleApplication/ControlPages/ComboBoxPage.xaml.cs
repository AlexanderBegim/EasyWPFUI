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
    /// Логика взаимодействия для ComboBoxPage.xaml
    /// </summary>
    public partial class ComboBoxPage : Page
    {
        public List<Tuple<string, FontFamily>> Fonts { get; } = new List<Tuple<string, FontFamily>>()
        {
            new Tuple<string, FontFamily>("Arial", new FontFamily("Arial")),
            new Tuple<string, FontFamily>("Comic Sans MS", new FontFamily("Comic Sans MS")),
            new Tuple<string, FontFamily>("Courier New", new FontFamily("Courier New")),
            new Tuple<string, FontFamily>("Segoe UI", new FontFamily("Segoe UI")),
            new Tuple<string, FontFamily>("Times New Roman", new FontFamily("Times New Roman"))
        };

        public List<double> FontSizes { get; } = new List<double>()
        {
            8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 36, 48, 72
        };


        public ComboBoxPage()
        {
            InitializeComponent();
        }

        private void comboBoxExample1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorName = e.AddedItems[0].ToString();
            Color color = Colors.Transparent;
            switch (colorName)
            {
                case "Yellow":
                    color = Colors.Yellow;
                    break;
                case "Green":
                    color = Colors.Green;
                    break;
                case "Blue":
                    color = Colors.Blue;
                    break;
                case "Red":
                    color = Colors.Red;
                    break;
            }
            comboBoxExample1Output.Fill = new SolidColorBrush(color);
        }

        /* *** */

        private void comboBoxExample2_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxExample2.SelectedIndex = 2;
        }

        private void comboBoxExample2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBoxExample2Output.FontFamily = ((Tuple<string, FontFamily>)comboBoxExample2.SelectedItem).Item2;
        }

        /* *** */

        private void comboBoxExample3_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxExample3.SelectedIndex = 2;
        }

        private void comboBoxExample3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(comboBoxExample3.Text, out double value))
            {
                comboBoxExample3Output.FontSize = value;
            }
        }
    }
}
