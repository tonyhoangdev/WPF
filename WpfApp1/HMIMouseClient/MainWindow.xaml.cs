using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using SpdTeam.Hooks;
using System.Net;

namespace HMIMouseClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int PORT_NUMBER = 55555;
        private string IP = "192.168.0.100";
        static ASCIIEncoding encoding = new ASCIIEncoding();
        TcpClient client = null;
        bool isConnected = false;
        const string STATUS_HEDAER = "Status: ";
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        MouseHook mouseHook;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void InstallMouse()
        {
            mouseHook = new MouseHook();
            mouseHook.LeftButtonDown += new MouseHook.MouseHookCallback(mouseHook_LeftButtonDown);
            mouseHook.LeftButtonUp += new MouseHook.MouseHookCallback(mouseHook_LeftButtonUp);
            mouseHook.Install();           
        }

        private void UninstallMouse()
        {
            if (mouseHook != null)
            {
                mouseHook.LeftButtonDown -= new MouseHook.MouseHookCallback(mouseHook_LeftButtonDown);
                mouseHook.LeftButtonUp -= new MouseHook.MouseHookCallback(mouseHook_LeftButtonUp);
                mouseHook.Uninstall();
            }

            lblStatusPointDown.Content = "x: | y: ";
        }

        bool isMouseLeftButtonDown = false;
        bool isMouseLeftButtonUp = false;
        bool isMouseMove = false;
        private void mouseHook_LeftButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            isMouseLeftButtonUp = true;
            lblStatusPointUp.Content = "Up: x: " + mouseStruct.pt.x + " | y: " + mouseStruct.pt.y;

        }

        private void mouseHook_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            isMouseLeftButtonDown = false;

            lblStatusPointDown.Content = "Down: x: " + mouseStruct.pt.x + " | y: " + mouseStruct.pt.y;
            string appName = "/opt/bin/touchinput";
            string args = string.Format(" tap {0} {1}", mouseStruct.pt.x, mouseStruct.pt.y);
            string cmd = appName + args;

            try
            {
                if (isConnected)
                {
                    byte[] data = encoding.GetBytes(cmd);
                    Stream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception)
            {
                lblStatus.Content = "Error: Don't have connect to server!";
            }

        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                isServerAvailable = true;

                if (cbAutoConnect.IsChecked == true && !isConnected)
                {
                    btnConnect_Click(null, null);
                }
            }
            else
            {
                isServerAvailable = false;
                isConnected = false;
                btnDisconnect_Click(null, null);
            }
            
            lblStatusRun.Content = "Server available: " + isServerAvailable.ToString();
        }

        bool isServerAvailable = false;
        IPAddress address;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(txtIP.Text.Trim(), out address))
            {
                lblStatus.Content = "IP ERROR...!";
                return;
            }

            IP = txtIP.Text.Trim();
            lblStatus.Content = STATUS_HEDAER;

            using (Ping ping = new Ping())
            {
                ping.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                ping.SendAsync(IP, null);
            }                           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGui(isConnected);
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                int.TryParse(txtPort.Text, out PORT_NUMBER);
                IP = txtIP.Text.Trim();
                var result = client.BeginConnect(IP, PORT_NUMBER, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (!success)
                {
                    return;
                }
                client.EndConnect(result);

                isConnected = true;

                InstallMouse();
            }
            catch (Exception)
            {
                isConnected = false;
                client.Close();
            }    
            finally
            {
                UpdateGui(isConnected);
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                isConnected = false;
                client.Close();
                UninstallMouse();
                UpdateGui(isConnected);                
            }                
        }

        private void UpdateGui(bool isConnected)
        {
            if (isConnected)
            {
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = true;
                lblStatus.Content = STATUS_HEDAER + "Connected!";
                txtIP.IsEnabled = false;
                txtPort.IsEnabled = false;
            }
            else
            {
                txtIP.IsEnabled = true;
                txtPort.IsEnabled = true;
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
                lblStatus.Content = STATUS_HEDAER + "Disconnected!";
            }
        }
    }
}
