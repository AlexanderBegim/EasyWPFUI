using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public enum AutoSuggestionBoxTextChangeReason
    {
        ProgrammaticChange = 1,
        SuggestionChosen = 2,
        UserInput = 0
    }

    public class AutoSuggestBoxTextChangedEventArgs : DependencyObject
    {
        public static readonly DependencyProperty ReasonProperty = DependencyProperty.Register("Reason", typeof(AutoSuggestionBoxTextChangeReason), typeof(AutoSuggestBoxTextChangedEventArgs), new PropertyMetadata(AutoSuggestionBoxTextChangeReason.ProgrammaticChange));

        public AutoSuggestionBoxTextChangeReason Reason
        {
            get
            {
                return (AutoSuggestionBoxTextChangeReason)GetValue(ReasonProperty);
            }
            set
            {
                SetValue(ReasonProperty, value);
            }
        }

        public AutoSuggestBoxTextChangedEventArgs(AutoSuggestionBoxTextChangeReason reason)
        {
            Reason = reason;
        }

        public bool CheckCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
