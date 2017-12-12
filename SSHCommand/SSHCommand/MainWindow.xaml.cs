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
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using Renci.SshNet.Common;

namespace SSHCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PasswordConnectionInfo info = null;
        PasswordConnectionInfo infoBoard = null;
        string user = null;
        string pass = null;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void PrepareUpload()
        {
            using (var sshclient = new SshClient(info))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("mkdir -p ~/tmp/uploadtest && chmod +rw ~/tmp/uploadtest"))
                {
                    cmd.Execute();
                    Console.WriteLine("Command > " + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                }
                sshclient.Disconnect();
            }

        }

        private void ExecuteShell()
        {
            using (var sshclient = new SshClient(info))
            {
                sshclient.Connect();

                Console.WriteLine(sshclient.CreateCommand("cd ~/tmp && ls -lah").Execute());
                Console.WriteLine(sshclient.CreateCommand("pwd").Execute());
                Console.WriteLine(sshclient.CreateCommand("cd ~/tmp/uploadtest && ls -lah").Execute());
                sshclient.Disconnect();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            info = new PasswordConnectionInfo(txtIP.Text, Convert.ToInt32(txtPort.Text), txtUser.Text, passwordBox.Password);
            // info = new PasswordConnectionInfo(txtIP.Text, "root", "");

            using (var client = new SshClient(info))
            {
                client.Connect();

                var a = client.RunCommand("touch file2.txt").ToString();
                var b = client.CreateCommand("touch file3.txt").Execute();
                var c = client.CreateCommand("ls -la").Execute();

            }

            lblStatus.Content = "Done Connect";

            PrepareUpload();

            lblStatus.Content = "Done Prepare Upload";
        }

        private void btnUpFile_Click(object sender, RoutedEventArgs e)
        {
            using (var sftp = new SftpClient(info))
            {
                string uploadfile = "Renci.SshNet.dll";

                sftp.Connect();
                sftp.ChangeDirectory(string.Format("/home/{0}/tmp/uploadtest", txtUser.Text));
                using (var upfileStream = File.OpenRead(uploadfile))
                {
                    sftp.UploadFile(upfileStream, uploadfile, true);
                }
                sftp.Disconnect();
            }

            ExecuteShell();
        }

        private void btnBuild_Click(object sender, RoutedEventArgs e)
        {
            using (var sshclient = new SshClient(info))
            {
                sshclient.Connect();

                Console.WriteLine(sshclient.CreateCommand("cd ~/ikc_cluster/qnx/GP/Apps/HMI/HMIApp && pwd && ./build_ikc_hmiapp.sh").Execute());

                lblStatus.Content = "Done Build";
            }

        }

        private void btnDownFile_Click(object sender, RoutedEventArgs e)
        {
            info = new PasswordConnectionInfo(txtIP.Text, Convert.ToInt32(txtPort.Text), txtUser.Text, passwordBox.Password);

            using (var sftp = new SftpClient(info))
            {
                sftp.Connect();

                string remoteDirectory = String.Format("/home/{0}/ikc_cluster/qnx/GP/Apps/HMI/HMIApp/Application/bin", txtUser.Text);
                var files = sftp.ListDirectory(remoteDirectory);

                foreach (var file in files)
                {
                    string remoteFileName = file.Name;
                    long size = file.Length;
                    long sizeDownloaded = 0;


                    if (remoteFileName.Equals("HMIApp"))
                    {
                        lblStatus.Content = "File size: " + size;

                        using (Stream fileOut = File.OpenWrite("D:\\HMIApp"))
                        {
                            sftp.DownloadFile(file.FullName, fileOut);
                        }
                    }
                }

                sftp.Disconnect();
                lblStatus.Content = "Done Download";
            }
        }

        SshClient clientBoard = null;
        private void btnConnectBoard_Click(object sender, RoutedEventArgs e)
        {
            infoBoard = new PasswordConnectionInfo(txtIPBoard.Text, Convert.ToInt32(txtPortBoard.Text), txtUserBoard.Text, passwordBoxBoard.Password);

            clientBoard = new SshClient(infoBoard);

            clientBoard.Connect();
            lblStatusBoard.Content = "Connected";      
            

            clientBoard.RunCommand("mount -n -o remount, rw /").ToString();
        }

        private void btnUpFileBoard_Click(object sender, RoutedEventArgs e)
        {
            SftpClient sftp = null;

            try
            {
                using (sftp = new SftpClient(infoBoard))
                {
                    string uploadfile = "debug";

                    sftp.Connect();
                    sftp.ChangeDirectory("/Apps");
                    using (var upfileStream = File.OpenRead(uploadfile))
                    {
                        sftp.UploadFile(upfileStream, uploadfile, true);
                    }

                    lblStatusBoard.Content = "Uploaded: " + uploadfile + " file!";
                }
            }
            catch (Exception)
            {
                lblStatusBoard.Content = "Upload Fail!";
            }
            finally
            {
                sftp.Disconnect();
            }

        }

        private void btnDownFileBoard_Click(object sender, RoutedEventArgs e)
        {

        }

        bool boardToggle = false;
        private void btnDebugBoard_Click(object sender, RoutedEventArgs e)
        {
            boardToggle = !boardToggle;
            if (boardToggle)
            {
                clientBoard.RunCommand("/Apps/debug DYN DP_ID_SETTING_LANGUAGE 1 0");
            }
            else
            {
                clientBoard.RunCommand("/Apps/debug DYN DP_ID_SETTING_LANGUAGE 1 1");
            }

        }
    }
}
