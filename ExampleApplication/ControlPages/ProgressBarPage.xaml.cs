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
    /// Логика взаимодействия для ProgressBarPage.xaml
    /// </summary>
    public partial class ProgressBarPage : Page
    {
        public ProgressBarPage()
        {
            InitializeComponent();
        }

        private void OnProgressBarExample1RadioButtonClick(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            string content = radio.Content.ToString();

            if (content == "Running")
            {
                progressBarExample1.ShowPaused = false;
                progressBarExample1.ShowError = false;
            }
            else if (content == "Paused")
            {
                progressBarExample1.ShowPaused = true;
                progressBarExample1.ShowError = false;
            }
            else
            {
                progressBarExample1.ShowPaused = false;
                progressBarExample1.ShowError = true;
            }

            if (progressBarExample1ShowPausedSubstitution != null)
            {
                progressBarExample1ShowPausedSubstitution.Value = progressBarExample1PausedRB.IsChecked.ToString();
            }

            if(progressBarExample1ShowErrorSubstitution != null)
            {
                progressBarExample1ShowErrorSubstitution.Value = progressBarExample1ErrorRB.IsChecked.ToString();
            }
        }

        private void ProgressValue_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
        {
            progressBarExample2.Value = sender.Value;
        }
    }
}
