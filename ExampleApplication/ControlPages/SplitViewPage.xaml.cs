using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для SplitViewPage.xaml
    /// </summary>
    public partial class SplitViewPage : Page
    {
        public ObservableCollection<NavLink> NavLinks { get; } = new ObservableCollection<NavLink>()
        {
            new NavLink() { Label = "People", Symbol = Symbol.People  },
            new NavLink() { Label = "Globe", Symbol = Symbol.Globe },
            new NavLink() { Label = "Message", Symbol = Symbol.Message },
            new NavLink() { Label = "Mail", Symbol = Symbol.Mail },
        };

        public SplitViewPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                splitViewExample1.PanePlacement = SplitViewPanePlacement.Right;
            }
            else
            {
                splitViewExample1.PanePlacement = SplitViewPanePlacement.Left;
            }
        }

        private void displayModeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            splitViewExample1.DisplayMode = (SplitViewDisplayMode)displayModeCombobox.SelectedItem;
            
            if(DisplayModeSubstitution != null)
            {
                DisplayModeSubstitution.Value = splitViewExample1.DisplayMode.ToString();
            }
        }

        private void paneBackgroundCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(paneBackgroundCombobox.SelectedItem is ComboBoxItem item && item.Tag is Color color)
            {
                splitViewExample1.PaneBackground = new SolidColorBrush(color);

                if(PaneBackgroundSubstitution != null)
                {
                    PaneBackgroundSubstitution.Value = item.Content.ToString();
                }
            }
            else
            {
                splitViewExample1.SetResourceReference(SplitView.PaneBackgroundProperty, "SystemControlBackgroundChromeMediumLowBrush");

                if (PaneBackgroundSubstitution != null)
                {
                    PaneBackgroundSubstitution.Value = "{DynamicResource SystemControlBackgroundChromeMediumLowBrush}";
                }
            }
        }

        private void openPaneLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(OpenPaneLengthSubstitution != null)
            {
                OpenPaneLengthSubstitution.Value = openPaneLengthSlider.Value.ToString();
            }
        }

        private void compactPaneLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(CompactPaneLengthSubstitution != null)
            {
                CompactPaneLengthSubstitution.Value = compactPaneLengthSlider.Value.ToString();
            }
        }

        private void NavLinksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            content.Text = (e.AddedItems[0] as NavLink).Label + " Page";
        }
    }

    public class NavLink
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
