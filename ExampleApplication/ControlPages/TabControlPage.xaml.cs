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
using ExampleApplication.SamplePage;
using Frame = System.Windows.Controls.Frame;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для TabControlPage.xaml
    /// </summary>
    public partial class TabControlPage : Page
    {
        public TabControlPage()
        {
            InitializeComponent();
        }

        private void tabControlExample1TabPlacement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(example1TabPlacementSubstitution != null)
            {
                example1TabPlacementSubstitution.Value = ((Dock)tabControlExample1TabPlacement.SelectedItem).ToString();
            }
        }

        private void tabControlExample2_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                tabControlExample2.Items.Add(CreateNewTab(i));
            }
        }

        private void tabControlExample2_AddTabButtonClick(TabControl sender, RoutedEventArgs args)
        {
            sender.Items.Add(CreateNewTab(sender.Items.Count));
        }

        private void tabControlExample2_TabCloseRequested(TabControl sender, TabItemTabCloseRequestedEventArgs args)
        {
            sender.Items.Remove(args.Tab);
        }

        private TabItem CreateNewTab(int index)
        {
            TabItem newItem = new TabItem
            {
                Header = $"Document {index}",
            };
            newItem.SetValue(TabItemHelper.IconProperty, new SymbolIcon() { Symbol = Symbol.Document });

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            Frame frame = new Frame();

            switch (index % 3)
            {
                case 0:
                    frame.Navigate(new SamplePage1());
                    break;
                case 1:
                    frame.Navigate(new SamplePage2());
                    break;
                case 2:
                    frame.Navigate(new SamplePage3());
                    break;
            }

            newItem.Content = frame;

            return newItem;
        }
    }
}
