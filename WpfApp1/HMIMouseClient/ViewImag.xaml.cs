using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace HMIMouseClient
{
    /// <summary>
    /// Interaction logic for ViewImag.xaml
    /// </summary>
    public partial class ViewImag : Window
    {

        private static Bitmap screenBitmap;
        private static Graphics screenGraphics;

        public ViewImag()
        {
            InitializeComponent();

            screenBitmap = new Bitmap(1920, 1080, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            screenGraphics = Graphics.FromImage(screenBitmap);
            screenGraphics.CopyFromScreen(0, 0,
                        0, 0, screenBitmap.Size, CopyPixelOperation.SourceCopy);

            screenBitmap.Save("abc.png", ImageFormat.Png);

            img.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\abc.png"));

            line1.Visibility = Visibility.Visible;
            line2.Visibility = Visibility.Visible;


        }

        private void img_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void Cnv_MouseMove(object sender, MouseEventArgs e)
        {
            //Ellipse ellipse = new Ellipse();
            //// ellipse.Fill = System.Drawing.Brushes.Sienna;
            //ellipse.Width = 100;
            //ellipse.Height = 100;
            //ellipse.StrokeThickness = 2;

            //Cnv.Children.Add(ellipse);

            //Canvas.SetLeft(ellipse, e.GetPosition(img).X);
            //Canvas.SetTop(ellipse, e.GetPosition(img).Y);

            //Console.WriteLine(e.GetPosition(img).X + " : " + e.GetPosition(img).Y);
        }

        private void Cnv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Cnv_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void img_MouseMove(object sender, MouseEventArgs e)
        {
           

            double x = e.GetPosition(img).X, y = e.GetPosition(img).Y;
            double scrWidth = 1920, scrHeight = 1080;

            line1.X1 = 0;
            line1.Y1 = y;
            line1.X2 = scrWidth;
            line1.Y2 = y;

            line2.X1 = x;
            line2.Y1 = 0;
            line2.X2 = x;
            line2.Y2 = scrHeight;

            txtPixel.Text = string.Format("{0} x {1}", x, y);


            Cnv.Margin = new Thickness(x, y, scrWidth - x, scrHeight - y);

            CnvRec.Margin = new Thickness(x, y, scrWidth - x, scrHeight - y);


            Console.WriteLine("Cnv_MouseDown - " + e.GetPosition(img).X + " : " + e.GetPosition(img).Y);

            //System.Drawing.Rectangle rec = new System.Drawing.Rectangle();
            //rec.Width = 300;
            //rec.Height = 300;

            

        }
    }
}
