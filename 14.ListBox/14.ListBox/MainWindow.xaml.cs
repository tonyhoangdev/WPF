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

namespace _14.ListBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> listData;
        public MainWindow()
        {
            InitializeComponent();

            listData = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                listData.Add(i.ToString());
            }

            cb1.ItemsSource = listData;
            lstb1.ItemsSource = listData;
            cb2.ItemsSource = listData;
        }
    }
}
