using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace _16.TreeView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MenuItem root = new MenuItem() { Title = "Menu" };
            MenuItem childItem1 = new MenuItem() { Title = "Child item #1" };
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.1" });
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.2" });
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() { Title = "Child item #2" });
            trvBingding.Items.Add(root);

            List<Family> families = new List<Family>();

            Family family1 = new Family() { Name = "Tony" };
            family1.Members.Add(new FamilyMember() { Name = "Tony1", Age = 25 });
            family1.Members.Add(new FamilyMember() { Name = "Tony2", Age = 15 });
            family1.Members.Add(new FamilyMember() { Name = "Tony3", Age = 20 });
            families.Add(family1);

            Family family2 = new Family() { Name = "Hoang" };
            family2.Members.Add(new FamilyMember() { Name = "Hoang1", Age = 5 });
            family2.Members.Add(new FamilyMember() { Name = "Hoang2", Age = 14 });
            family2.Members.Add(new FamilyMember() { Name = "Hoang3", Age = 24 });
            family2.Members.Add(new FamilyMember() { Name = "Hoang4", Age = 18 });

            families.Add(family2);
            trvFamilies.ItemsSource = families;

            // lazy loading
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo item in drives)
            {
                trvLazyLoading.Items.Add(CreateTreeItem(item));
            }
        }

        private TreeViewItem CreateTreeItem(object obj)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = obj.ToString();
            item.Tag = obj;
            item.Items.Add("Loading");
            return item;
        }

        private void trvLazyLoading_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if ((item.Items.Count == 1) && (item.Items[0] is string))
            {
                item.Items.Clear();

                DirectoryInfo expandedDir = null;
                
                try
                {
                    if (item.Tag is DriveInfo)
                    {
                        expandedDir = (item.Tag as DriveInfo).RootDirectory;
                    }
                    if (item.Tag is DirectoryInfo)
                    {
                        expandedDir = (item.Tag as DirectoryInfo);
                    }
                    foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                    {
                        item.Items.Add(CreateTreeItem(subDir));
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }

    public class MenuItem
    {
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }
    }

    public class Family
    {
        public Family()
        {
            this.Members = new ObservableCollection<FamilyMember>();
        }

        public string Name { get; set; }

        public ObservableCollection<FamilyMember> Members { get; set; }
    }

    public class FamilyMember
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
