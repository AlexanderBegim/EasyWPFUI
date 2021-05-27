using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using EasyWPFUI.Automation.Peers;
using EasyWPFUI.Converters;
using EasyWPFUI.Controls.Helpers;

namespace EasyWPFUI.Controls
{
    public class AutoSuggestBox : ItemsControl
    {
        #region Events

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

        #endregion

        #region Description Property

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(object), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public object Description
        {
            get
            {
                return GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        #endregion

        #region Header Property

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        #endregion

        #region IsSuggestionListOpen Property

        public static readonly DependencyProperty IsSuggestionListOpenProperty = DependencyProperty.Register("IsSuggestionListOpen", typeof(bool), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public bool IsSuggestionListOpen
        {
            get
            {
                return (bool)GetValue(IsSuggestionListOpenProperty);
            }
            set
            {
                SetValue(IsSuggestionListOpenProperty, value);
            }
        }

        #endregion

        #region MaxSuggestionListHeight Property

        public static readonly DependencyProperty MaxSuggestionListHeightProperty = DependencyProperty.Register("MaxSuggestionListHeight", typeof(double), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(0d));

        public double MaxSuggestionListHeight
        {
            get
            {
                return (double)GetValue(MaxSuggestionListHeightProperty);
            }
            set
            {
                SetValue(MaxSuggestionListHeightProperty, value);
            }
        }

        #endregion

        #region PlaceholderText Property

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public string PlaceholderText
        {
            get
            {
                return (string)GetValue(PlaceholderTextProperty);
            }
            set
            {
                SetValue(PlaceholderTextProperty, value);
            }
        }

        #endregion

        #region QueryIcon Property

        public static readonly DependencyProperty QueryIconProperty = DependencyProperty.Register("QueryIcon", typeof(IconElement), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public IconElement QueryIcon
        {
            get
            {
                return (IconElement)GetValue(QueryIconProperty);
            }
            set
            {
                SetValue(QueryIconProperty, value);
            }
        }

        #endregion

        #region Text Property

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(OnTextPropertyChanged));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoSuggestBox asb = d as AutoSuggestBox;

            asb.OnTextPropertyChanged();
        }

        #endregion

        #region TextBoxStyle Property

        public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public Style TextBoxStyle
        {
            get
            {
                return (Style)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion

        #region TextMemberPath Property

        public static readonly DependencyProperty TextMemberPathProperty = DependencyProperty.Register("TextMemberPath", typeof(string), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public string TextMemberPath
        {
            get
            {
                return (string)GetValue(TextMemberPathProperty);
            }
            set
            {
                SetValue(TextMemberPathProperty, value);
            }
        }

        #endregion

        #region UpdateTextOnSelect Property

        public static readonly DependencyProperty UpdateTextOnSelectProperty = DependencyProperty.Register("UpdateTextOnSelect", typeof(bool), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public bool UpdateTextOnSelect
        {
            get
            {
                return (bool)GetValue(UpdateTextOnSelectProperty);
            }
            set
            {
                SetValue(UpdateTextOnSelectProperty, value);
            }
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(AutoSuggestBox), new FrameworkPropertyMetadata());

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        #endregion


        #region Methods

        static AutoSuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(typeof(AutoSuggestBox)));
        }

        public AutoSuggestBox()
        {
            IsKeyboardFocusWithinChanged += OnAutoSuggestBoxIsKeyboardFocusWithinChanged;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AutoSuggestBoxAutomationPeer(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild(c_suggestionsPopup) is Popup suggestionsPopup)
            {
                m_suggestionsPopup = suggestionsPopup;

                suggestionsPopup.Opened += OnSuggestionsPopupOpened;
                suggestionsPopup.Closed += OnSuggestionsPopupClosed;
            }

            if (GetTemplateChild(c_popupBorderName) is Border popupBorder)
            {
                m_popupBorder = popupBorder;
            }

            if (GetTemplateChild(c_textBoxName) is TextBox textBox)
            {
                textBox.ApplyTemplate();

                m_textBox = textBox;

                textBox.TextChanged += OnTextBoxTextChanged;
                textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;

                if (textBox.Template?.FindName(c_queryButtonName, textBox) is Button queryButton)
                {
                    m_queryButton = queryButton;

                    queryButton.Click += OnQueryButtonClick;
                }
            }

            if (GetTemplateChild(c_suggestionsList) is ListView suggestionsList)
            {
                m_suggestionsList = suggestionsList;

                suggestionsList.SelectionChanged += OnSuggestionsListSelectionChanged;
                suggestionsList.PreviewMouseDown += OnSuggestionsListPreviewMouseDown;
                suggestionsList.PreviewMouseUp += OnSuggestionsListPreviewMouseUp;
            }
        }

        private void OnSuggestionChosen(object selectedItem)
        {
            SuggestionChosen?.Invoke(this, new AutoSuggestBoxSuggestionChosenEventArgs(m_suggestionsList.SelectedItem));
        }

        private void OnQuerySubmitted(bool queryButtonInvoke = false)
        {
            AutoSuggestBoxQuerySubmittedEventArgs args = new AutoSuggestBoxQuerySubmittedEventArgs(Text);

            if (!queryButtonInvoke && IsSuggestionListOpen && m_suggestionsList != null && m_suggestionsList.SelectedItem != null)
            {
                args.ChosenSuggestion = m_suggestionsList.SelectedItem;
            }

            QuerySubmitted?.Invoke(this, args);
        }

        private void OnTextChanged(AutoSuggestionBoxTextChangeReason reason)
        {
            TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs(reason));
        }

        private void UpdateCornerRadius(bool isPopupOpen)
        {
            if(m_popupBorder != null && m_textBox != null)
            {
                CornerRadius textBoxRadius = (CornerRadius)(GetValue(ControlHelper.CornerRadiusProperty) ?? Application.Current.Resources[c_controlCornerRadiusKey]);
                CornerRadius popupRadius = (CornerRadius)Application.Current.Resources[c_overlayCornerRadiusKey];

                if (isPopupOpen)
                {
                    bool isOpenDown = IsPopupOpenDown();
                    CornerRadiusFilterConverter cornerRadiusConverter = new CornerRadiusFilterConverter();

                    CornerRadiusFilterKind popupRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Bottom : CornerRadiusFilterKind.Top;
                    popupRadius = cornerRadiusConverter.Convert(popupRadius, popupRadiusFilter);

                    CornerRadiusFilterKind textBoxRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom;
                    textBoxRadius = cornerRadiusConverter.Convert(textBoxRadius, textBoxRadiusFilter);
                }

                m_popupBorder.CornerRadius = popupRadius;
                m_textBox.SetValue(ControlHelper.CornerRadiusProperty, textBoxRadius);
            }
        }

        private bool IsPopupOpenDown()
        {
            double verticalOffset = 0;

            if(m_popupBorder != null && m_textBox != null)
            {
                Point popupTop = m_popupBorder.TranslatePoint(new Point(0, 0), m_textBox);
                verticalOffset = popupTop.Y;
            }

            return verticalOffset > 0;
        }

        /* Property Changed */

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if(m_suggestionsList != null)
            {
                m_ignoreTextChanges = true;
                m_suggestionsList.ClearValue(ListView.SelectedIndexProperty);
                m_suggestionsList.ClearValue(ListView.SelectedItemProperty);
                m_ignoreTextChanges = false;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (HasItems && IsKeyboardFocusWithin)
            {
                IsSuggestionListOpen = true;
            }
            else
            {
                IsSuggestionListOpen = false;
            }
        }

        private void OnTextPropertyChanged()
        {
            if (m_ignoreTextChanges)
            {
                return;
            }

            m_ignoreTextChanges = true;

            if (m_textBox != null)
            {
                m_textBox.Text = Text;
            }

            m_ignoreTextChanges = false;

            OnTextChanged(AutoSuggestionBoxTextChangeReason.ProgrammaticChange);
        }

        /* Event Handlers */

        private void OnAutoSuggestBoxIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(bool)e.NewValue && IsSuggestionListOpen)
            {
                IsSuggestionListOpen = false;
            }
        }

        private void OnSuggestionsPopupOpened(object sender, EventArgs e)
        {
           UpdateCornerRadius(true);
        }

        private void OnSuggestionsPopupClosed(object sender, EventArgs e)
        {
            UpdateCornerRadius(false);
        }

        private void OnSuggestionsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_ignoreTextChanges || m_ignoreSelectionChanges)
            {
                return;
            }

