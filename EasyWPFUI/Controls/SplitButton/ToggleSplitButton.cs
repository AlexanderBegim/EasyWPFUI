using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using EasyWPFUI.Automation.Peers;

namespace EasyWPFUI.Controls
{
    public class ToggleSplitButton : SplitButton
    {
        #region Events

        public event TypedEventHandler<ToggleSplitButton, ToggleSplitButtonIsCheckedChangedEventArgs> IsCheckedChanged;

        #endregion

        #region IsChecked Property

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleSplitButton), new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        public bool IsChecked
        {
            get
            {
                return (bool)GetValue(IsCheckedProperty);
            }
            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSplitButton toggleSplitButton = d as ToggleSplitButton;

            if (toggleSplitButton == null)
                return;

            toggleSplitButton.OnIsCheckedChanged();
        }

        #endregion

        #region Methods

        static ToggleSplitButton()
        {

        }

        public ToggleSplitButton()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ToggleSplitButtonAutomationPeer(this);
        }

        internal override void OnClickPrimary(object sender, RoutedEventArgs e)
        {
            Toggle();

            base.OnClickPrimary(sender, e);
        }

        internal override bool InternalIsChecked()
        {
            return IsChecked;
        }

        internal void Toggle()
        {
            IsChecked = !IsChecked;
        }

        private void OnIsCheckedChanged()
        {
            if(m_hasLoaded)
            {
                ToggleSplitButtonIsCheckedChangedEventArgs eventArgs = new ToggleSplitButtonIsCheckedChangedEventArgs();
                IsCheckedChanged?.Invoke(this, eventArgs);

                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);

                if(peer != null)
                {
                    ToggleState newValue = IsChecked ? ToggleState.On : ToggleState.Off;
                    ToggleState oldValue = (newValue == ToggleState.On) ? ToggleState.Off : ToggleState.On;

                    peer.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldValue, newValue);
                }
            }

            UpdateVisualStates();
        }

        #endregion
    }
}
