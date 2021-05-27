using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using EasyWPFUI.Media.ColorPalette;

namespace EasyWPFUI
{
    public class AppResources : ThemeResourceDictionary, ISupportInitialize
    {
        private ResourceDictionary themeDark;
        private ResourceDictionary themeHighContrast;
        private ResourceDictionary themeLight;
        private ResourceDictionary currentAccentColor;

        #region Properties

        public ApplicationTheme? RequestTheme { get; set; }

        public Color? AccentColor { get; set; }

        internal static AppResources Default { get; private set; }

        #endregion

        #region Methods

        public AppResources()
        {
            if (Default == null)
                Default = this;
        }

        public new void BeginInit()
        {
            base.BeginInit();
        }

        public new void EndInit()
        {
            base.EndInit();

            if(DesignMode.IsInDesignMode)
            {
                return;
            }

            ThemeManager.Initialize(RequestTheme, AccentColor);

            SetAccentColor(ThemeManager.AccentColor, false);
            SetApplicationTheme(ThemeManager.ApplicationTheme);

            LoadDefaultResources();

            // Update Theme Resources
            UpdateResources(ThemeManager.ApplicationTheme);
        }

        internal void SetApplicationTheme(ApplicationTheme theme)
        {
            RemoveIfExists(themeLight);

            RemoveIfExists(themeDark);

            RemoveIfExists(themeHighContrast);

            if (SystemParameters.HighContrast)
            {
                EnsureThemeResources(ref themeHighContrast, "ThemeResources/ThemeHighContrast.xaml", true);

                InsertThemeToDictionary(themeHighContrast);
            }
            else
            {
                if(theme == ApplicationTheme.Light)
                {
                    EnsureThemeResources(ref themeLight, "ThemeResources/ThemeLight.xaml", true);

                    InsertThemeToDictionary(themeLight);
                }

                if (theme == ApplicationTheme.Dark)
                {
                    EnsureThemeResources(ref themeDark, "ThemeResources/ThemeDark.xaml", true);

                    InsertThemeToDictionary(themeDark);
                }
            }
        }

        internal void SetAccentColor(Color accent, bool refreshTheme = true)
        {
            ColorPalette palette = new ColorPalette(11, accent, null);

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Add("SystemAccentColor", accent);
            dictionary.Add("SystemAccentColorDark1", palette.Palette[6].ActiveColor);
            dictionary.Add("SystemAccentColorDark2", palette.Palette[7].ActiveColor);
            dictionary.Add("SystemAccentColorDark3", palette.Palette[8].ActiveColor);
            dictionary.Add("SystemAccentColorLight1", palette.Palette[4].ActiveColor);
            dictionary.Add("SystemAccentColorLight2", palette.Palette[3].ActiveColor);
            dictionary.Add("SystemAccentColorLight3", palette.Palette[2].ActiveColor);

            if (currentAccentColor != null)
            {
                MergedDictionaries.Remove(currentAccentColor);
            }

            MergedDictionaries.Insert(0, dictionary);

            currentAccentColor = dictionary;

            // Refresh current theme
            if (refreshTheme)
            {
                SetApplicationTheme(ThemeManager.ApplicationTheme);
            }
        }

        internal void SetElementTheme(FrameworkElement element, ElementTheme theme)
        {
            if (theme == ElementTheme.Light)
            {
                EnsureThemeResources(ref themeLight, "ThemeResources/ThemeLight.xaml");

                if (themeDark != null && element.Resources.MergedDictionaries.Contains(themeDark))
                {
                    element.Resources.MergedDictionaries.Remove(themeDark);
                }

                element.Resources.MergedDictionaries.Add(themeLight);
            }

            if (theme == ElementTheme.Dark)
            {
                EnsureThemeResources(ref themeDark, "ThemeResources/ThemeDark.xaml");

                if (themeLight != null && element.Resources.MergedDictionaries.Contains(themeLight))
                {
                    element.Resources.MergedDictionaries.Remove(themeLight);
                }

                element.Resources.MergedDictionaries.Add(themeDark);
            }

            if (theme == ElementTheme.Default)
            {
                if (themeLight != null && element.Resources.MergedDictionaries.Contains(themeLight))
                {
                    element.Resources.MergedDictionaries.Remove(themeLight);
                }

                if (themeDark != null && element.Resources.MergedDictionaries.Contains(themeDark))
                {
                    element.Resources.MergedDictionaries.Remove(themeDark);
                }
            }
        }

        private void LoadDefaultResources()
        {
            ResourceDictionary defaultControls = new ResourceDictionary()
            {
                Source = GetResourcesUri("Controls.xaml")
            };

            MergedDictionaries.Insert(2, defaultControls);
        }

        private void EnsureThemeResources(ref ResourceDictionary themeDictionary, string sourcePath, bool refreshDictionary = false)
        {
            if (themeDictionary == null || refreshDictionary)
            {
                themeDictionary = new ResourceDictionary()
                {
                    Source = GetResourcesUri(sourcePath)
                };
            }
        }

        private void RemoveIfExists(ResourceDictionary themeDictionary)
        {
            if (themeDictionary != null)
            {
                MergedDictionaries.Remove(themeDictionary);
            }
        }

        private void InsertThemeToDictionary(ResourceDictionary themeDictionary)
        {
            MergedDictionaries.Insert(1, themeDictionary);
        }

        private Uri GetResourcesUri(string sourcePath)
        {
            return new Uri($"pack://application:,,,/EasyWPFUI;component/{sourcePath}");
        }

        #endregion
    }
}
