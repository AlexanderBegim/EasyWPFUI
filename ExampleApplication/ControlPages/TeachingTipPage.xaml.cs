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
    /// Логика взаимодействия для TeachingTipPage.xaml
    /// </summary>
    public partial class TeachingTipPage : Page
    {
        public TeachingTipPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.Current?.PageHeader != null)
            {
                MainWindow.Current.PageHeader.ToggleThemeTeachingTip1.IsOpen = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Current?.PageHeader != null)
            {
                MainWindow.Current.PageHeader.ToggleThemeTeachingTip2.IsOpen = true;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(MainWindow.Current?.PageHeader != null)
            {
                MainWindow.Current.PageHeader.ToggleThemeTeachingTip3.IsOpen = true;
            }
        }
    }
}
