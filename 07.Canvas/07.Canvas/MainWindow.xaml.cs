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

namespace _07.Canvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "btn1")
            {
                Panel.SetZIndex(btn1, 88);
                Panel.SetZIndex(btn2, 89);
            }
            else if (btn.Name == "btn2")
            {
                Panel.SetZIndex(btn1, 89);
                Panel.SetZIndex(btn2, 88);
            }
            else
            {

            }
        }
    }
}
