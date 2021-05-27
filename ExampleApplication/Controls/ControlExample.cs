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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using EasyWPFUI;
using ColorCode;
using ColorCode.Common;

namespace ExampleApplication.Controls
{
    /// <summary>
    /// Describes a textual substitution in sample content.
    /// If enabled (default), then $(Key) is replaced with the stringified value.
    /// If disabled, then $(Key) is replaced with the empty string.
    /// </summary>
    public sealed class ControlExampleSubstitution : DependencyObject
    {
        public event TypedEventHandler<ControlExampleSubstitution, object> ValueChanged;

        public string Key { get; set; }

        private object _value = null;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        public string ValueAsString()
        {
            if (!IsEnabled)
            {
                return string.Empty;
            }

            object value = Value;

            // For solid color brushes, use the underlying color.
            if (value is SolidColorBrush)
            {
                value = ((SolidColorBrush)value).Color;
            }

            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }

    [ContentProperty("Example")]
    public class ControlExample : Control
    {
        private ContentPresenter m_xamlPresenter;
        private ContentPresenter m_csharpPresenter;

        private enum SyntaxHighlightLanguage { Xml, CSharp };
        private static Regex SubstitutionPattern = new Regex(@"\$\(([^\)]+)\)");

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register("Example", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Example
        {
            get { return GetValue(ExampleProperty); }
            set { SetValue(ExampleProperty, value); }
        }

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Output
        {
            get { return GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Options
        {
            get { return GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty XamlProperty = DependencyProperty.Register("Xaml", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string Xaml
        {
            get { return (string)GetValue(XamlProperty); }
            set { SetValue(XamlProperty, value); }
        }

        public static readonly DependencyProperty XamlSourceProperty = DependencyProperty.Register("XamlSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public Uri XamlSource
        {
            get { return (Uri)GetValue(XamlSourceProperty); }
            set { SetValue(XamlSourceProperty, value); }
        }

        public static readonly DependencyProperty CSharpProperty = DependencyProperty.Register("CSharp", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string CSharp
        {
            get { return (string)GetValue(CSharpProperty); }
            set { SetValue(CSharpProperty, value); }
        }

        public static readonly DependencyProperty CSharpSourceProperty = DependencyProperty.Register("CSharpSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public Uri CSharpSource
        {
            get { return (Uri)GetValue(CSharpSourceProperty); }
            set { SetValue(CSharpSourceProperty, value); }
        }

        public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList), typeof(ControlExample), new PropertyMetadata(null));
        public IList Substitutions
        {
            get { return (IList)GetValue(SubstitutionsProperty); }
            set { SetValue(SubstitutionsProperty, value); }
        }

        public static readonly DependencyProperty ExampleHeightProperty = DependencyProperty.Register("ExampleHeight", typeof(GridLength), typeof(ControlExample), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));
        public GridLength ExampleHeight
        {
            get { return (GridLength)GetValue(ExampleHeightProperty); }
            set { SetValue(ExampleHeightProperty, value); }
        }

        public new static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ControlExample), new PropertyMetadata(HorizontalAlignment.Left));
        public new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }


        static ControlExample()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlExample), new FrameworkPropertyMetadata(typeof(ControlExample)));
        }

        public ControlExample()
        {
            Substitutions = new List<ControlExampleSubstitution>();

            ThemeManager.ApplicationThemeChanged += OnThemeManagerApplicationThemeChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("RootGrid") is Grid rootGrid)
            {
                rootGrid.Loaded += OnRootGridLoaded;
            }

            if (GetTemplateChild("XamlPresenter") is ContentPresenter xamlPresenter)
            {
                m_xamlPresenter = xamlPresenter;

                xamlPresenter.Loaded += OnXamlPresenterLoaded;
            }

            if (GetTemplateChild("CSharpPresenter") is ContentPresenter csharpPresenter)
            {
                m_csharpPresenter = csharpPresenter;

                csharpPresenter.Loaded += OnCsharpPresenterLoaded;
            }
        }

