using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using System.Threading;
using System.Windows.Threading;
using System.Net.NetworkInformation;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int PORT_NUMBER = 4567;
        private const string IP = "192.168.0.1";
        static ASCIIEncoding encoding = new ASCIIEncoding();
        TcpClient client = null;
        bool isConnected = false;
        const string STATUS_HEDAER = "Status: ";
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                isServerAvailable = true;
            }
            else
            {
                isServerAvailable = false;
                isConnected = false;
            }

            UpdateGui(isConnected);

            lblStatusRun.Content = "Server available: " + isServerAvailable.ToString();
        }

        bool isServerAvailable = false;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            using (Ping ping = new Ping())
            {
                ping.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                ping.SendAsync(IP, null);
            }                           
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("UP");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("OK");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("DOWN");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnTrip_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("TRIP");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnOKLong_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("OK_LONG");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
        
        int sliderSpeedValue = 0;
        int sliderTachoValue = 0;
        private void slider_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSpeedValue = Convert.ToInt32(sliderSpeed.Value);
        }

        private void sliderTacho_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderTachoValue = Convert.ToInt32(sliderTacho.Value);
        }

        private void sliderSpeed_LostMouseCapture(object sender, MouseEventArgs e)
        {
            byte[] data = encoding.GetBytes("Speed");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes("DPID_GAUGE_SPEEDOMETER_IN:n:" + sliderSpeedValue.ToString() + "\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void sliderTacho_LostMouseCapture(object sender, MouseEventArgs e)
        {
            byte[] data = encoding.GetBytes("Tacho");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes("DPID_GAUGE_TACHOMETER_IN:n:" + sliderTachoValue.ToString() + "\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnAccOn_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("ACC");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes("DPID_ACCESSORY_IN:n:2\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnAccOff_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("ACC");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes("DPID_ACCESSORY_IN:n:1\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnFullPopupOff_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("OffFullPopup");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes(":n:1\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnMiniPopupOff_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = encoding.GetBytes("OffMiniPopup");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            Thread.Sleep(300);

            data = encoding.GetBytes(":n:1\n");
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void btnFullPopupOn_Click(object sender, RoutedEventArgs e)
        {
            int fullPopupValue;
            string dpidAlertName = txtDpidAlertFullName.Text;
            int dpidAlertValue;

            int.TryParse(txtDpidFullPopupValue.Text, out fullPopupValue);
            int.TryParse(txtDpidAlertFullValue.Text, out dpidAlertValue);

            if (string.IsNullOrEmpty(dpidAlertName))
            {
                MessageBox.Show("Please correct DPID ALERT name!", "Notice");

                return;
            }

            byte[] data = encoding.GetBytes("FullPopup");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Thread.Sleep(300);
            data = encoding.GetBytes(fullPopupValue.ToString());
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Thread.Sleep(300);
            data = encoding.GetBytes(string.Format("{0}:n:{1}\n", dpidAlertName, dpidAlertValue));
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

        }

        private void btnMiniPopupOn_Click(object sender, RoutedEventArgs e)
        {
            int miniPopupValue;
            string dpidAlertName = txtDpidAlertMiniName.Text;
            int dpidAlertValue;

            int.TryParse(txtDpidMiniPopupValue.Text, out miniPopupValue);
            int.TryParse(txtDpidAlertMiniValue.Text, out dpidAlertValue);

            if (miniPopupValue == 0 || string.IsNullOrEmpty(dpidAlertName))
            {
                int popupError = 0;

                if (miniPopupValue == 0)
                {
                    popupError |= 1;
                }

                if (string.IsNullOrEmpty(dpidAlertName))
                {
                    popupError |= 2;
                }

                if (popupError == 1)
                {
                    MessageBox.Show("Please correct MINI POPUP value!", "Notice");
                }
                else if (popupError == 2)
                {
                    MessageBox.Show("Please correct DPID ALERT name!", "Notice");
                }
                else if (popupError == 3)
                {
                    MessageBox.Show("Please correct MINI POPUP and DPID ALERT name!", "Notice");
                }

                return;
            }

            byte[] data = encoding.GetBytes("MiniPopup");
            Stream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Thread.Sleep(300);
            data = encoding.GetBytes(miniPopupValue.ToString());
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Thread.Sleep(300);
            data = encoding.GetBytes(string.Format("{0}:n:{1}\n", dpidAlertName, dpidAlertValue));
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

        }

        private void btnTabAssist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTabTrip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTabNav_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTabSport_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isConnected)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                case Key.A + 32:
                    btnTrip_Click(sender, e);
                    break;
                case Key.S:
                case Key.S + 32:
                    btnDown_Click(sender, e);
                    break;
                case Key.D:
                case Key.D + 32:
                    btnOK_Click(sender, e);
                    break;
                case Key.W:
                case Key.W + 32:
                    btnUp_Click(sender, e);
                    break;
                case Key.Q:
                case Key.Q + 32:
                    btnOKLong_Click(sender, e);
                    break;
                default:
                    break;
            }
            
            if (e.Key == Key.D1 && e.Key == Key.LeftAlt)
            {
                lblStatusRun.Content = "- Key: Alt + 1";
            }
            //lblStatusRun.Content = "Key: " + e.Key;
            e.Handled = false;
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
                var result = client.BeginConnect(IP, PORT_NUMBER, null, null);
                //client.Connect(IP, PORT_NUMBER);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (!success)
                {
                    return;
                }
                client.EndConnect(result);

                isConnected = true;
                
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
                UpdateGui(isConnected);                
            }                
        }

        private void UpdateGui(bool isConnected)
        {
            if (isConnected)
            {
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = true;
                groupMMI.IsEnabled = true;
                groupGauge.IsEnabled = true;
                groupAcc.IsEnabled = true;
                tabControl.IsEnabled = true;
                lblStatus.Content = STATUS_HEDAER + "Connected to sever!";
            }
            else
            {
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
                groupMMI.IsEnabled = false;
                groupGauge.IsEnabled = false;
                groupAcc.IsEnabled = false;
                tabControl.IsEnabled = false;
                lblStatus.Content = STATUS_HEDAER + "Not connected to server!";
            }
        }
    }
}
