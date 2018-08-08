using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace _19.ValueConverter
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
    }

    public class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? selectedDate = value as DateTime?;

            if (selectedDate == null)
            {
                return null;
            }

            return selectedDate.Value.Year;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int year = (int)value;
            return new DateTime(year, 1, 1);
        }
    }
}
