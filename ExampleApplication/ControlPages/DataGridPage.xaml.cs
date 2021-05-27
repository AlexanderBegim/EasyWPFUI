// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
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
using ExampleApplication.Data;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для DataGridPage.xaml
    /// </summary>
    public partial class DataGridPage : Page
    {
        private DataGridDataSource viewModel = new DataGridDataSource();

        public List<DataGridDataItem> Items { get; set; }

        public DataGridPage()
        {
            InitializeComponent();

            DataContext = this;

            Items = new List<DataGridDataItem>(viewModel.GetDataAsync().Result);

            Loaded += DataGridPage_Loaded;
        }

        private async void DataGridPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridComboBoxColumn comboBox = dataGrid.Columns.FirstOrDefault(c => c is DataGridComboBoxColumn) as DataGridComboBoxColumn;

            if (comboBox != null)
            {
                comboBox.ItemsSource = await viewModel.GetMountains();
            }
        }
    }
}
