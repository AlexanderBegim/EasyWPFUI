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

namespace ExampleApplication.Controls
{
    /// <summary>
    /// Логика взаимодействия для NavigationViewHeader.xaml
    /// </summary>
    public partial class NavigationViewHeader : UserControl
    {
        public event EventHandler ChangeThemeRequested;

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(NavigationViewHeader), new FrameworkPropertyMetadata());

        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }

        public NavigationViewHeader()
        {
            InitializeComponent();
        }

        private void OnChangeThemeButtonClick(object sender, RoutedEventArgs e)
        {
            ChangeThemeRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ToggleThemeTeachingTip2_ActionButtonClick(EasyWPFUI.Controls.TeachingTip sender, object args)
        {
            ChangeThemeRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