        private void OnRootGridLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var substitution in Substitutions)
            {
                if(substitution is ControlExampleSubstitution item)
                {
                    item.ValueChanged += OnValueChanged;
                }
            }
        }

        private void OnValueChanged(ControlExampleSubstitution sender, object e)
        {
            GenerateAllSyntaxHighlightedContent();
        }

        private void OnXamlPresenterLoaded(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent(sender as ContentPresenter, Xaml as string, XamlSource, Languages.Xml);
        }

        private void OnCsharpPresenterLoaded(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent(sender as ContentPresenter, CSharp, CSharpSource, Languages.CSharp);
        }

        private void OnThemeManagerApplicationThemeChanged(object sender, ApplicationThemeChangedEventArgs e)
        {
            // If the theme has changed after the user has already opened the app (ie. via settings), then the new locally set theme will overwrite the colors that are set during Loaded.
            // Therefore we need to re-format the REB to use the correct colors.

            GenerateAllSyntaxHighlightedContent();
        }


        private void GenerateAllSyntaxHighlightedContent()
        {
            GenerateSyntaxHighlightedContent(m_xamlPresenter, Xaml as string, XamlSource, Languages.Xml);
            GenerateSyntaxHighlightedContent(m_csharpPresenter, CSharp, CSharpSource, Languages.CSharp);
        }

        private void GenerateSyntaxHighlightedContent(ContentPresenter presenter, string sampleString, Uri sampleUri, ILanguage highlightLanguage)
        {
            if (!string.IsNullOrEmpty(sampleString))
            {
                FormatAndRenderSampleFromString(sampleString, presenter, highlightLanguage);
            }
            else
            {
                FormatAndRenderSampleFromFile(sampleUri, presenter, highlightLanguage);
            }
        }

        private async void FormatAndRenderSampleFromFile(Uri source, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            if (source != null && source.OriginalString.EndsWith(".txt"))
            {

                Uri sampleUri = new Uri("pack://application:,,,/ExampleApplication;component/Data/SampleCode/" + source.OriginalString);

                StreamResourceInfo resource = Application.GetResourceStream(sampleUri);

                string sampleString = await new StreamReader(resource.Stream).ReadToEndAsync();

                FormatAndRenderSampleFromString(sampleString, presenter, highlightLanguage);
            }
            else
            {
                presenter.Visibility = Visibility.Collapsed;
            }
        }

        private void FormatAndRenderSampleFromString(string sampleString, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            // Trim out stray blank lines at start and end.
            sampleString = sampleString.TrimStart('\n').TrimEnd();

            // Also trim out spaces at the end of each line
            sampleString = string.Join("\n", sampleString.Split('\n').Select(s => s.TrimEnd()));

            // Perform any applicable substitutions.
            sampleString = SubstitutionPattern.Replace(sampleString, match =>
            {
                foreach (var substitution in Substitutions)
                {
                    if (substitution is ControlExampleSubstitution item && item.Key == match.Groups[1].Value)
                    {
                        return item.ValueAsString();
                    }
                }
                throw new KeyNotFoundException(match.Groups[1].Value);
            });

            //var sampleCodeRTB = new RichTextBox { FontFamily = new FontFamily("Consolas") };
            //sampleCodeRTB.Background = Brushes.Transparent;
            //sampleCodeRTB.IsReadOnly = true;
            //sampleCodeRTB.BorderThickness = new Thickness(0);

            //var formatter = GenerateRichTextFormatter();
            //formatter.FormatRichTextBlock(sampleString, highlightLanguage, sampleCodeRTB);
            //presenter.Content = sampleCodeRTB;

            FlowDocumentScrollViewer sampleCodeDocument = new FlowDocumentScrollViewer()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Document = new FlowDocument(new Paragraph()
                {
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 14,
                    KeepWithNext = false,
                    TextAlignment = TextAlignment.Left

                })
            };

            RichTextBlockFormatter formatter = GenerateRichTextFormatter();
            formatter.FormatInlines(sampleString, highlightLanguage, (sampleCodeDocument.Document.Blocks.FirstBlock as Paragraph).Inlines);

            presenter.Content = sampleCodeDocument;
        }

        private RichTextBlockFormatter GenerateRichTextFormatter()
        {
            var formatter = new RichTextBlockFormatter(ThemeManager.ApplicationTheme);

            if (ThemeManager.ApplicationTheme == ApplicationTheme.Dark)
            {
                UpdateFormatterDarkThemeColors(formatter);
            }

            return formatter;
        }

        private void UpdateFormatterDarkThemeColors(RichTextBlockFormatter formatter)
        {
            // Replace the default dark theme resources with ones that more closely align to VS Code dark theme.
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttribute]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeQuotes]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeValue]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.HtmlComment]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlDelimiter]);
            formatter.Styles.Remove(formatter.Styles[ScopeName.XmlName]);

            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
            {
                Foreground = "#FF87CEFA",
                ReferenceName = "xmlAttribute"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
            {
                Foreground = "#FFFFA07A",
                ReferenceName = "xmlAttributeQuotes"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
            {
                Foreground = "#FFFFA07A",
                ReferenceName = "xmlAttributeValue"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
            {
                Foreground = "#FF6B8E23",
                ReferenceName = "htmlComment"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
            {
                Foreground = "#FF808080",
                ReferenceName = "xmlDelimiter"
            });
            formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
            {
                Foreground = "#FF5F82E8",
                ReferenceName = "xmlName"
            });
        }
    }
}
