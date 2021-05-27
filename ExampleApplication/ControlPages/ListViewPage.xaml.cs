using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.Globalization;
using System.ComponentModel;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для ListViewPage.xaml
    /// </summary>
    public partial class ListViewPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Contact> Contacts { get; private set; }

        public ListViewPage()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += ListViewPage_Loaded;
        }

        private async void ListViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Contacts = await Contact.GetContactsAsync();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Contacts)));
        }

        /* *** */

        private void listViewExample2ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewExample2 != null)
            {
                string selectionMode = e.AddedItems[0].ToString();
                switch (selectionMode)
                {
                    case "Single":
                        listViewExample2.SelectionMode = SelectionMode.Single;
                        break;
                    case "Multiple":
                        listViewExample2.SelectionMode = SelectionMode.Multiple;
                        break;
                    case "Extended":
                        listViewExample2.SelectionMode = SelectionMode.Extended;
                        break;
                }

                if (listViewExample2Substitution != null)
                {
                    listViewExample2Substitution.Value = listViewExample2.SelectionMode.ToString();
                }
            }
        }
    }

    public class Contact
    {
        #region Properties

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Company { get; private set; }
        public string Name => FirstName + " " + LastName;

        public string Group { get; private set; }

        #endregion

        public Contact(string firstName, string lastName, string company)
        {
            FirstName = firstName;
            LastName = lastName;
            Company = company;
        }

        #region Public Methods

        public static async Task<ObservableCollection<Contact>> GetContactsAsync()
        {
            Uri sampleUri = new Uri("pack://application:,,,/ExampleApplication;component/Data/Contacts.txt");

            StreamResourceInfo resource = Application.GetResourceStream(sampleUri);

            List<string> lines = new List<string>();

            using(StreamReader reader = new StreamReader(resource.Stream))
            {
                while(reader.Peek() >= 0)
                {
                    lines.Add(await reader.ReadLineAsync());
                }
            }

            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < lines.Count; i += 3)
            {
                contacts.Add(new Contact(lines[i], lines[i + 1], lines[i + 2]) { Group = lines[i + 1].Substring(0,1) });
            }

            return contacts;
        }

        public static async Task<ObservableCollection<GroupInfoList>> GetContactsGroupedAsync()
        {
            var query = from item in await GetContactsAsync()
                        group item by item.LastName.Substring(0, 1).ToUpper() into g
                        orderby g.Key
                        select new GroupInfoList(g) { Key = g.Key };

            return new ObservableCollection<GroupInfoList>(query);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }
        public object Key { get; set; }
    }

    public class ContactGroupKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Substring(0, 1).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
