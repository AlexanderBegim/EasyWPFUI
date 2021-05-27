using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    [TemplatePart(Name = nameof(HeaderContentPresenter), Type = typeof(ContentPresenter))]
    [TemplatePart(Name = nameof(SwitchKnobBounds), Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(SwitchKnob), Type = typeof(FrameworkElement))]
    [TemplatePart(Name = nameof(KnobTranslateTransform), Type = typeof(TranslateTransform))]
    [TemplatePart(Name = nameof(SwitchThumb), Type = typeof(Thumb))]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = CommonStateNormal)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = CommonStateMouseOver)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = CommonStatePressed)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = CommonStateDisabled)]
    [TemplateVisualState(GroupName = ContentStatesGroup, Name = OffContentState)]
    [TemplateVisualState(GroupName = ContentStatesGroup, Name = OnContentState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = DraggingState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = OffState)]
    [TemplateVisualState(GroupName = ToggleStatesGroup, Name = OnState)]
    [ContentProperty(nameof(Header))]
    public class ToggleSwitch : Control
    {
        private const string CommonStatesGroup = "CommonStates";
        private const string ContentStatesGroup = "ContentStates";
        private const string OffContentState = "OffContent";
        private const string OnContentState = "OnContent";
        private const string ToggleStatesGroup = "ToggleStates";
        private const string DraggingState = "Dragging";
        private const string OffState = "Off";
        private const string OnState = "On";
        private const string CommonStateNormal = "Normal";
        private const string CommonStateMouseOver = "PointerOver";
        private const string CommonStatePressed = "Pressed";
        private const string CommonStateDisabled = "Disabled";

        private double _onTranslation;
        private double _startTranslation;
        private bool _wasDragged;

        #region Properties

        private ContentPresenter HeaderContentPresenter { get; set; }

        private FrameworkElement SwitchKnobBounds { get; set; }

        private FrameworkElement SwitchKnob { get; set; }

        private TranslateTransform KnobTranslateTransform { get; set; }

        private Thumb SwitchThumb { get; set; }

        #endregion

        #region Events

        public event RoutedEventHandler Toggled;

        #endregion

        #region Header Property

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata(null, OnHeaderChanged));

        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;

            toggleSwitch.UpdateHeaderContentPresenterVisibility();
            toggleSwitch.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderChanged(object oldContent, object newContent) { }

        #endregion

        #region HeaderTemplate Property

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new FrameworkPropertyMetadata(null, OnHeaderTemplateChanged));

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;

            toggleSwitch.UpdateHeaderContentPresenterVisibility();
        }

        #endregion

        #region IsOn Property

        public static DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOnChanged));

        public bool IsOn
        {
            get
            {
                return (bool)GetValue(IsOnProperty);
            }
            set
            {
                SetValue(IsOnProperty, value);
            }
        }

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;

            toggleSwitch.OnToggled();
            toggleSwitch.UpdateVisualStates(true);
        }

        #endregion

        #region OnContent Property

        public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register("OnContent", typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata("On", OnOnContentChanged));

        public object OnContent
        {
            get
            {
                return GetValue(OnContentProperty);
            }
            set
            {
                SetValue(OnContentProperty, value);
            }
        }

        private static void OnOnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;

            toggleSwitch.OnOnContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnOnContentChanged(object oldContent, object newContent) { }

        #endregion

        #region OnContentTemplate Property

        public static readonly DependencyProperty OnContentTemplateProperty = DependencyProperty.Register("OnContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch));

        public DataTemplate OnContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(OnContentTemplateProperty);
            }
            set
            {
                SetValue(OnContentTemplateProperty, value);
            }
        }

        #endregion

        #region OffContent Property

        public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register("OffContent", typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata("Off", OnOffContentChanged));

        public object OffContent
        {
            get
            {
                return GetValue(OffContentProperty);
            }
            set
            {
                SetValue(OffContentProperty, value);
            }
        }

        private static void OnOffContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;

            toggleSwitch.OnOffContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnOffContentChanged(object oldContent, object newContent) { }

        #endregion

        #region OffContentTemplate Property

        public static readonly DependencyProperty OffContentTemplateProperty = DependencyProperty.Register("OffContentTemplate", typeof(DataTemplate), typeof(ToggleSwitch));

        public DataTemplate OffContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(OffContentTemplateProperty);
            }
            set
            {
                SetValue(OffContentTemplateProperty, value);
            }
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleSwitch));

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

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));

            EventManager.RegisterClassHandler(typeof(ToggleSwitch), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
        }

        public ToggleSwitch()
        {
            IsEnabledChanged += OnIsEnabledChanged;
        }

        public override void OnApplyTemplate()
        {
            if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
            {
                SwitchThumb.DragStarted -= OnSwitchThumbDragStarted;
                SwitchThumb.DragDelta -= OnSwitchThumbDragDelta;
                SwitchThumb.DragCompleted -= OnSwitchThumbDragCompleted;
            }

            base.OnApplyTemplate();

            HeaderContentPresenter = GetTemplateChild(nameof(HeaderContentPresenter)) as ContentPresenter;
            SwitchKnobBounds = GetTemplateChild(nameof(SwitchKnobBounds)) as FrameworkElement;
            SwitchKnob = GetTemplateChild(nameof(SwitchKnob)) as FrameworkElement;
            KnobTranslateTransform = GetTemplateChild(nameof(KnobTranslateTransform)) as TranslateTransform;
            SwitchThumb = GetTemplateChild(nameof(SwitchThumb)) as Thumb;

            if (SwitchKnobBounds != null && SwitchKnob != null && KnobTranslateTransform != null && SwitchThumb != null)
            {
                SwitchThumb.DragStarted += OnSwitchThumbDragStarted;
                SwitchThumb.DragDelta += OnSwitchThumbDragDelta;
                SwitchThumb.DragCompleted += OnSwitchThumbDragCompleted;
            }

            UpdateHeaderContentPresenterVisibility();
            UpdateVisualStates(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ToggleSwitchAutomationPeer(this);
        }

        protected virtual void OnToggled()
        {
            Toggled?.Invoke(this, new RoutedEventArgs());
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == IsMouseOverProperty)
            {
                UpdateVisualStates(true);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (SwitchKnobBounds != null && SwitchKnob != null)
            {
                _onTranslation = SwitchKnobBounds.ActualWidth - SwitchKnob.ActualWidth - SwitchKnob.Margin.Left - SwitchKnob.Margin.Right;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                Toggle();
            }

            base.OnKeyUp(e);
        }

        private void UpdateHeaderContentPresenterVisibility()
        {
            if(HeaderContentPresenter != null)
            {
                bool showHeader = (Header is string str && !string.IsNullOrEmpty(str)) || Header != null;
                HeaderContentPresenter.Visibility = showHeader ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            string stateName;

            if (!IsEnabled)
            {
                stateName = CommonStateDisabled;
            }
            else if (IsMouseOver)
            {
                stateName = CommonStateMouseOver;
            }
            else
            {
                stateName = CommonStateNormal;
            }

            VisualStateManager.GoToState(this, stateName, useTransitions);

            if (SwitchThumb != null && SwitchThumb.IsDragging)
            {
                stateName = DraggingState;
            }
            else
            {
                stateName = IsOn ? OnState : OffState;
            }

            VisualStateManager.GoToState(this, stateName, useTransitions);

            VisualStateManager.GoToState(this, IsOn ? OnContentState : OffContentState, useTransitions);
        }

        private void Toggle()
        {
            SetCurrentValue(IsOnProperty, !IsOn);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualStates(true);
        }

        private void OnSwitchThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            e.Handled = true;

            _startTranslation = KnobTranslateTransform.X;
            UpdateVisualStates(true);
            KnobTranslateTransform.X = _startTranslation;
        }

        private void OnSwitchThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            e.Handled = true;

            if(e.HorizontalChange != 0)
            {
                _wasDragged = true;
                double dragTranslation = _startTranslation + e.HorizontalChange;
                KnobTranslateTransform.X = Math.Max(0, Math.Min(_onTranslation, dragTranslation));
            }
        }

        private void OnSwitchThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            e.Handled = true;

            if(_wasDragged)
            {
                double edge = IsOn ? _onTranslation : 0;
                if(KnobTranslateTransform.X != edge)
                {
                    Toggle();
                }
            }
            else
            {
                Toggle();
            }

            _wasDragged = false;
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

            if(!toggleSwitch.IsKeyboardFocusWithin)
            {
                e.Handled = toggleSwitch.Focus() || e.Handled;
            }
        }

        #endregion
    }
}
