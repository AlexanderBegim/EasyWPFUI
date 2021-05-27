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
    /// Логика взаимодействия для CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();
        }

        private void calendarExample1IsTodayHighlighted_Checked(object sender, RoutedEventArgs e)
        {
            if(calendarExample1IsTodayHighlightedSubstitution != null)
            {
                calendarExample1IsTodayHighlightedSubstitution.Value = calendarExample1IsTodayHighlighted.IsChecked.ToString();
            }
        }

        private void calendarExample1SelectionMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(calendarExample1SelectionModeSubstitution != null)
            {
                calendarExample1SelectionModeSubstitution.Value = calendarExample1SelectionMode.SelectedItem.ToString();
            }
        }
    }
}