            if (m_suggestionsList.SelectedItem != null)
            {
                OnSuggestionChosen(m_suggestionsList.SelectedItem);

                if(m_isSuggestionsListMouseDown)
                {
                    OnQuerySubmitted();
                }

                m_suggestionsList.ScrollIntoView(m_suggestionsList.SelectedItem);

                if (UpdateTextOnSelect)
                {
                    m_ignoreTextChanges = true;

                    Text = m_textBox.Text = m_suggestionsList.SelectedValue?.ToString() ?? string.Empty;

                    m_ignoreTextChanges = false;

                    OnTextChanged(AutoSuggestionBoxTextChangeReason.SuggestionChosen);
                }
            }
        }

        private void OnSuggestionsListPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_isSuggestionsListMouseDown = true;
        }

        private void OnSuggestionsListPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsSuggestionListOpen = false;
            m_isSuggestionsListMouseDown = false;
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                IsSuggestionListOpen = false;

                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                OnQuerySubmitted();

                if (IsSuggestionListOpen)
                {
                    IsSuggestionListOpen = false;
                }

                e.Handled = true;
            }

            if(e.Key == Key.Up)
            {
                if (IsSuggestionListOpen && m_suggestionsList != null && m_suggestionsList.Items != null && m_suggestionsList.Items.Count > 0)
                {
                    if (m_suggestionsList.SelectedIndex > -1 && m_suggestionsList.SelectedIndex <= m_suggestionsList.Items.Count - 1)
                    {
                        m_suggestionsList.SelectedIndex--;
                    }
                    else
                    {
                        m_suggestionsList.SelectedIndex = m_suggestionsList.Items.Count - 1;
                    }
                }

                e.Handled = true;
            }

            if(e.Key == Key.Down)
            {
                if(IsSuggestionListOpen && m_suggestionsList != null && m_suggestionsList.Items != null && m_suggestionsList.Items.Count > 0)
                {
                    if(m_suggestionsList.SelectedIndex < m_suggestionsList.Items.Count - 1)
                    {
                        m_suggestionsList.SelectedIndex++;
                    }
                    else
                    {
                        m_suggestionsList.SelectedIndex = -1;
                    }
                }

                e.Handled = true;
            }
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if(m_ignoreTextChanges)
            {
                return;
            }

            m_ignoreTextChanges = true;

            Text = m_textBox.Text;

            m_ignoreTextChanges = false;

            OnTextChanged(AutoSuggestionBoxTextChangeReason.UserInput);
        }

        private void OnQueryButtonClick(object sender, RoutedEventArgs e)
        {
            OnQuerySubmitted(true);
        }

        #endregion

        private const string c_suggestionsPopup = "SuggestionsPopup";
        private const string c_textBoxName = "TextBox";
        private const string c_queryButtonName = "QueryButton";
        private const string c_suggestionsList = "SuggestionsList";
        private const string c_popupBorderName = "SuggestionsContainer";
        private const string c_controlCornerRadiusKey = "ControlCornerRadius";
        private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

        private Popup m_suggestionsPopup;
        private TextBox m_textBox;
        private Button m_queryButton;
        private ListView m_suggestionsList;
        private Border m_popupBorder;

        private bool m_ignoreTextChanges = false;
        private bool m_ignoreSelectionChanges = false;
        private bool m_isSuggestionsListMouseDown = false;
    }
}
