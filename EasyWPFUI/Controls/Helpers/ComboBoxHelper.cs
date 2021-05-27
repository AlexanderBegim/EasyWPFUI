using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using EasyWPFUI.Converters;

namespace EasyWPFUI.Controls.Helpers
{
    public class ComboBoxHelper
    {
        private const string c_popupBorderName = "PopupBorder";
        private const string c_editableTextName = "PART_EditableTextBox";
        private const string c_editableTextBorderName = "BorderElement";
        private const string c_controlCornerRadiusKey = "ControlCornerRadius";
        private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

        #region KeepInteriorCornersSquare Property

        public static readonly DependencyProperty KeepInteriorCornersSquareProperty = DependencyProperty.RegisterAttached("KeepInteriorCornersSquare", typeof(bool), typeof(ComboBoxHelper), new FrameworkPropertyMetadata(OnKeepInteriorCornersSquarePropertyChanged));

        public static bool GetKeepInteriorCornersSquare(UIElement ui)
        {
            return (bool)ui.GetValue(KeepInteriorCornersSquareProperty);
        }

        public static void SetKeepInteriorCornersSquare(UIElement ui, bool value)
        {
            ui.SetValue(KeepInteriorCornersSquareProperty, value);
        }

        private static void OnKeepInteriorCornersSquarePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ComboBox comboBox)
            {
                bool shouldMonitorDropDownState = (bool)e.NewValue;

                if(shouldMonitorDropDownState)
                {
                    comboBox.DropDownOpened += OnDropDownOpened;
                    comboBox.DropDownClosed += OnDropDownClosed;
                }
                else
                {
                    comboBox.DropDownOpened -= OnDropDownOpened;
                    comboBox.DropDownClosed -= OnDropDownClosed;
                }
            }
        }

        #endregion

        #region Methods

        private static void OnDropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            UpdateCornerRadius(comboBox, /*IsDropDownOpen=*/false);
        }

        private static void OnDropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                UpdateCornerRadius(comboBox, /*IsDropDownOpen=*/true);
            }));
        }

        private static void UpdateCornerRadius(ComboBox comboBox, bool isDropDownOpen)
        {
            if (comboBox.IsEditable)
            {
                CornerRadius textBoxRadius = (CornerRadius)(comboBox.GetValue(ControlHelper.CornerRadiusProperty) ?? Application.Current.Resources[c_controlCornerRadiusKey]);
                CornerRadius popupRadius = (CornerRadius)Application.Current.Resources[c_overlayCornerRadiusKey];

                if (isDropDownOpen)
                {
                    bool isOpenDown = IsPopupOpenDown(comboBox);
                    CornerRadiusFilterConverter cornerRadiusConverter = new CornerRadiusFilterConverter();

                    CornerRadiusFilterKind popupRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Bottom : CornerRadiusFilterKind.Top;
                    popupRadius = cornerRadiusConverter.Convert(popupRadius, popupRadiusFilter);

                    CornerRadiusFilterKind textBoxRadiusFilter = isOpenDown ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom;
                    textBoxRadius = cornerRadiusConverter.Convert(textBoxRadius, textBoxRadiusFilter);
                }

                if (comboBox.Template?.FindName(c_popupBorderName, comboBox) is Border popupBorder)
                {
                    popupBorder.CornerRadius = popupRadius;
                }

                if (comboBox.Template?.FindName(c_editableTextName, comboBox) is TextBox textBox)
                {
                    textBox.SetValue(ControlHelper.CornerRadiusProperty, textBoxRadius);
                }
            }
        }

        private static bool IsPopupOpenDown(ComboBox comboBox)
        {
            double verticalOffset = 0;
            if (comboBox.Template?.FindName(c_popupBorderName, comboBox) is Border popupBorder)
            {
                if (comboBox.Template.FindName(c_editableTextName, comboBox) is TextBox textBox)
                {
                    // GeneralTransform transform = popupBorder.TransformToVisual(textBox);
                    Point popupTop = popupBorder.TranslatePoint(new Point(0, 0), textBox);
                    verticalOffset = popupTop.Y;
                }
            }

            return verticalOffset > 0;
        }

        #endregion
    }
}
