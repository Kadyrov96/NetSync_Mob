using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace MobTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        private FolderHandler syncFolderHandler;
        private IStreamHandler streamsController;
        private Synchroniser synchroniser;
        private SSL_Client client;

        public MainPage()
        {
            InitializeComponent();
            syncFolderHandler = new FolderHandler();
        }

        private void select_button_Click(object sender, RoutedEventArgs e)
        {
            syncFolderHandler.SelectFolder();
        }

        private void wifi_button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            connectionSettingsTask.Show();
        }

        private void btnConnect_Click_1(object sender, RoutedEventArgs e)
        {
            client.ConnectToServer(txtServer.Text);
            streamsController = new IOStreamHandler(client.TcpClient);
        }

        private void btnDisconnect_Click_1(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
        }

        private async void sync_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                while (tcpClient == null)
                {
                    HostName serverHost = new HostName(txtServer.Text);
                    tcpClient = new StreamSocket();
                    try
                    {
                        await tcpClient.ConnectAsync(serverHost, "816", SocketProtectionLevel.Tls12);
                        streamsController.ReceiveData(hashHolderDirectory);
                        uplFilesList = new List<string>();
                        TextReader statementFileReader = new StreamReader(hashHolderDirectory + "ST.txt");
                        {
                            string temp;
                            int i = 0;
                            while ((temp = statementFileReader.ReadLine()) != null)
                            {
                                i++;
                                if (temp.Length < 3)
                                {
                                    incoming_count = Convert.ToInt32(temp);
                                }
                                else
                                {
                                    switch (temp.Substring(temp.Length - 4, 4))
                                    {
                                        case "DOWN":
                                            uplFilesList.Add(temp.Substring(0, temp.Length - 5));
                                            break;

                                        case "DELT":
                                            File.Delete(syncDirPath + @"\" + temp.Substring(0, temp.Length - 5));
                                            break;
                                    }
                                }
                            }

                            for (int ji = 0; ji < incoming_count; ji++)
                            {
                                streamsController.ReceiveData(syncDirPath);                           
                            }

                            foreach (var fileName in uplFilesList)
                            {
                                streamsController.SendData(syncDirPath + @"\" + fileName);
                            }
                        }
                    }
                    catch (Exception exMessage)
                    {
                        MessageBox.Show(exMessage.Message, "Service information", MessageBoxButton.OK);
                    }
                    finally
                    {
                        fileWriter.Close();
                        fileWriter.Dispose();
                    }
                }
            }
            catch (Exception exMessage)
            {
                MessageBox.Show(exMessage.Message, "Service information", MessageBoxButton.OK);
            }
        }
        private void scan_button_Click(object sender, RoutedEventArgs e)
        {
            if(synchroniser.IsSyncDataStoreExists())
            {
                synchroniser.ReadSyncDataStore();
            }
            else
            {
                synchroniser.CreateSyncDataStore();
            }
            synchroniser.CompareDevicesSyncData();

           
        }
    }
}
