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

namespace _13.ComboBox
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

        List<Food> lstName;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstName = new List<Food>() {
                new Food() {Name = "Cang cu ki", Price="290.000"},
                new Food() {Name = "Cua gach HT", Price="490.000"},
                new Food() { Name = "Cua thit HT", Price="470.000"}
            };
            cbItemSource.ItemsSource = lstName;
            cbItemSource.DisplayMemberPath = "Name";
            cbItemSource.SelectedValuePath = "Price";

            cbItemSource.SelectionChanged += CbItemSource_SelectionChanged;

            cbItemTemplate.ItemsSource = lstName;

            cbColors.ItemsSource = typeof(Colors).GetProperties();
        }

        private void CbItemSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("Name: " + (cbItemSource.SelectedItem as Food).Name + "\nPrice: " + cbItemSource.SelectedValue.ToString(), "Info");
        }
    }

    public class Food
    {
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
