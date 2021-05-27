using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class AutoSuggestBoxQuerySubmittedEventArgs : DependencyObject
    {
        public object ChosenSuggestion { get; internal set; }

        public string QueryText { get; internal set; }

        public AutoSuggestBoxQuerySubmittedEventArgs()
        {

        }

        public AutoSuggestBoxQuerySubmittedEventArgs(string queryText)
        {
            ChosenSuggestion = null;

            QueryText = queryText;
        }

        public AutoSuggestBoxQuerySubmittedEventArgs(object chosenSuggestion, string queryText)
        {
            ChosenSuggestion = chosenSuggestion;

            QueryText = queryText;
        }
    }
}
