using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EasyWPFUI.Controls;

namespace ExampleApplication.Data
{
    public sealed class NavigationMenuItem
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string BadgeString { get; set; }

        public IconElement Icon { get; set; }

        public ImageSource ImagePath { get; set; }

        public Uri PageUri { get; set; }

        public List<NavigationMenuItem> SubmenuItems { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsGroupItem
        {
            get
            {
                return SubmenuItems != null;
            }
        }

        public bool NextItemIsSeparator { get; set; }


        public NavigationMenuItem(string title, IconElement icon, Uri pageUri)
        {
            Title = title;

            Icon = icon;

            PageUri = pageUri;
        }

        public NavigationMenuItem(string title, IconElement icon, Uri pageUri, List<NavigationMenuItem> submenuItems, bool isExpanded = false) : this(title, icon, pageUri)
        {
            SubmenuItems = submenuItems;

            IsExpanded = isExpanded;
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public static class NavigationMenuData
    {
        public static List<NavigationMenuItem> GetNavigationMenu()
        {
            List<NavigationMenuItem> basicItems = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Buttons", null, GetPageUri("ButtonPage"))
                {
                    Subtitle = "A control that responds to user input and raises a Click event."
                },
                new NavigationMenuItem("CheckBox", null, GetPageUri("CheckBoxPage"))
                {
                    Subtitle = "A control that a user can select or clear."
                },
                new NavigationMenuItem("ComboBox", null, GetPageUri("ComboBoxPage"))
                {
                    Subtitle = "A drop-down list of items a user can select from."
                },
                new NavigationMenuItem("RadioButton", null, GetPageUri("RadioButtonPage"))
                {
                    Subtitle = "A control that allows a user to select a single option from a group of options."
                },
                new NavigationMenuItem("Slider", null, GetPageUri("SliderPage"))
                {
                    Subtitle = "A control that lets the user select from a range of values by moving a Thumb control along a track."
                },
            };

            List<NavigationMenuItem> collectionsItems = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("DataGrid", null, GetPageUri("DataGridPage"))
                {
                    Subtitle = "The DataGrid control presents data in a customizable table of rows and columns."
                },
                new NavigationMenuItem("ListBox", null, GetPageUri("ListBoxPage"))
                {
                    Subtitle = "A control that presents an inline list of items that the user can select from."
                },
                new NavigationMenuItem("ListView", null, GetPageUri("ListViewPage"))
                {
                    Subtitle = "A control that presents a collection of items in a vertical list."
                },
                new NavigationMenuItem("TreeView", null, GetPageUri("TreeViewPage"))
                {
                    Subtitle = "The  TreeView control is a hierarchical list pattern with expanding and collapsing nodes that contain nested items."
                },
            };

            List<NavigationMenuItem> dateAndTime = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Calendar", null, GetPageUri("CalendarPage"))
                {
                    Subtitle = "A control that presents a calendar for a user to choose a date from."
                },
                new NavigationMenuItem("DatePicker", null, GetPageUri("DatePickerPage"))
                {
                    Subtitle = "A control that lets users pick a date value using a calendar."
                },
            };

            List<NavigationMenuItem> dialogsAndFlyout = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Flyout", null, GetPageUri("FlyoutPage"))
                {
                    Subtitle = "Shows contextual information and enables user interaction."
                },
                new NavigationMenuItem("TeachingTip", null, GetPageUri("TeachingTipPage"))
                {
                    Subtitle = "A content-rich flyout for guiding users and enabling teaching moments."
                },
            };

