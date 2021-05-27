using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.ComponentModel;
using EasyWPFUI.Controls;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для ItemsRepeaterPage.xaml
    /// </summary>
    public partial class ItemsRepeaterPage : Page
    {
        private Random random = new Random();
        private int MaxLength = 425;

        public ObservableCollection<Bar> BarItems { get; private set; }
        public MyItemsSource filteredRecipeData = new MyItemsSource(null);
        public List<Recipe> staticRecipeData;
        private bool IsSortDescending = false;

        private double AnimatedBtnHeight;
        private Thickness AnimatedBtnMargin;

        private StackLayout VerticalStackLayout;
        private StackLayout HorizontalStackLayout;
        private UniformGridLayout UniformGridLayout;

        public ItemsRepeaterPage()
        {
            InitializeComponent();

            DataContext = this;

            VerticalStackLayout = (StackLayout)Resources["VerticalStackLayout"];
            HorizontalStackLayout = (StackLayout)Resources["HorizontalStackLayout"];
            UniformGridLayout = (UniformGridLayout)Resources["UniformGridLayout"];

            InitializeData();

            repeaterExample2.ItemsSource = Enumerable.Range(0, 500);
        }

        private void InitializeData()
        {
            if (BarItems == null)
            {
                BarItems = new ObservableCollection<Bar>();
            }
            BarItems.Add(new Bar(300, this.MaxLength));
            BarItems.Add(new Bar(25, this.MaxLength));
            BarItems.Add(new Bar(175, this.MaxLength));

            List<object> basicData = new List<object>
            {
                64,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                128,
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                256,
                "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                512,
                "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                1024
            };

            // MixedTypeRepeater.ItemsSource = basicData;

            List<NestedCategory> nestedCategories = new List<NestedCategory>
            {
                new NestedCategory("Fruits", GetFruits()),
                new NestedCategory("Vegetables", GetVegetables()),
                new NestedCategory("Grains", GetGrains()),
                new NestedCategory("Proteins", GetProteins())
            };

            // outerRepeater.ItemsSource = nestedCategories;

            // Set sample code to display on page's initial load
            SampleCodeLayout.Value = @"<ui:StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";

            SampleCodeDT.Value = @"<DataTemplate x:Key=""HorizontalBarTemplate"">
    <Border Background=""{DynamicResource SystemControlPageBackgroundChromeLowBrush}"" Width=""{Binding MaxLength}"" >
        <Rectangle Fill=""{DynamicResource SystemAccentColorBrush}"" Width=""{Binding Length}"" 
                   Height=""24"" HorizontalAlignment=""Left""/> 
    </Border>
</DataTemplate>";

            SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";
        }

        private ObservableCollection<string> GetFruits()
        {
            return new ObservableCollection<string> { "Apricots", "Bananas", "Grapes", "Strawberries", "Watermelon", "Plums", "Blueberries" };
        }

        private ObservableCollection<string> GetVegetables()
        {
            return new ObservableCollection<string> { "Broccoli", "Spinach", "Sweet potato", "Cauliflower", "Onion", "Brussels sprouts", "Carrots" };
        }
        private ObservableCollection<string> GetGrains()
        {
            return new ObservableCollection<string> { "Rice", "Quinoa", "Pasta", "Bread", "Farro", "Oats", "Barley" };
        }
        private ObservableCollection<string> GetProteins()
        {
            return new ObservableCollection<string> { "Steak", "Chicken", "Tofu", "Salmon", "Pork", "Chickpeas", "Eggs" };
        }

        // ==========================================================================
        // Basic, non-interactive ItemsRepeater
        // ==========================================================================
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            BarItems.Add(new Bar(random.Next(this.MaxLength), this.MaxLength));
            DeleteBtn.IsEnabled = true;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BarItems.Count > 0)
            {
                BarItems.RemoveAt(0);
                if (BarItems.Count == 0)
                {
                    DeleteBtn.IsEnabled = false;
                }
            }
        }

        private void RadioBtn_Click(object sender, RoutedEventArgs e)
        {
            string itemTemplateKey = string.Empty;
            var layoutKey = ((FrameworkElement)sender).Tag as string;

            if (layoutKey.Equals(nameof(this.VerticalStackLayout))) // we used x:Name in the resources which both acts as the x:Key value and creates a member field by the same name
            {
                layout.Value = layoutKey;
                itemTemplateKey = "HorizontalBarTemplate";

                repeaterExample1.MaxWidth = MaxLength + 12;

                SampleCodeLayout.Value = @"<ui:StackLayout x:Name=""VerticalStackLayout"" Orientation=""Vertical"" Spacing=""8""/>";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""HorizontalBarTemplate"">
    <Border Background=""{DynamicResource SystemControlPageBackgroundChromeLowBrush}"" Width=""{Binding MaxLength}"" >
        <Rectangle Fill=""{DynamicResource SystemAccentColorBrush}"" Width=""{Binding Length}""
                   Height=""24"" HorizontalAlignment=""Left""/> 
    </Border>
</DataTemplate>";
            }
            else if (layoutKey.Equals(nameof(this.HorizontalStackLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "VerticalBarTemplate";

                repeaterExample1.MaxWidth = 6000;

                SampleCodeLayout.Value = @"<ui:StackLayout x:Name=""HorizontalStackLayout"" Orientation=""Horizontal"" Spacing=""8""/> ";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""VerticalBarTemplate"">
    <Border Background=""{DynamicResource SystemControlPageBackgroundChromeLowBrush}"" Height=""{Binding MaxHeight}"">
        <Rectangle Fill=""{DynamicResource SystemAccentColorBrush}"" Height=""{Binding Height}"" 
                   Width=""48"" VerticalAlignment=""Top""/>
    </Border>
</DataTemplate>";
            }
            else if (layoutKey.Equals(nameof(this.UniformGridLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "CircularTemplate";

                repeaterExample1.MaxWidth = 540;

                SampleCodeLayout.Value = @"<ui:UniformGridLayout x:Name=""UniformGridLayout"" MinRowSpacing=""8"" MinColumnSpacing=""8""/>";
                SampleCodeDT.Value = @"<DataTemplate x:Key=""CircularTemplate"">
    <Grid>
        <Ellipse Fill=""{DynamicResource SystemControlPageBackgroundChromeLowBrush}"" Height=""{Binding MaxDiameter}"" 
                 Width=""{Binding MaxDiameter}"" VerticalAlignment=""Center"" HorizontalAlignment=""Center""/>
        <Ellipse Fill=""{DynamicResource SystemAccentColorBrush}"" Height=""{Binding Diameter}"" 
                 Width=""{Binding Diameter}"" VerticalAlignment=""Center"" HorizontalAlignment=""Center""/>
    </Grid>
</DataTemplate>";
            }

            repeaterExample1.Layout = Resources[layoutKey] as VirtualizingLayout;
            repeaterExample1.ItemTemplate = Resources[itemTemplateKey] as DataTemplate;
            repeaterExample1.ItemsSource = BarItems;

            elementGenerator.Value = itemTemplateKey;
        }


        // ==========================================================================
        // Virtualizing, scrollable list of items laid out by ItemsRepeater
        // ==========================================================================
        private void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = ((FrameworkElement)sender).Tag as string;

            repeaterExample2.Layout = Resources[layoutKey] as VirtualizingLayout;

            layout2.Value = layoutKey;

            if (layoutKey == "UniformGridLayout2")
            {
                SampleCodeLayout2.Value = @"<muxc:UniformGridLayout x:Key=""UniformGridLayout2"" MinItemWidth=""108"" MinItemHeight=""108""
                   MinRowSpacing=""12"" MinColumnSpacing=""12""/>";
            }
            else if (layoutKey == "MyFeedLayout")
            {
                SampleCodeLayout2.Value = @"<common:ActivityFeedLayout x:Key=""MyFeedLayout"" ColumnSpacing=""12""
                          RowSpacing=""12"" MinItemSize=""80, 108""/>";
            }
        }
    }

    public class NestedCategory
    {
        public string CategoryName { get; set; }
        public ObservableCollection<string> CategoryItems { get; set; }
        public NestedCategory(string catName, ObservableCollection<string> catItems)
        {
            CategoryName = catName;
            CategoryItems = catItems;
        }
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Accent { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if ((int)item % 2 == 0)
            {
                return Normal;
            }
            else
            {
                return Accent;
            }
        }
    }

    public class StringOrIntTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(string))
            {
                return StringTemplate;
            }
            else if (item.GetType() == typeof(int))
            {
                return IntTemplate;
            }
            else
            {
                return null;
            }
        }
    }

    public class Bar
    {
        public Bar(double length, int max)
        {
            Length = length;
            MaxLength = max;

            Height = length / 4;
            MaxHeight = max / 4;

            Diameter = length / 6;
            MaxDiameter = max / 6;
        }
        public double Length { get; set; }
        public int MaxLength { get; set; }

        public double Height { get; set; }
        public double MaxHeight { get; set; }

        public double Diameter { get; set; }
        public double MaxDiameter { get; set; }
    }

    public class Recipe
    {
        public int Num { get; set; }
        public string Ingredients { get; set; }
        public List<string> IngList { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int NumIngredients
        {
            get
            {
                return IngList.Count();
            }
        }

        public void RandomizeIngredients()
        {
            // To give the items different heights, give recipes random numbers of random ingredients
            Random rndNum = new Random();
            Random rndIng = new Random();

            ObservableCollection<string> extras = new ObservableCollection<string>{
                                                         "Garlic",
                                                         "Lemon",
                                                         "Butter",
                                                         "Lime",
                                                         "Feta Cheese",
                                                         "Parmesan Cheese",
                                                         "Breadcrumbs"};
            for (int i = 0; i < rndNum.Next(0, 4); i++)
            {
                string newIng = extras[rndIng.Next(0, 6)];
                if (!IngList.Contains(newIng))
                {
                    Ingredients += "\n" + newIng;
                    IngList.Add(newIng);
                }
            }

        }
    }

    // Custom data source class that assigns elements unique IDs, making filtering easier
    public class MyItemsSource : IList, IKeyIndexMapping, INotifyCollectionChanged
    {
        private List<Recipe> inner = new List<Recipe>();

        public MyItemsSource(IEnumerable<Recipe> collection)
        {
            InitializeCollection(collection);
        }

        public void InitializeCollection(IEnumerable<Recipe> collection)
        {
            inner.Clear();
            if (collection != null)
            {
                inner.AddRange(collection);
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region IReadOnlyList<T>
        public int Count => this.inner != null ? this.inner.Count : 0;

        public object this[int index]
        {
            get
            {
                return inner[index] as Recipe;
            }

            set
            {
                inner[index] = (Recipe)value;
            }
        }

        public IEnumerator<Recipe> GetEnumerator() => this.inner.GetEnumerator();

        #endregion

        #region INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IKeyIndexMapping
        public string KeyFromIndex(int index)
        {
            return inner[index].Num.ToString();
        }

        public int IndexFromKey(string key)
        {
            foreach (Recipe item in inner)
            {
                if (item.Num.ToString() == key)
                {
                    return inner.IndexOf(item);
                }
            }
            return -1;
        }

        #endregion

        #region Unused List methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        #endregion
    }
}
