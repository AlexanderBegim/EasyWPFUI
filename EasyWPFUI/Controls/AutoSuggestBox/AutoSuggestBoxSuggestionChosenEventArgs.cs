using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class AutoSuggestBoxSuggestionChosenEventArgs : DependencyObject
    {
        public object SelectedItem { get; internal set; }

        public AutoSuggestBoxSuggestionChosenEventArgs(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }
}
