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

namespace _15.ListView_GridView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<User> lstUsers;
        List<User> lstUserGroup;
        List<User> lstUserSort;
        public MainWindow()
        {
            InitializeComponent();

            lstUsers = new List<User>() {
                new User() { Name = "minh",  Age = 26, Email = "minh1@gmail.com", Sex = SexType.Male},
                new User() { Name = "hoang",  Age = 27, Email = "hoang1@gmail.com", Sex = SexType.Female },
                new User() { Name = "tony",  Age = 25, Email = "tony1@gmail.com", Sex = SexType.Male},
                new User() { Name = "anh",  Age = 25, Email = "anh@gmail.com", Sex = SexType.Female},

            };
            lstUserGroup = lstUsers.ToList();
            lstUserSort = lstUsers.ToList();

            //
            lstViewUsers.ItemsSource = lstUsers;
            CollectionView viewFilter = (CollectionView)CollectionViewSource.GetDefaultView(lstViewUsers.ItemsSource);
            viewFilter.Filter = UserFilter;

            //
            lstViewUserGroup.ItemsSource = lstUserGroup;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lstViewUserGroup.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Sex"));

            //
            lstViewUsersSort.ItemsSource = lstUserSort;
            
        }

        private bool UserFilter(object obj)
        {
            if (string.IsNullOrEmpty(txtFilter.Text))
            {
                return true;
            }
            else
            {
                return (obj as User).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        static bool isSorted = false;
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader grid = sender as GridViewColumnHeader;
            CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(lstViewUsersSort.ItemsSource);

            view1.SortDescriptions.Clear();
            view1.SortDescriptions.Add(new System.ComponentModel.SortDescription(grid.Content.ToString(), isSorted ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending));
            isSorted = !isSorted;
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lstViewUsers.ItemsSource).Refresh();
        }
    }

    public enum SexType
    {
        Male, 
        Female
    }

    public class User
    {
        public string Name { get; set; }
        public int  Age { get; set; }
        public string Email { get; set; }
        public SexType Sex { get; set; }
    }
}