            List<NavigationMenuItem> layout = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("GroupBox", null, GetPageUri("GroupBoxPage"))
                {
                    Subtitle = ""
                },
                new NavigationMenuItem("GridSplitter", null, GetPageUri("GridSplitterPage"))
                {
                    Subtitle = ""
                },
                new NavigationMenuItem("ItemsRepeater", null, GetPageUri("ItemsRepeaterPage"))
                {
                    Subtitle = "A flexible, primitive control for data-driven layouts."
                },
                new NavigationMenuItem("SplitView", null, GetPageUri("SplitViewPage"))
                {
                    Subtitle = "A container that has 2 content areas, with multiple display options for the pane."
                },
            };

            List<NavigationMenuItem> menusAndToolbars = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Menu", null, GetPageUri("MenuPage"))
                {
                    Subtitle = "A classic menu, allowing the display of MenuItems containing Menu."
                },
                new NavigationMenuItem("ContextMenu", null, GetPageUri("ContextMenuPage"))
                {
                    Subtitle = ""
                },
            };

            List<NavigationMenuItem> navigation = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("NavigationView", null, GetPageUri("NavigationViewPage"))
                {
                    Subtitle = "Common vertical layout for top-level areas of your app via a collapsible navigation menu."
                },
                new NavigationMenuItem("TabControl", null, GetPageUri("TabControlPage"))
                {
                    Subtitle = ""
                },
                new NavigationMenuItem("TabControl (Pivot Style)", null, GetPageUri("TabControlPivotStylePage"))
                {
                    Subtitle = ""
                },
            };

            List<NavigationMenuItem> scrolling = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("ScrollViewer", null, GetPageUri("ScrollViewerPage"))
                {
                    Subtitle = "A container control that lets the user pan and zoom its content."
                },
            };

            List<NavigationMenuItem> statusAndInfo = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("InfoBar", null, GetPageUri("InfoBarPage")),
                new NavigationMenuItem("ProgressBar", null, GetPageUri("ProgressBarPage")),
                new NavigationMenuItem("ProgressRing", null, GetPageUri("ProgressRingPage")),
                new NavigationMenuItem("ToolTip", null, GetPageUri("ToolTipPage")),
            };

            List<NavigationMenuItem> styles = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Accent Colors", null, GetPageUri("AccentColorPage")),
                new NavigationMenuItem("Compact Sizing", null, GetPageUri("CompactSizingPage")),
            };

            List<NavigationMenuItem> text = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("AutoSuggestBox", null, GetPageUri("AutoSuggestBoxPage")),
                new NavigationMenuItem("NumberBox", null, GetPageUri("NumberBoxPage")),
                new NavigationMenuItem("PasswordBox", null, GetPageUri("PasswordBoxPage")),
                new NavigationMenuItem("RichTextBox", null, GetPageUri("RichTextBoxPage")),
                new NavigationMenuItem("TextBlock", null, GetPageUri("TextBlockPage")),
                new NavigationMenuItem("TextBox", null, GetPageUri("TextBoxPage")),
            };


            List<NavigationMenuItem> navigationMenu = new List<NavigationMenuItem>()
            {
                new NavigationMenuItem("Overview", GetIcon("\uE71D"), GetPageUri("OverviewPage")) { NextItemIsSeparator = true },
                new NavigationMenuItem("Basic Input", GetIcon("\uE73A"), GetPageUri(null), basicItems),
                new NavigationMenuItem("Collections", GetIcon("\uE80A"), GetPageUri(null), collectionsItems),
                new NavigationMenuItem("Date And Time", GetIcon("\uEC92"), GetPageUri(null), dateAndTime),
                new NavigationMenuItem("Dialogs And Flyouts", GetIcon("\uE8BD"), GetPageUri(null), dialogsAndFlyout),
                new NavigationMenuItem("Layout", GetIcon("\uE8A1"), GetPageUri(null), layout),
                new NavigationMenuItem("Menus and Toolbars", GetIcon("\uE74E"), GetPageUri(null), menusAndToolbars),
                new NavigationMenuItem("Navigation", GetIcon("\uE700"), GetPageUri(null), navigation),
                new NavigationMenuItem("Scrolling", GetIcon("\uE8CB"), GetPageUri(null), scrolling),
                new NavigationMenuItem("Status and Info", GetIcon("\uE8F2"), GetPageUri(null), statusAndInfo),
                new NavigationMenuItem("Styles", GetIcon("\uE790"), GetPageUri(null), styles),
                new NavigationMenuItem("Text", GetIcon("\uE8D2"), GetPageUri(null), text),
            };

            return navigationMenu;
        }

        private static IconElement GetIcon(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath))
            {
                return null;
            }

            if (iconPath.IndexOf('.') > 0)
            {
                return new BitmapIcon()
                {
                    UriSource = new Uri(iconPath, UriKind.RelativeOrAbsolute)
                };
            }

            return new FontIcon()
            {
                Glyph = iconPath
            };
        }

        private static Uri GetPageUri(string pageUri)
        {
            if(!string.IsNullOrEmpty(pageUri))
            {
                return new Uri($"ControlPages/{pageUri}.xaml", UriKind.Relative);
            }
            else
            {
                return null;
            }
        }
    }
}
