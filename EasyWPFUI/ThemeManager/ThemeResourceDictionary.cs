using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI
{
    public class ThemeResourceDictionary : ResourceDictionary
    {
        private ResourceDictionary _currentDictionary;

        public Dictionary<object, ResourceDictionary> ThemeDictionaries { get; set; }

        public ThemeResourceDictionary()
        {
            ThemeDictionaries = new Dictionary<object, ResourceDictionary>();

            ThemeManager.ApplicationThemeChanged += OnApplicationThemeChanged;
        }

        private void OnApplicationThemeChanged(object sender, ApplicationThemeChangedEventArgs e)
        {
            UpdateResources(e.NewTheme);
        }

        internal void UpdateResources(ApplicationTheme theme)
        {
            ResourceDictionary dictionary;

            if (ThemeDictionaries.TryGetValue(theme.ToString(), out dictionary))
            {
                if (_currentDictionary != null && MergedDictionaries.Contains(_currentDictionary))
                {
                    MergedDictionaries.Remove(_currentDictionary);
                }

                MergedDictionaries.Add(dictionary);
                _currentDictionary = dictionary;
            }
            else
            {
                if (_currentDictionary != null && MergedDictionaries.Contains(_currentDictionary))
                {
                    MergedDictionaries.Remove(_currentDictionary);
                }
            }
        }
    }
}
