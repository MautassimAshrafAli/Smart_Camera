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
    public class video_stop_con : Control
    {




        public string icon
        {
            get { return (string)GetValue(iconProperty); }
            set { SetValue(iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iconProperty =
            DependencyProperty.Register("icon", typeof(string), typeof(video_stop_con));



        public double icon_size
        {
            get { return (double)GetValue(icon_sizeProperty); }
            set { SetValue(icon_sizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for icon_size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty icon_sizeProperty =
            DependencyProperty.Register("icon_size", typeof(double), typeof(video_stop_con));




        public Thickness margin_icon
        {
            get { return (Thickness)GetValue(margin_iconProperty); }
            set { SetValue(margin_iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for margin_icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty margin_iconProperty =
            DependencyProperty.Register("margin_icon", typeof(Thickness), typeof(video_stop_con));



        public Brush icon_color
        {
            get { return (Brush)GetValue(iocn_colorProperty); }
            set { SetValue(iocn_colorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for iocn_color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iocn_colorProperty =
            DependencyProperty.Register("iocn_color", typeof(Brush), typeof(video_stop_con));



        public string time
        {
            get { return (string)GetValue(timeProperty); }
            set { SetValue(timeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty timeProperty =
            DependencyProperty.Register("time", typeof(string), typeof(video_stop_con));



        public string time_size
        {
            get { return (string)GetValue(time_sizeProperty); }
            set { SetValue(time_sizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for time_size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty time_sizeProperty =
            DependencyProperty.Register("time_size", typeof(string), typeof(video_stop_con));



        public Thickness margin_time
        {
            get { return (Thickness)GetValue(margin_timeProperty); }
            set { SetValue(margin_timeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for margin_time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty margin_timeProperty =
            DependencyProperty.Register("margin_time", typeof(Thickness), typeof(video_stop_con));



        public Brush time_color
        {
            get { return (Brush)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(Brush), typeof(video_stop_con));



        public Visibility btn_visibility
        {
            get { return (Visibility)GetValue(btn_visibilityProperty); }
            set { SetValue(btn_visibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for btn_visibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty btn_visibilityProperty =
            DependencyProperty.Register("btn_visibility", typeof(Visibility), typeof(video_stop_con));




        public double btn_w
        {
            get { return (double)GetValue(btn_wProperty); }
            set { SetValue(btn_wProperty, value); }
        }

        // Using a DependencyProperty as the backing store for btn_w.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty btn_wProperty =
            DependencyProperty.Register("btn_w", typeof(double), typeof(video_stop_con));



        public double btn_h
        {
            get { return (double)GetValue(btn_hProperty); }
            set { SetValue(btn_hProperty, value); }
        }

        // Using a DependencyProperty as the backing store for btn_h.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty btn_hProperty =
            DependencyProperty.Register("btn_h", typeof(double), typeof(video_stop_con));







    }
}
