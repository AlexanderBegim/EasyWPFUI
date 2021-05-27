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
    /// Логика взаимодействия для CompactSizingPage.xaml
    /// </summary>
    public partial class CompactSizingPage : Page
    {
        private ResourceDictionary compactDictionary;

        public CompactSizingPage()
        {
            InitializeComponent();
        }

        private void Standart_Checked(object sender, RoutedEventArgs e)
        {
            if(compactDictionary != null && Resources.MergedDictionaries.Contains(compactDictionary))
            {
                Resources.MergedDictionaries.Remove(compactDictionary);
            }
        }

        private void Compact_Checked(object sender, RoutedEventArgs e)
        {
            if(compactDictionary == null)
            {
                compactDictionary = new ResourceDictionary()
                {
                    Source = new Uri($"pack://application:,,,/EasyWPFUI;component/DensityStyles/Compact.xaml")
                };
            }

            Resources.MergedDictionaries.Add(compactDictionary);
        }
    }
}
