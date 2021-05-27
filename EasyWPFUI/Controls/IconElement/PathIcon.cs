using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EasyWPFUI.Controls
{
    public class PathIcon : IconElement
    {
        private Path path;

        #region Data Property

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(BitmapIconSource), new FrameworkPropertyMetadata());

        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        #endregion

        #region Methods

        public PathIcon()
        {

        }

        protected override void InitializeComponent()
        {
            path = new Path();
            path.Stretch = Stretch.None;

            path.SetBinding(Path.FillProperty, new Binding() { Source = this, Path = new PropertyPath(ForegroundProperty) });
            path.SetBinding(Path.DataProperty, new Binding() { Source = this, Path = new PropertyPath(DataProperty) });

            AddChild(path);
        }

        #endregion
    }
}
