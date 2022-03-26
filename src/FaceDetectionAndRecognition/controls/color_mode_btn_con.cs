using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace FaceDetectionAndRecognition.controls
{
    public class color_mode_btn_con : Control
    {



        public Visibility Isactiv
        {
            get { return (Visibility)GetValue(IsactivProperty); }
            set { SetValue(IsactivProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Isactiv.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsactivProperty =
            DependencyProperty.Register("Isactiv", typeof(Visibility), typeof(color_mode_btn_con));



        public string icon
        {
            get { return (string)GetValue(iconProperty); }
            set { SetValue(iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iconProperty =
            DependencyProperty.Register("icon", typeof(string), typeof(color_mode_btn_con));

        public double icon_size
        {
            get { return (double)GetValue(icon_sizeProperty); }
            set { SetValue(icon_sizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon_size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty icon_sizeProperty =
            DependencyProperty.Register("icon_size", typeof(double), typeof(color_mode_btn_con));



        public Brush icon_color
        {
            get { return (Brush)GetValue(icon_colorProperty); }
            set { SetValue(icon_colorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon_color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty icon_colorProperty =
            DependencyProperty.Register("icon_color", typeof(Brush), typeof(color_mode_btn_con));







    }
}
