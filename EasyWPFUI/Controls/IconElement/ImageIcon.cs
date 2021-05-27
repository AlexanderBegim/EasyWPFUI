using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class ImageIcon : IconElement
    {
        private Image m_rootImage;

        #region ImageSource Property

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageIcon), new FrameworkPropertyMetadata());

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        #endregion

        #region Methods

        public ImageIcon()
        {

        }

        protected override void InitializeComponent()
        {
            m_rootImage = new Image();

            m_rootImage.SetBinding(Image.SourceProperty, new Binding() { Source = this, Path = new PropertyPath(SourceProperty) });

            AddChild(m_rootImage);
        }

        #endregion
    }
}
