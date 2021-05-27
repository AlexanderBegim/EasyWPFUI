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
using EasyWPFUI;
using EasyWPFUI.Media;
using ExampleApplication.Commands;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для AccentColorPage.xaml
    /// </summary>
    public partial class AccentColorPage : Page
    {
        public List<Color> Colors { get; } = new List<Color>()
        {
            AccentColors.YellowGold,
            AccentColors.Gold,
            AccentColors.OrangeBright,
            AccentColors.OrangeDark,
            AccentColors.Rust,
            AccentColors.PaleRust,
            AccentColors.BrickRed,
            AccentColors.ModRed,

            AccentColors.PaleRed,
            AccentColors.Red,
            AccentColors.RoseBright,
            AccentColors.Rose,
            AccentColors.PlumLight,
            AccentColors.Plum,
            AccentColors.OrchidLight,
            AccentColors.Orchid,

            AccentColors.DefaultBlue,
            AccentColors.NavyBlue,
            AccentColors.PurpleShadow,
            AccentColors.PurpleShadowDark,
            AccentColors.IrisPastel,
            AccentColors.IrisSpring,
            AccentColors.VioletRedLight,
            AccentColors.VioletRed,

            AccentColors.CoolBlueBright,
            AccentColors.CoolBlue,
            AccentColors.Seafoam,
            AccentColors.SeafoamTeal,
            AccentColors.MintLight,
            AccentColors.MintDark,
            AccentColors.TurfGreen,
            AccentColors.SportGreen,

            AccentColors.Gray,
            AccentColors.GrayBrown,
            AccentColors.SteelBlue,
            AccentColors.MetalBlue,
            AccentColors.PaleMoss,
            AccentColors.Moss,
            AccentColors.MeadowGreen,
            AccentColors.Green,

            AccentColors.Overcast,
            AccentColors.Storm,
            AccentColors.BlueGray,
            AccentColors.GrayDark,
            AccentColors.LiddyGreen,
            AccentColors.Sage,
            AccentColors.CamouflageDesert,
            AccentColors.Camouflage,
        };

        private Color _selectedAccentColor = ThemeManager.AccentColor;
        public Color SelectedAccentColor
        {
            get
            {
                return _selectedAccentColor;
            }
            set
            {
                _selectedAccentColor = value;
                ChangeAccentColor();
            }
        }

        public AccentColorPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void ChangeAccentColor()
        {
            ThemeManager.AccentColor = SelectedAccentColor;
        }
    }
}
