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
using Frame = System.Windows.Controls.Frame;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для NavigationViewPage.xaml
    /// </summary>
    public partial class NavigationViewPage : Page
    {
        public ObservableCollection<CategoryBase> Categories { get; set; }

        public NavigationViewPage()
        {
            InitializeComponent();

            DataContext = this;

            Categories = new ObservableCollection<CategoryBase>()
            {
                new Category() { Name = "Category 1", Glyph = Symbol.Home, Tooltip = "This is category 1" },
                new Category() { Name = "Category 2", Glyph = Symbol.Keyboard, Tooltip = "This is category 2" },
                new Category() { Name = "Category 3", Glyph = Symbol.Library, Tooltip = "This is category 3" },
                new Category() { Name = "Category 4", Glyph = Symbol.Mail, Tooltip = "This is category 4" }
            };

            navigationViewExample1.SelectedItem = navigationViewExample1.MenuItems.OfType<NavigationViewItem>().First();
            navigationViewExample2.SelectedItem = navigationViewExample2.MenuItems.OfType<NavigationViewItem>().First();
            navigationViewExample3.SelectedItem = Categories.First();
            navigationViewExample4.SelectedItem = navigationViewExample4.MenuItems.OfType<NavigationViewItem>().First();
        }

        private void SampleNavigate(NavigationView nv, NavigationViewSelectionChangedEventArgs args, Frame contentFrame)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(new Uri("/SamplePage/SampleSettingsPage.xaml", UriKind.Relative));
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                string selectedItemTag = ((string)selectedItem.Tag);
                nv.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                Uri uri = new Uri($"/SamplePage/{selectedItemTag}.xaml", UriKind.Relative);
                contentFrame.Navigate(uri);
            }
        }

        private void navigationViewExample1_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SampleNavigate(sender, args, contentFrame1);
        }

        private void navigationViewExample2_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SampleNavigate(sender, args, contentFrame2);
        }

        private void navigationViewExample3_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame3.Navigate(new Uri($"/SamplePage/SampleSettingsPage.xaml", UriKind.Relative));
            }
            else
            {
                var selectedItem = (Category)args.SelectedItem;
                string selectedItemTag = ((string)selectedItem.Name);
                sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                Uri uri = new Uri($"/SamplePage/SamplePage1.xaml", UriKind.Relative);
                contentFrame3.Navigate(uri);
            }
        }

        private void navigationViewExample4_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SampleNavigate(sender, args, contentFrame1);
        }
    }

    public class CategoryBase
    {

    }

    public class Category : CategoryBase
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public Symbol Glyph { get; set; }
    }

    public class Header : CategoryBase
    {
        public string Name { get; set; }
    }

    
    class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate HeaderTemplate { get; set; }

        public DataTemplate SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
        }
    }
}
