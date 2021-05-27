using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using EasyWPFUI.Media;

namespace EasyWPFUI
{
    public class ThemeManager
    {
        #region Events

        public static event EventHandler<ApplicationThemeChangedEventArgs> ApplicationThemeChanged;

        public static event EventHandler<ApplicationAccentColorChangedEventArgs> ApplicationAccentColorChanged;

        public static event EventHandler<ElementThemeChangedEventArgs> ElementThemeChanged;

        #endregion

        #region ApplicationTheme Property

        private static ApplicationTheme _applicationTheme;
        public static ApplicationTheme ApplicationTheme
        {
            get
            {
                return _applicationTheme;
            }
            set
            {
                _applicationTheme = value;

                ChangeApplicationTheme(value);
            }
        }

        #endregion

        #region ElementTheme Property

        public static readonly DependencyProperty ElementThemeProperty = DependencyProperty.RegisterAttached("ElementTheme", typeof(ElementTheme), typeof(ThemeManager), new FrameworkPropertyMetadata(ElementTheme.Default, OnElementThemePropertyChanged));

        public static ElementTheme GetElementTheme(UIElement ui)
        {
            return (ElementTheme)ui.GetValue(ElementThemeProperty);
        }

        public static void SetElementTheme(UIElement ui, ElementTheme value)
        {
            ui.SetValue(ElementThemeProperty, value);
        }

        private static void OnElementThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChangeElementTheme((FrameworkElement)d, (ElementTheme)e.NewValue);
        }

        #endregion

        #region UseThemeResources Property

        public static readonly DependencyProperty UseThemeResourcesProperty = DependencyProperty.RegisterAttached("UseThemeResources", typeof(bool), typeof(ThemeManager), new FrameworkPropertyMetadata(false, OnUseThemeResourcesPropertyChanged));

        public static bool GetUseThemeResources(FrameworkElement ui)
        {
            return (bool)ui.GetValue(UseThemeResourcesProperty);
        }

        public static void SetUseThemeResources(FrameworkElement ui, bool value)
        {
            ui.SetValue(UseThemeResourcesProperty, value);
        }

        private static void OnUseThemeResourcesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChangeUseThemeResources((FrameworkElement)d, (bool)e.NewValue);
        }

        #endregion

        #region AccentColor Property

        private static Color _accentColor;
        public static Color AccentColor
        {
            get
            {
                return _accentColor;
            }
            set
            {
                Color oldValue = _accentColor;
                _accentColor = value;

                ChangeAccentColor(oldValue, value);
            }
        }

        #endregion

        #region ThemeManager

        static ThemeManager()
        {

        }

        public static ApplicationTheme GetDefaultTheme()
        {
            return ApplicationTheme.Light;
        }

        public static Color GetDefaultAccentColor()
        {
            return AccentColors.DefaultBlue;
        }

        internal static void Initialize(ApplicationTheme? theme, Color? accentColor)
        {
            _accentColor = accentColor.HasValue ? accentColor.Value : GetDefaultAccentColor();

            _applicationTheme = theme.HasValue ? theme.Value : GetDefaultTheme();
        }

        private static void ChangeApplicationTheme(ApplicationTheme theme)
        {
            AppResources.Default.SetApplicationTheme(theme);

            ApplicationThemeChanged?.Invoke(null, new ApplicationThemeChangedEventArgs(theme));
        }

        private static void ChangeAccentColor(Color oldColor, Color newColor)
        {
            AppResources.Default.SetAccentColor(newColor);

            ApplicationAccentColorChanged?.Invoke(null, new ApplicationAccentColorChangedEventArgs(oldColor, newColor));
        }

        private static void ChangeElementTheme(FrameworkElement element, ElementTheme theme)
        {
            AppResources.Default.SetElementTheme(element, theme);

            ElementThemeChanged?.Invoke(null, new ElementThemeChangedEventArgs(element, theme));
        }

        private static void ChangeUseThemeResources(FrameworkElement element, bool value)
        {
            if(value)
            {
                if(element.IsInitialized)
                {
                    UpdateThemeResourcesElement(element);
                }
                else
                {
                    element.Initialized += OnUseThemeResourcesElementInitialized;
                }
            }
            else
            {
                // TODO ?
            }
        }

        private static void OnUseThemeResourcesElementInitialized(object sender, EventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            element.Initialized -= OnUseThemeResourcesElementInitialized;

            UpdateThemeResourcesElement(element);
        }

        private static void UpdateThemeResourcesElement(FrameworkElement element)
        {
            if (element.Resources is ThemeResourceDictionary dictionary)
            {
                dictionary.UpdateResources(ApplicationTheme);
            }
        }

        #endregion
    }
}
