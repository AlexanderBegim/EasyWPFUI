//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyWPFUI.Controls;
using System.Windows.Automation;

namespace ExampleApplication.ControlPages
{
    /// <summary>
    /// Логика взаимодействия для ButtonPage.xaml
    /// </summary>
    public partial class ButtonPage : Page
    {
        private TextMarkerStyle markerStyle = TextMarkerStyle.None;
        private List documentList;

        public ButtonPage()
        {
            InitializeComponent();

            splitButtonExample1RichTextBox.Document.Blocks.Clear();
            splitButtonExample1RichTextBox.Document.Blocks.Add(new Paragraph(new Run("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempor commodo ullamcorper a lacus.")));

            toggleSplitButtonExample1RichTextBox.Document.Blocks.Clear();
            InitToggleSplitButtonExampleRichTextDocument();
        }

        private void defaultButtonExample1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            defaultButtonExample1Substitution.IsEnabled = !defaultButtonExample1.IsEnabled;
        }

        private int repeatButtonExample1Clicks = 0;
        private void repeatButtonExample1_Click(object sender, RoutedEventArgs e)
        {
            repeatButtonExample1Clicks++;

            repeatButtonExample1Output.Text = "Number of clicks: " + repeatButtonExample1Clicks;
        }

        private void repeatButtonExample1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            repeatButtonExample1Substitution.IsEnabled = !repeatButtonExample1.IsEnabled;
        }


        private void ToggleButtonExample1_Checked(object sender, RoutedEventArgs e)
        {
            toggleButtonExample1Output.Text = "On";
        }

        private void ToggleButtonExample1_Unchecked(object sender, RoutedEventArgs e)
        {
            toggleButtonExample1Output.Text = "Off";
        }

        private void ToggleButtonExample1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            toggleButtonExample1Substitution.IsEnabled = !toggleButtonExample1.IsEnabled;
        }


        private void splitButtonExample1_Click(EasyWPFUI.Controls.SplitButton sender, EasyWPFUI.Controls.SplitButtonClickEventArgs args)
        {
            Rectangle rectangle = (Rectangle)sender.Content;

            Brush brush = rectangle.Fill;

            TextRange selection = new TextRange(splitButtonExample1RichTextBox.Selection.Start, splitButtonExample1RichTextBox.Selection.End);
            selection.ApplyPropertyValue(ForegroundProperty, brush);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedColor = (Button)sender;

            Rectangle rectangle = (Rectangle)clickedColor.Content;

            Brush brush = rectangle.Fill;

            splitButtonExample1.Flyout.Hide();

            CurrentColor.Fill = brush;

            if(splitButtonExample1RichTextBox.Selection.IsEmpty)
            {
                splitButtonExample1RichTextBox.Focus();

                TextPointer start = splitButtonExample1RichTextBox.Document.ContentStart;
                TextPointer end = splitButtonExample1RichTextBox.Document.ContentEnd;

                splitButtonExample1RichTextBox.Selection.Select(start, end);
            }
        }


        private void InitToggleSplitButtonExampleRichTextDocument()
        {
            documentList = new List(new ListItem(new Paragraph()));
            documentList.MarkerStyle = markerStyle;
            toggleSplitButtonExample1RichTextBox.Document.Blocks.Add(documentList);
        }

        private void toggleSplitButtonExample1_IsCheckedChanged(EasyWPFUI.Controls.ToggleSplitButton sender, EasyWPFUI.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if(toggleSplitButtonExample1.IsChecked)
            {
                documentList.MarkerStyle = markerStyle;
            }
            else
            {
                documentList.MarkerStyle = TextMarkerStyle.None;
            }
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            Button ciickedBullet = (Button)sender;
            SymbolIcon symbol = (SymbolIcon)ciickedBullet.Content;

            if(symbol.Symbol == Symbol.List)
            {
                markerStyle = TextMarkerStyle.Disc;

                toggleSplitButtonExample1SymbolIcon.Symbol = Symbol.List;
                toggleSplitButtonExample1SymbolIcon.SetValue(AutomationProperties.NameProperty, "Bullets");
            }
            else if (symbol.Symbol == Symbol.Bullets)
            {
                markerStyle = TextMarkerStyle.UpperRoman;

                toggleSplitButtonExample1SymbolIcon.Symbol = Symbol.Bullets;
                toggleSplitButtonExample1SymbolIcon.SetValue(AutomationProperties.NameProperty, "Roman Numerals");
            }

            if(!(toggleSplitButtonExample1RichTextBox.Document.Blocks.LastBlock is List))
            {
                InitToggleSplitButtonExampleRichTextDocument();
            }

            documentList.MarkerStyle = markerStyle;

            toggleSplitButtonExample1.IsChecked = true;
            toggleSplitButtonExample1.Flyout.Hide();

            if (toggleSplitButtonExample1RichTextBox.Selection.IsEmpty)
            {
                toggleSplitButtonExample1RichTextBox.Focus();

                TextPointer start = toggleSplitButtonExample1RichTextBox.Document.ContentStart;
                TextPointer end = toggleSplitButtonExample1RichTextBox.Document.ContentEnd;

                toggleSplitButtonExample1RichTextBox.Selection.Select(start, end);
            }
        }
    }
}
