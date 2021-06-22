// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using EasyWPFUI;
using EasyWPFUI.Controls;
using EasyWPFUI.Controls.Primitives;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public class NumberBox : Control
    {
        private const string c_numberBoxHeaderName = "HeaderContentPresenter";
        private const string c_numberBoxDownButtonName = "DownSpinButton";
        private const string c_numberBoxUpButtonName = "UpSpinButton";
        private const string c_numberBoxTextBoxName = "InputBox";
        private const string c_numberBoxPopupButtonName = "PopupButton";
        private const string c_numberBoxPopupName = "UpDownPopup";
        private const string c_numberBoxPopupDownButtonName = "PopupDownSpinButton";
        private const string c_numberBoxPopupUpButtonName = "PopupUpSpinButton";
        private const string c_numberBoxPopupContentRootName = "PopupContentRoot";

        private const double c_popupShadowDepth = 16.0;
        private const string c_numberBoxPopupShadowDepthName = "NumberBoxPopupShadowDepth";

        private bool m_valueUpdating = false;
        private bool m_textUpdating = false;

        private TextBox m_textBox;
        private Popup m_popup;
        private ContentPresenter m_headerPresenter;

        #region Events

        public event TypedEventHandler<NumberBox,NumberBoxValueChangedEventArgs> ValueChanged;

        #endregion

        #region Minimum Property

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(double.MinValue, OnMinimumPropertyChanged));

        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnMinimumPropertyChanged(e);
        }

        #endregion

        #region Maximim Property

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(double.MaxValue, OnMaximumPropertyChanged));

        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnMaximumPropertyChanged(e);
        }

        #endregion

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(double.NaN, OnValuePropertyChanged));

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnValuePropertyChanged(e);
        }

        #endregion

        #region SmallChange Property

        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(1d, OnSmallChangePropertyChanged));

        public double SmallChange
        {
            get
            {
                return (double)GetValue(SmallChangeProperty);
            }
            set
            {
                SetValue(SmallChangeProperty, value);
            }
        }

        private static void OnSmallChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnSmallChangePropertyChanged(e);
        }

        #endregion

        #region LargeChange Property

        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(10d));

        public double LargeChange
        {
            get
            {
                return (double)GetValue(LargeChangeProperty);
            }
            set
            {
                SetValue(LargeChangeProperty, value);
            }
        }

        #endregion

        #region Text Property

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NumberBox), new FrameworkPropertyMetadata(OnTextPropertyChanged));

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
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnTextPropertyChanged(e);
        }

        #endregion

        #region Header Property

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(NumberBox), new FrameworkPropertyMetadata(OnHeaderPropertyChanged));

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

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnHeaderPropertyChanged(e);
        }

        #endregion

        #region HeaderTemplate Property

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NumberBox), new FrameworkPropertyMetadata(OnHeaderTemplatePropertyChanged));

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }
            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnHeaderTemplatePropertyChanged(e);
        }

        #endregion

        #region SelectionFlyoutProperty Property

        public static readonly DependencyProperty SelectionFlyoutProperty = DependencyProperty.Register("SelectionFlyout", typeof(FlyoutBase), typeof(NumberBox), new FrameworkPropertyMetadata(null));

        public FlyoutBase SelectionFlyout
        {
            get
            {
                return (FlyoutBase)GetValue(SelectionFlyoutProperty);
            }
            set
            {
                SetValue(SelectionFlyoutProperty, value);
            }
        }

        #endregion

        #region SelectionHighlightColor Property

        public static readonly DependencyProperty SelectionHighlightColorProperty = DependencyProperty.Register("SelectionHighlightColor", typeof(Brush), typeof(NumberBox), new FrameworkPropertyMetadata(null));

        public Brush SelectionHighlightColor
        {
            get
            {
                return (Brush)GetValue(SelectionHighlightColorProperty);
            }
            set
            {
                SetValue(SelectionHighlightColorProperty, value);
            }
        }

        #endregion

        #region PlaceholderText Property

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(NumberBox), new FrameworkPropertyMetadata(null));

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

        #region TextAlignment Property

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumberBox), new FrameworkPropertyMetadata(TextAlignment.Left));

        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        #endregion

        #region Description Property

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(object), typeof(NumberBox), new FrameworkPropertyMetadata(null));

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

        #region ValidationMode Property

        public static readonly DependencyProperty ValidationModeProperty = DependencyProperty.Register("ValidationMode", typeof(NumberBoxValidationMode), typeof(NumberBox), new FrameworkPropertyMetadata(OnValidationModePropertyChanged));

        public NumberBoxValidationMode ValidationMode
        {
            get
            {
                return (NumberBoxValidationMode)GetValue(ValidationModeProperty);
            }
            set
            {
                SetValue(ValidationModeProperty, value);
            }
        }

        private static void OnValidationModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnValidationModePropertyChanged(e);
        }

        #endregion

        #region SpinButtonPlacementMode Property

        public static readonly DependencyProperty SpinButtonPlacementModeProperty = DependencyProperty.Register("SpinButtonPlacementMode", typeof(NumberBoxSpinButtonPlacementMode), typeof(NumberBox), new FrameworkPropertyMetadata(NumberBoxSpinButtonPlacementMode.Hidden, OnSpinButtonPlacementModePropertyChanged));

        public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
        {
            get
            {
                return (NumberBoxSpinButtonPlacementMode)GetValue(SpinButtonPlacementModeProperty);
            }
            set
            {
                SetValue(SpinButtonPlacementModeProperty, value);
            }
        }

        private static void OnSpinButtonPlacementModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnSpinButtonPlacementModePropertyChanged(e);
        }

        #endregion

        #region IsWrapEnabled Property

        public static readonly DependencyProperty IsWrapEnabledProperty = DependencyProperty.Register("IsWrapEnabled", typeof(bool), typeof(NumberBox), new FrameworkPropertyMetadata(false, OnIsWrapEnabledPropertyChanged));

        public bool IsWrapEnabled
        {
            get
            {
                return (bool)GetValue(IsWrapEnabledProperty);
            }
            set
            {
                SetValue(IsWrapEnabledProperty, value);
            }
        }

        private static void OnIsWrapEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnIsWrapEnabledPropertyChanged(e);
        }

        #endregion

        #region AcceptsExpression Property

        public static readonly DependencyProperty AcceptsExpressionProperty = DependencyProperty.Register("AcceptsExpression", typeof(bool), typeof(NumberBox), new FrameworkPropertyMetadata(false));

        public bool AcceptsExpression
        {
            get
            {
                return (bool)GetValue(AcceptsExpressionProperty);
            }
            set
            {
                SetValue(AcceptsExpressionProperty, value);
            }
        }

        #endregion

        #region NumberFormatter Property

        public static readonly DependencyProperty NumberFormatterProperty = DependencyProperty.Register("NumberFormatter", typeof(INumberBoxFormatter), typeof(NumberBox), new FrameworkPropertyMetadata(OnNumberFormatterPropertyChanged));

        public INumberBoxFormatter NumberFormatter
        {
            get
            {
                return (INumberBoxFormatter)GetValue(NumberFormatterProperty);
            }
            set
            {
                SetValue(NumberFormatterProperty, value);
            }
        }

        private static void OnNumberFormatterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnNumberFormatterPropertyChanged(e);
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NumberBox), new FrameworkPropertyMetadata(OnCornerRadiusPropertyChanged));

        public bool CornerRadius
        {
            get
            {
                return (bool)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = d as NumberBox;

            if (numberBox == null)
                return;

            numberBox.OnCornerRadiusPropertyChanged(e);
        }

        #endregion


        #region Methods

        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

        public NumberBox()
        {
            NumberFormatter = GetRegionalSettingsAwareDecimalFormatter();

            MouseWheel += OnNumberBoxScroll;
            GotKeyboardFocus += OnNumberBoxGotFocus;
            LostKeyboardFocus += OnNumberBoxLostFocus;

            SetDefaultInputScope();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            string spinUpName = Properties.Resources.Strings.Resources.NumberBoxUpSpinButtonName;
            string spinDownName = Properties.Resources.Strings.Resources.NumberBoxDownSpinButtonName;

            RepeatButton spinDown = GetTemplateChild(c_numberBoxDownButtonName) as RepeatButton;

            if (spinDown != null)
            {
                spinDown.Click += OnSpinDownClick;

                // Do localization for the down button
                if (string.IsNullOrEmpty(AutomationProperties.GetName(spinDown)))
                {
                    AutomationProperties.SetName(spinDown, spinDownName);
                }
            }

            RepeatButton spinUp = GetTemplateChild(c_numberBoxUpButtonName) as RepeatButton;

            if (spinUp != null)
            {
                spinUp.Click += OnSpinUpClick; ;

                // Do localization for the down button
                if (string.IsNullOrEmpty(AutomationProperties.GetName(spinUp)))
                {
                    AutomationProperties.SetName(spinUp, spinUpName);
                }
            }

            UpdateHeaderPresenterState();

            m_textBox = GetTemplateChild(c_numberBoxTextBoxName) as TextBox;

            if (m_textBox != null)
            {
                m_textBox.PreviewKeyDown += OnNumberBoxKeyDown;

                m_textBox.KeyUp += OnNumberBoxKeyUp;
            }

            m_popup = GetTemplateChild(c_numberBoxPopupName) as Popup;

            if(m_popup != null)
            {
                m_popup.Opened += OnPopupOpened;
                m_popup.Closed += OnPopupClosed;
            }

            RepeatButton popupSpinDown = GetTemplateChild(c_numberBoxPopupDownButtonName) as RepeatButton;

            if (popupSpinDown != null)
            {
                popupSpinDown.Click += OnSpinDownClick;
            }

            RepeatButton popupSpinUp = GetTemplateChild(c_numberBoxPopupUpButtonName) as RepeatButton;

            if (popupSpinUp != null)
            {
                popupSpinUp.Click += OnSpinUpClick;
            }

            IsEnabledChanged += OnIsEnabledChanged;

            UpdateSpinButtonPlacement();
            UpdateSpinButtonEnabled();

            UpdateVisualStateForIsEnabledChange();

            if (ReadLocalValue(ValueProperty) == DependencyProperty.UnsetValue && ReadLocalValue(TextProperty) != DependencyProperty.UnsetValue)
            {
                UpdateValueToText();
            }
            else
            {
                UpdateTextToValue();
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NumberBoxAutomationPeer(this);
        }

        private void SetDefaultInputScope()
        {
            // Sets the default value of the InputScope property.
            // Note that InputScope is a class that cannot be set to a default value within the IDL.
            InputScopeName inputScopeName = new InputScopeName(InputScopeNameValue.Number);
            InputScope inputScope = new InputScope();
            inputScope.Names.Add(inputScopeName);

            SetValue(InputScopeProperty, inputScope);

            return;
        }

        private INumberBoxFormatter GetRegionalSettingsAwareDecimalFormatter()
        {
            return new DefaultNumberBoxFormatter();
        }

        private void UpdateValueToText()
        {
            if (m_textBox != null)
            {
                m_textBox.Text = Text;
                ValidateInput();
            }
        }

        private void CoerceMinimum()
        {
            double max = Maximum;

            if (Minimum > max)
            {
                Minimum = max;
            }
        }

        private void CoerceMaximum()
        {
            double min = Minimum;

            if (Maximum < min)
            {
                Maximum = min;
            }
        }

        private void CoerceValue()
        {
            // Validate that the value is in bounds
            double value = Value;

            if (!double.IsNaN(value) && !IsInBounds(value) && ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
            {
                // Coerce value to be within range
                double max = Maximum;

                if (value > max)
                {
                    Value = max;
                }
                else
                {
                    Value = Minimum;
                }
            }
        }

        private void ValidateInput()
        {
            // Validate the content of the inner textbox
            if (m_textBox != null)
            {
                string text = m_textBox.Text.Trim();

                // Handles empty TextBox case, set text to current value
                if (string.IsNullOrEmpty(text))
                {
                    Value = double.NaN;
                }
                else
                {
                    // Setting NumberFormatter to something that isn't an INumberParser will throw an exception, so this should be safe
                    INumberBoxFormatter numberParser = NumberFormatter;

                    double? value = AcceptsExpression ? NumberBoxParser.Compute(text, numberParser) : numberParser.Parse(text);

                    if (value == null) // TODO Check this
                    {
                        if (ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
                        {
                            // Override text to current value
                            UpdateTextToValue();
                        }
                    }
                    else
                    {
                        if (value.Value == Value)
                        {
                            // Even if the value hasn't changed, we still want to update the text (e.g. Value is 3, user types 1 + 2, we want to replace the text with 3)
                            UpdateTextToValue();
                        }
                        else
                        {
                            Value = value.Value;
                        }
                    }
                }
            }
        }

        private void StepValue(double change)
        {
            // Before adjusting the value, validate the contents of the textbox so we don't override it.
            ValidateInput();

            double newVal = Value;

            if (!double.IsNaN(newVal))
            {
                newVal += change;

                if (IsWrapEnabled)
                {
                    double max = Maximum;
                    double min = Minimum;

                    if (newVal > max)
                    {
                        newVal = min;
                    }
                    else if (newVal < min)
                    {
                        newVal = max;
                    }
                }

                Value = newVal;

                // We don't want the caret to move to the front of the text for example when using the up/down arrows
                // to change the numberbox value.
                MoveCaretToTextEnd();
            }
        }

        private void UpdateTextToValue()
        {
            if (m_textBox != null)
            {
                string newText = string.Empty;

                double value = Value;

                if (!double.IsNaN(value))
                {
                    // Rounding the value here will prevent displaying digits caused by floating point imprecision.
                    // double roundedValue = m_displayRounder.RoundDouble(value);
                    double roundedValue = Convert.ToDouble(value.ToString("G12"));
                    newText = NumberFormatter.Format(roundedValue);
                }

                m_textBox.Text = newText;

                try
                {
                    m_textUpdating = true;

                    Text = newText;
                }
                finally
                {
                    m_textUpdating = false;
                }
            }
        }

        private void UpdateSpinButtonPlacement()
        {
            NumberBoxSpinButtonPlacementMode spinButtonMode = SpinButtonPlacementMode;

            if (spinButtonMode == NumberBoxSpinButtonPlacementMode.Inline)
            {
                VisualStateManager.GoToState(this, "SpinButtonsVisible", false);
            }
            else if (spinButtonMode == NumberBoxSpinButtonPlacementMode.Compact)
            {
                VisualStateManager.GoToState(this, "SpinButtonsPopup", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "SpinButtonsCollapsed", false);
            }
        }

        private void UpdateSpinButtonEnabled()
        {
            double value = Value;
            bool isUpButtonEnabled = false;
            bool isDownButtonEnabled = false;

            if (!double.IsNaN(value))
            {
                if (IsWrapEnabled || ValidationMode != NumberBoxValidationMode.InvalidInputOverwritten)
                {
                    // If wrapping is enabled, or invalid values are allowed, then the buttons should be enabled
                    isUpButtonEnabled = true;
                    isDownButtonEnabled = true;
                }
                else
                {
                    if (value < Maximum)
                    {
                        isUpButtonEnabled = true;
                    }
                    if (value > Minimum)
                    {
                        isDownButtonEnabled = true;
                    }
                }
            }

            VisualStateManager.GoToState(this, isUpButtonEnabled ? "UpSpinButtonEnabled" : "UpSpinButtonDisabled", false);
            VisualStateManager.GoToState(this, isDownButtonEnabled ? "DownSpinButtonEnabled" : "DownSpinButtonDisabled", false);
        }

        private bool IsInBounds(double value)
        {
            return (value >= Minimum && value <= Maximum);
        }

        private void UpdateHeaderPresenterState()
        {
            bool shouldShowHeader = false;

            // Load header presenter as late as possible

            // To enable lightweight styling, collapse header presenter if there is no header specified
            object header = Header;
            if (header != null)
            {
                // Check if header is string or not
                if (header is string headerAsString)
                {
                    if (!string.IsNullOrEmpty(headerAsString))
                    {
                        // Header is not empty string
                        shouldShowHeader = true;
                    }
                }
                else
                {
                    // Header is not a string, so let's show header presenter
                    shouldShowHeader = true;
                }
            }

            DataTemplate headerTemplate = HeaderTemplate;

            if (headerTemplate != null)
            {
                shouldShowHeader = true;
            }

            if (shouldShowHeader && m_headerPresenter == null)
            {
                ContentPresenter headerPresenter = GetTemplateChild(c_numberBoxHeaderName) as ContentPresenter;
                if (headerPresenter != null)
                {
                    // Set presenter to enable lightweight styling of the headers margin
                    m_headerPresenter = headerPresenter;
                }
            }


            if (m_headerPresenter != null)
            {
                m_headerPresenter.Visibility = (shouldShowHeader) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void MoveCaretToTextEnd()
        {
            if (m_textBox != null)
            {
                // This places the caret at the end of the text.
                m_textBox.Select(m_textBox.Text.Length, 0);
            }
        }

        private void ValidateNumberFormatter(INumberBoxFormatter value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Value");
            }
        }

        private void UpdateVisualStateForIsEnabledChange()
        {
            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);
        }


        private void OnNumberBoxScroll(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if(m_textBox != null)
            {
                if(m_textBox.IsFocused)
                {
                    int delta = e.Delta;

                    if(delta > 0)
                    {
                        StepValue(SmallChange);
                    }
                    else if(delta < 0)
                    {
                        StepValue(-SmallChange);
                    }

                    // Only set as handled when we actually changed our state.
                    e.Handled = true;
                }
            }
        }

        private void OnNumberBoxGotFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            // When the control receives focus, select the text
            if (m_textBox != null)
            {
                m_textBox.SelectAll();
            }

            if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Compact)
            {
                if (m_popup != null)
                {
                    m_popup.IsOpen = true;
                }
            }
        }

        private void OnNumberBoxLostFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            ValidateInput();

            if (m_popup != null)
            {
                m_popup.IsOpen = false;
            }
        }

        private void OnSpinUpClick(object sender, RoutedEventArgs e)
        {
            StepValue(SmallChange);
        }

        private void OnSpinDownClick(object sender, RoutedEventArgs e)
        {
            StepValue(-SmallChange);
        }

        private void OnNumberBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Handle these on key down so that we get repeat behavior.
            switch (e.Key)
            {
                case Key.Up:
                    StepValue(SmallChange);
                    e.Handled = true;
                    break;

                case Key.Down:
                    StepValue(-SmallChange);
                    e.Handled = true;
                    break;

                case Key.PageUp:
                    StepValue(LargeChange);
                    e.Handled = true;
                    break;

                case Key.PageDown:
                    StepValue(-LargeChange);
                    e.Handled = true;
                    break;
            }
        }

        private void OnNumberBoxKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ValidateInput();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    UpdateTextToValue();
                    e.Handled = true;
                    break;
            }
        }


        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualStateForIsEnabledChange();
        }

        private void OnMinimumPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceMaximum();
            CoerceValue();

            UpdateSpinButtonEnabled();
        }

        private void OnMaximumPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceMinimum();
            CoerceValue();

            UpdateSpinButtonEnabled();
        }

        private void OnValuePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // This handler may change Value; don't send extra events in that case.
            if (!m_valueUpdating)
            {
                double oldValue = (double)e.OldValue;

                try
                {
                    m_valueUpdating = true;

                    CoerceValue();

                    double newValue = Value;

                    if (newValue != oldValue && !(double.IsNaN(oldValue)) && !(double.IsNaN(newValue)))
                    {
                        // Fire ValueChanged event
                        NumberBoxValueChangedEventArgs valueChangedArgs = new NumberBoxValueChangedEventArgs(oldValue, newValue);
                        ValueChanged?.Invoke(this, valueChangedArgs);

                        // Fire value property change for UIA
                        NumberBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as NumberBoxAutomationPeer;

                        if (peer != null)
                        {
                            peer.RaiseValueChangedEvent(oldValue, newValue);
                        }
                    }

                    UpdateTextToValue();
                    UpdateSpinButtonEnabled();
                }
                finally
                {
                    m_valueUpdating = false;
                }
            }
        }

        private void OnSmallChangePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateSpinButtonEnabled();
        }

        private void OnTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!m_textUpdating)
            {
                UpdateValueToText();
            }
        }

        private void OnHeaderPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateHeaderPresenterState();
        }

        private void OnHeaderTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateHeaderPresenterState();
        }

        private void OnValidationModePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ValidateInput();
            UpdateSpinButtonEnabled();
        }

        private void OnSpinButtonPlacementModePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateSpinButtonPlacement();
        }

        private void OnIsWrapEnabledPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateSpinButtonEnabled();
        }

        private void OnNumberFormatterPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update text with new formatting
            UpdateTextToValue();
        }

        private void OnCornerRadiusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Inline)
            {
                // Enforce T-rule for the textbox in Inline SpinButtonPlacementMode.
                VisualStateManager.GoToState(this, "SpinButtonsVisible", false);
            }
        }


        private void OnPopupOpened(object sender, EventArgs e)
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (m_popup != null && m_popup.IsOpen)
            {
                double offset = m_popup.HorizontalOffset;
                m_popup.HorizontalOffset = offset + 1;
                m_popup.HorizontalOffset = offset;
            }
        }

        #endregion
    }
}
