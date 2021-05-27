using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class BitmapIcon : IconElement
    {
        private Image m_image;

        #region UriSource Property

        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapIcon), new FrameworkPropertyMetadata(OnUriSourcePropertyChanged));

        public Uri UriSource
        {
            get
            {
                return (Uri)GetValue(UriSourceProperty);
            }
            set
            {
                SetValue(UriSourceProperty, value);
            }
        }

        private static void OnUriSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BitmapIcon bitmapIcon = d as BitmapIcon;

            if (bitmapIcon == null)
                return;

            bitmapIcon.SetImageSourceFromUriSource();
        }

        #endregion

        #region ShowAsMonochrome Property

        public static readonly DependencyProperty ShowAsMonochromeProperty = DependencyProperty.Register("ShowAsMonochrome", typeof(bool), typeof(BitmapIcon), new FrameworkPropertyMetadata(true));

        public bool ShowAsMonochrome
        {
            get
            {
                return (bool)GetValue(ShowAsMonochromeProperty);
            }
            set
            {
                SetValue(ShowAsMonochromeProperty, value);
            }
        }

        #endregion


        #region Methods

        public BitmapIcon()
        {

        }

        protected override void InitializeComponent()
        {
            m_image = new Image();

            SetImageSourceFromUriSource();

            AddChild(m_image);
        }

        private void SetImageSourceFromUriSource()
        {
            if (UriSource != null && m_image != null)
            {
                BitmapImage source = new BitmapImage(UriSource);

                m_image.Source = source;
            }
            else if (UriSource == null)
            {
                m_image.ClearValue(Image.SourceProperty);
            }
        }

        #endregion
    }
}
