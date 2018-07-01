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

namespace _08.Border
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
            // get vertical, horizontal offset value
            MessageBox.Show(scvBtn.VerticalOffset.ToString() + " " + scvBtn.HorizontalOffset);

            // get view port height, width
            MessageBox.Show(scvBtn.ViewportHeight.ToString() + " " + scvBtn.ViewportWidth);

            // get scrollable height and width
            MessageBox.Show(scvBtn.ScrollableHeight.ToString() + " " + scvBtn.ScrollableWidth);
            
            // scroll to end
            scvBtn.ScrollToEnd();
        }
    }
}
