using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FaceDetectionAndRecognition.controls
{
    public class camera_op_con : Control
    {
        public Visibility Isactive
        {
            get { return (Visibility)GetValue(IsactiveProperty); }
            set { SetValue(IsactiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Isactive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsactiveProperty =
            DependencyProperty.Register("Isactive", typeof(Visibility), typeof(camera_op_con));



        public string Isactive_icon
        {
            get { return (string)GetValue(Isactive_iconProperty); }
            set { SetValue(Isactive_iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Isactive_icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Isactive_iconProperty =
            DependencyProperty.Register("Isactive_icon", typeof(string), typeof(camera_op_con));



        public double Isactiv_size
        {
            get { return (double)GetValue(Isactiv_sizeProperty); }
            set { SetValue(Isactiv_sizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Isactiv_size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Isactiv_sizeProperty =
            DependencyProperty.Register("Isactiv_size", typeof(double), typeof(camera_op_con));




        public Brush Isactiv_color
        {
            get { return (Brush)GetValue(Isactiv_colorProperty); }
            set { SetValue(Isactiv_colorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Isactiv_color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Isactiv_colorProperty =
            DependencyProperty.Register("Isactiv_color", typeof(Brush), typeof(camera_op_con));



        public string icon
        {
            get { return (string)GetValue(iconProperty); }
            set { SetValue(iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iconProperty =
            DependencyProperty.Register("icon", typeof(string), typeof(camera_op_con));

        public double icon_size
        {
            get { return (double)GetValue(icon_sizeProperty); }
            set { SetValue(icon_sizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon_size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty icon_sizeProperty =
            DependencyProperty.Register("icon_size", typeof(double), typeof(camera_op_con));


        public Thickness margin_icon
        {
            get { return (Thickness)GetValue(margin_iconProperty); }
            set { SetValue(margin_iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for margin_icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty margin_iconProperty =
            DependencyProperty.Register("margin_icon", typeof(Thickness), typeof(camera_op_con));





        public Brush icon_color
        {
            get { return (Brush)GetValue(icon_colorProperty); }
            set { SetValue(icon_colorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon_color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty icon_colorProperty =
            DependencyProperty.Register("icon_color", typeof(Brush), typeof(camera_op_con));







    }
}
