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
using EasyWPFUI.Controls.Helpers;
using EasyWPFUI.Controls.Primitives;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для FlyoutPage.xaml
    /// </summary>
    public partial class FlyoutPage : Page
    {
        public FlyoutPage()
        {
            InitializeComponent();
        }

        private void flyoutExample1Placement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(flyoutExample1 != null)
            {
                flyoutExample1.Placement = (FlyoutPlacementMode)flyoutExample1Placement.SelectedItem;
            }
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if(ButtonHelper.GetFlyout(flyoutExample1Button) is Flyout flyout)
            {
                flyout.Hide();
            }
        }
    }
}
