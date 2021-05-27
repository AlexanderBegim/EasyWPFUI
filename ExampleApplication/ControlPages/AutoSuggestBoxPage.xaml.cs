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
    /// Логика взаимодействия для AutoSuggestBoxPage.xaml
    /// </summary>
    public partial class AutoSuggestBoxPage : Page
    {
        private List<string> Cats = new List<string>()
        {
            "Abyssinian",
            "Aegean",
            "American Bobtail",
            "American Curl",
            "American Ringtail",
            "American Shorthair",
            "American Wirehair",
            "Aphrodite Giant",
            "Arabian Mau",
            "Asian cat",
            "Asian Semi-longhair",
            "Australian Mist",
            "Balinese",
            "Bambino",
            "Bengal",
            "Birman",
            "Bombay",
            "Brazilian Shorthair",
            "British Longhair",
            "British Shorthair",
            "Burmese",
            "Burmilla",
            "California Spangled",
            "Chantilly-Tiffany",
            "Chartreux",
            "Chausie",
            "Colorpoint Shorthair",
            "Cornish Rex",
            "Cymric",
            "Cyprus",
            "Devon Rex",
            "Donskoy",
            "Dragon Li",
            "Dwelf",
            "Egyptian Mau",
            "European Shorthair",
            "Exotic Shorthair",
            "Foldex",
            "German Rex",
            "Havana Brown",
            "Highlander",
            "Himalayan",
            "Japanese Bobtail",
            "Javanese",
            "Kanaani",
            "Khao Manee",
            "Kinkalow",
            "Korat",
            "Korean Bobtail",
            "Korn Ja",
            "Kurilian Bobtail",
            "Lambkin",
            "LaPerm",
            "Lykoi",
            "Maine Coon",
            "Manx",
            "Mekong Bobtail",
            "Minskin",
            "Napoleon",
            "Munchkin",
            "Nebelung",
            "Norwegian Forest Cat",
            "Ocicat",
            "Ojos Azules",
            "Oregon Rex",
            "Oriental Bicolor",
            "Oriental Longhair",
            "Oriental Shorthair",
            "Persian (modern)",
            "Persian (traditional)",
            "Peterbald",
            "Pixie-bob",
            "Ragamuffin",
            "Ragdoll",
            "Raas",
            "Russian Blue",
            "Russian White",
            "Sam Sawet",
            "Savannah",
            "Scottish Fold",
            "Selkirk Rex",
            "Serengeti",
            "Serrade Petit",
            "Siamese",
            "Siberian or´Siberian Forest Cat",
            "Singapura",
            "Snowshoe",
            "Sokoke",
            "Somali",
            "Sphynx",
            "Suphalak",
            "Thai",
            "Thai Lilac",
            "Tonkinese",
            "Toyger",
            "Turkish Angora",
            "Turkish Van",
            "Turkish Vankedisi",
            "Ukrainian Levkoy",
            "Wila Krungthep",
            "York Chocolate"
        };

        public AutoSuggestBoxPage()
        {
            InitializeComponent();
        }

        private void autoSuggestBoxExample1_TextChanged(EasyWPFUI.Controls.AutoSuggestBox sender, EasyWPFUI.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            var suitableItems = new List<string>();
            var splitText = sender.Text.ToLower().Split(' ');
            foreach (var cat in Cats)
            {
                var found = splitText.All((key) =>
                {
                    return cat.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(cat);
                }
            }
            if (suitableItems.Count == 0)
            {
                suitableItems.Add("No results found");
            }
            sender.ItemsSource = suitableItems;
        }

        private void autoSuggestBoxExample1_SuggestionChosen(EasyWPFUI.Controls.AutoSuggestBox sender, EasyWPFUI.Controls.AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SuggestionOutput.Text = args.SelectedItem.ToString();
        }
    }
}
