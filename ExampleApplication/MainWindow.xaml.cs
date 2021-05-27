using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using EasyWPFUI.Controls;
using EasyWPFUI.Extensions;
using ExampleApplication.Commands;
using ExampleApplication.Controls;

namespace ExampleApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<Data.NavigationMenuItem> navigationItems;
        private bool isBackNavigation;

        #region Properties

        public List<NavigationViewItemBase> NavigationMenuItems { get; private set; }

        public static MainWindow Current { get; private set; }

        public NavigationViewHeader PageHeader
        {
            get
            {
                return navigationView.FindDescendant(typeof(NavigationViewHeader)) as NavigationViewHeader;
            }
        }

        #endregion

        #region Commands

        public RelayCommand<ApplicationTheme> ChangeApplicationThemeCommand { get; private set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Current = this;

            InitializeCommands();

            InitializeNavigationMenu();

            navigationView.SelectedItem = NavigationMenuItems.First();

            SetTitleBar(TitleBar);

            GetTitleBar().LayoutMetricsChanged += OnMainWindowTitleBarLayoutMetricsChanged;
        }

        private void InitializeCommands()
        {
            ChangeApplicationThemeCommand = new RelayCommand<ApplicationTheme>(theme =>
            {
                ThemeManager.ApplicationTheme = theme;
            });
        }

        private void InitializeNavigationMenu()
        {
            navigationItems = Data.NavigationMenuData.GetNavigationMenu();

            NavigationMenuItems = GetMenuItems(navigationItems);

            OnPropertyChanged("NavigationMenuItems");
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnMainWindowTitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Thickness currMargin = TitleBar.Margin;

            TitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, sender.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Data.NavigationMenuItem item = args.SelectedItemContainer?.DataContext as Data.NavigationMenuItem;

            if (args.IsSettingsSelected)
            {
                navigationFrame.Navigate(new SamplePage.SampleSettingsPage());
                navigationView.Header = args.SelectedItemContainer.Content.ToString();
            }
            else
            {
                if (item != null && !isBackNavigation)
                {
                    if (!item.IsGroupItem)
                    {
                        navigationFrame.Navigate(item.PageUri);
                        navigationView.Header = args.SelectedItemContainer.Content.ToString();
                    }
                }
            }
        }

        private void OnNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(navigationFrame.CanGoBack)
            {
                isBackNavigation = true;
                navigationFrame.GoBack();
            }
        }

        private void OnNavigationViewDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currentMargin = TitleBar.Margin;

            if(sender.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                TitleBar.Margin = new Thickness(sender.CompactPaneLength * 2, currentMargin.Top, currentMargin.Right, currentMargin.Bottom);
            }
            else
            {
                TitleBar.Margin = new Thickness(sender.CompactPaneLength, currentMargin.Top, currentMargin.Right, currentMargin.Bottom);
            }

            UpdateTitleBarMargin();
            UpdateNavigationViewHeaderPadding();
        }

        private void OnNavigationViewPaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            UpdateTitleBarMargin();
        }

        private void OnNavigationViewPaneOpening(NavigationView sender, object args)
        {
            UpdateTitleBarMargin();
        }

        private void OnNavigationViewLoaded(object sender, RoutedEventArgs e)
        {
            if(navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Auto)
            {
                UpdateNavigationViewHeaderPadding();
            }
        }

        private void OnNavigationViewSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
         {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<Data.NavigationMenuItem> suggestions = new List<Data.NavigationMenuItem>();

                string[] querySplit = sender.Text.Split(' ');

                foreach(Data.NavigationMenuItem group in navigationItems)
                {
                    if(!group.IsGroupItem && group.PageUri != null)
                    {
                        suggestions.Add(group);
                    }

                    if (group.IsGroupItem)
                    {
                        var matchingItems = group.SubmenuItems.Where(item =>
                        {
                            bool flag = true;
                            foreach (string queryToken in querySplit)
                            {
                                // Check if token is not in string
                                if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                                {
                                    // Token is not in string, so we ignore this item.
                                    flag = false;
                                }
                            }
                            return flag;
                        });

                        foreach (var item in matchingItems)
                        {
                            suggestions.Add(item);
                        }
                    }
                }

                if (suggestions.Count > 0)
                {
                    navigationViewSearchBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title);
                }
                else
                {
                    navigationViewSearchBox.ItemsSource = new string[] { "No results found" };
                }
            }
        }

        private void OnNavigationViewSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(args.ChosenSuggestion != null && args.ChosenSuggestion is Data.NavigationMenuItem selectedItem)
            {
                NavigationViewItemBase item = GetMenuItemFromUri(selectedItem.PageUri, NavigationMenuItems);

                if(item != null)
                {
                    navigationView.SelectedItem = item;
                }
            }
        }

        private void OnNavigationFrameNavigated(object sender, NavigationEventArgs e)
        {
            CloseTeachingTips();

            if (isBackNavigation)
            {
                NavigationViewItemBase item = GetMenuItemFromUri(e.Uri, NavigationMenuItems);

                if(item != null)
                    navigationView.SelectedItem = item;

                isBackNavigation = false;
            }
        }

        private void CloseTeachingTips()
        {
            if (Current?.PageHeader != null)
            {
                Current.PageHeader.ToggleThemeTeachingTip1.IsOpen = false;
                Current.PageHeader.ToggleThemeTeachingTip3.IsOpen = false;
            }
        }

        private List<NavigationViewItemBase> GetMenuItems(List<Data.NavigationMenuItem> menuItems)
        {
            List<NavigationViewItemBase> navMenu = new List<NavigationViewItemBase>();

            foreach (Data.NavigationMenuItem menuItem in menuItems)
            {
                NavigationViewItem item = new NavigationViewItem()
                {
                    Content = menuItem.Title,
                    Icon = menuItem.Icon,
                    DataContext = menuItem
                };

                if (menuItem.SubmenuItems != null)
                {
                    item.MenuItems = GetMenuItems(menuItem.SubmenuItems);
                    item.IsExpanded = menuItem.IsExpanded;
                }

                navMenu.Add(item);

                if (menuItem.NextItemIsSeparator)
                {
                    navMenu.Add(new NavigationViewItemSeparator());
                }
            }

            return navMenu;
        }

        private NavigationViewItemBase GetMenuItemFromUri(Uri source, IList items)
        {
            foreach (NavigationViewItemBase itemBase in items)
            {
                if (itemBase.DataContext is Data.NavigationMenuItem menuItem)
                {
                    if (menuItem.PageUri == source)
                    {
                        return itemBase;
                    }
                    else if (menuItem.IsGroupItem && itemBase is NavigationViewItem item)
                    {
                        if(GetMenuItemFromUri(source, (IList)item.MenuItems) is NavigationViewItemBase itm)
                        {
                            return itm;
                        }
                    }
                }
            }

            return null;
        }

        private void UpdateTitleBarMargin()
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if ((navigationView.DisplayMode == NavigationViewDisplayMode.Expanded && navigationView.IsPaneOpen) || navigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                TitleBar.RenderTransform = new TranslateTransform() { X = smallLeftIndent };
            }
            else
            {
                TitleBar.RenderTransform = new TranslateTransform() { X = largeLeftIndent };
            }
        }

        private void UpdateNavigationViewHeaderPadding()
        {
            if(navigationView.FindDescendant(typeof(NavigationViewHeader)) is NavigationViewHeader header)
            {
                if(navigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    header.Padding = new Thickness(0, 28, 12, 0);
                    Thickness currMargin = header.Margin;
                    header.Margin = new Thickness(-4 + currMargin.Left, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    header.Padding = new Thickness(12, 28, 12, 0);

                    Thickness currMargin = header.Margin;
                    if (currMargin.Left == -4 || currMargin.Left == 0)
                        currMargin.Left = 0;
                    else
                        currMargin.Left -= -4;
                    header.Margin = new Thickness(currMargin.Left, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void OnNavigationViewHeaderChangeThemeRequested(object sender, EventArgs e)
        {
            if (ThemeManager.ApplicationTheme == ApplicationTheme.Dark)
            {
                ThemeManager.ApplicationTheme = ApplicationTheme.Light;
            }
            else
            {
                ThemeManager.ApplicationTheme = ApplicationTheme.Dark;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnNavigationViewHeaderChangeThemeRequested(this, null);
        }
    }
}
