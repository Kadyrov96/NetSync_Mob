using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace MobTest
{
    class SSL_Client
    {
        private StreamSocket tcpClient;

        public StreamSocket TcpClient
        {
            get
            {
                return tcpClient;
            }

            set
            {
                tcpClient = value;
            }
        }

        public SSL_Client()
        {
            string host = Dns.GetHostName();
            serverCertificate = new X509Certificate2(@"C:\Users\Admin\Desktop\123.pfx", "vbhs456");
            listener = new TcpListener(Dns.GetHostEntry(host).AddressList[0], 816);
        }
        internal async void ConnectToServer(string _hostname)
        {
            while (tcpClient == null)
            {
                HostName serverHost = new HostName(_hostname);
                TcpClient = new StreamSocket();
                try
                {
                    await TcpClient.ConnectAsync(serverHost, "816", SocketProtectionLevel.Tls12);
                    MessageBox.Show("connected!", "Service information", MessageBoxButton.OK);
                }
                catch (Exception exMessage)
                {
                    MessageBox.Show(exMessage.Message, "Service information", MessageBoxButton.OK);
                }
            }

        }

        internal void Disconnect()
        {
            TcpClient.Dispose();
            TcpClient = null;
            MessageBox.Show("Disconnected from remote device!", "Service information", MessageBoxButton.OK);
        }
    }
}
