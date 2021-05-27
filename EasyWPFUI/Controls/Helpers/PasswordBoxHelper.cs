using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls.Helpers
{
    public class PasswordBoxHelper
    {
        #region HelperProperty

        private static readonly DependencyProperty HelperProperty = DependencyProperty.RegisterAttached("PasswordBoxHelper", typeof(PasswordBoxHelper), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(null));

        #endregion

        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

        public static bool GetIsHelperEnabled(UIElement ui)
        {
            return (bool)ui.GetValue(IsHelperEnabledProperty);
        }

        public static void SetIsHelperEnabled(UIElement ui, bool value)
        {
            ui.SetValue(IsHelperEnabledProperty, value);
        }

        private static void OnIsHelperEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = d as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.NewValue)
            {
                PasswordBoxHelper helper = new PasswordBoxHelper(passwordBox);
                helper.Attach();
                passwordBox.SetValue(HelperProperty, helper);
            }
            else
            {
                PasswordBoxHelper helper = passwordBox.GetValue(HelperProperty) as PasswordBoxHelper;

                if (helper == null)
                    return;

                helper.Detach();
                passwordBox.ClearValue(HelperProperty);
            }
        }

        #endregion

        #region HasPassword Property

        public static readonly DependencyProperty HasPasswordProperty = DependencyProperty.RegisterAttached("HasPassword", typeof(bool), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(false));

        public static bool GetHasPassword(UIElement ui)
        {
            return (bool)ui.GetValue(HasPasswordProperty);
        }

        public static void SetHasPassword(UIElement ui, bool value)
        {
            ui.SetValue(HasPasswordProperty, value);
        }

        #endregion

        #region PasswordRevelMode

        public static readonly DependencyProperty PasswordRevealModeProperty = DependencyProperty.RegisterAttached("PasswordRevealMode", typeof(PasswordRevealMode), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(PasswordRevealMode.Peek));

        public static PasswordRevealMode GetPasswordRevealMode(UIElement ui)
        {
            return (PasswordRevealMode)ui.GetValue(PasswordRevealModeProperty);
        }

        public static void SetPasswordRevealMode(UIElement ui, PasswordRevealMode value)
        {
            ui.SetValue(PasswordRevealModeProperty, value);
        }

        #endregion

        #region Fields

        private PasswordBox passwordBox;
        private TextBox textBox;

        #endregion

        #region Methods

        public PasswordBoxHelper(PasswordBox passwordBox)
        {
            this.passwordBox = passwordBox;
        }

        public void Attach()
        {
            passwordBox.PasswordChanged += OnPasswordChanged;
            passwordBox.GotFocus += OnPasswordGotFocus;

            if(passwordBox.IsLoaded)
            {
                InitializeTextBox();
            }
            else
            {
                passwordBox.Loaded += OnLoaded;
            }
        }

        private void OnPasswordGotFocus(object sender, RoutedEventArgs e)
        {

        }

        public void Detach()
        {
            passwordBox.PasswordChanged -= OnPasswordChanged;

            if(textBox != null)
            {
                textBox.TextChanged -= OnTextBoxTextChanged;
            }
        }

        private void InitializeTextBox()
        {
            textBox = passwordBox.Template?.FindName("PasswordTextBox", passwordBox) as TextBox;

            if (textBox == null)
                return;

            textBox.TextChanged += OnTextBoxTextChanged;
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if(GetPasswordRevealMode(passwordBox) == PasswordRevealMode.Visible)
            {
                passwordBox.Password = textBox.Text;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeTextBox();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            textBox.Text = passwordBox.Password;

            if(string.IsNullOrEmpty(passwordBox.Password))
            {
                SetHasPassword(passwordBox, false);
            }
            else
            {
                SetHasPassword(passwordBox, true);
            }
        }

        #endregion
    }
}
