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
    /// Логика взаимодействия для ProgressRingPage.xaml
    /// </summary>
    public partial class ProgressRingPage : Page
    {
        public ProgressRingPage()
        {
            InitializeComponent();
        }

        private void ProgressToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if(example1IsActiveSubstitution != null)
            {
                example1IsActiveSubstitution.Value = ProgressToggle.IsOn.ToString();
            }
        }
    }
}
