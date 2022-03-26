using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using Tesseract;
using Page = Tesseract.Page;

namespace FaceDetectionAndRecognition
{
    /// <summary>
    /// Interaction logic for text_d.xaml
    /// </summary>
    public partial class text_d : Window
    {
        public text_d()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {

                MessageBox.Show("15$ for full code --> Contact me on whatsApp to complete the payment process +201124932549");

            }
            
        }
    }
}
