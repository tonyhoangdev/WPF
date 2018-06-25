
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Selenium
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
            //// ChromeDriver cd = new ChromeDriver();

            // profile
            // turn off webRTC
            FirefoxProfile firefoxProfile = new FirefoxProfile();

            if ((bool)cbProxyWebRTC.IsChecked)
            {
                firefoxProfile.SetPreference("media.peerconnection.enabled", false);
            }

            if ((bool)cbProxyEnabled.IsChecked)
            {
                // changes proxy
                string proxy_ip = txtProxyIp.Text;
                int proxy_port = int.Parse(txtProxyPort.Text);

                firefoxProfile.SetPreference("network.proxy.type", 1);
                firefoxProfile.SetPreference("network.proxy.http", proxy_ip);
                firefoxProfile.SetPreference("network.proxy.http_port", proxy_port);
                firefoxProfile.SetPreference("network.proxy.ssl", proxy_ip);
                firefoxProfile.SetPreference("network.proxy.ssl_port", proxy_port);
            }

            // init
            FirefoxDriver cd = new FirefoxDriver(firefoxProfile);

            if (!(bool)cbIsachInfo.IsChecked)
            {
                // go to link
                if (!String.IsNullOrEmpty(txtLink.Text))
                {
                    cd.Url = txtLink.Text;
                }
                else
                {
                    cd.Url = "https://whoer.net/";
                }

                cd.Navigate();
            }
            else
            {
                cd.Url = "https://isach.info/login.php";
                cd.Navigate();

                var search = cd.FindElementById("user_name");
                search.SendKeys(txtUser.Text);

                search = cd.FindElementById("user_password");
                search.SendKeys(passPass.Password);

                search.SendKeys(Keys.Return);

                // sleep
                Thread.Sleep(4000);

                // go home
                cd.Url = "https://isach.info";
                cd.Navigate();
            }

        }
    }
}
