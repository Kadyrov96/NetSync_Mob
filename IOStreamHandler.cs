using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace MobTest
{
    class IOStreamHandler : IStreamHandler
    {
        private static int bufSize;
        private StreamSocket tcpClient;

        private DataWriter socketWriter;
        private DataReader socketReader;

        public IOStreamHandler(StreamSocket _TcpClient)
        {
            tcpClient = _TcpClient;
            bufSize = 2048;
        }

        private async void OutputStreamWriter(byte[] buffer)
        {
            socketWriter.WriteBytes(buffer);
            await socketWriter.StoreAsync();
        }

        private async void InputStreamWriter(byte[] buffer, uint count)
        {
            await socketReader.LoadAsync(count);
            socketReader.ReadBytes(buffer);
        }

        async void IStreamHandler.ReceiveData(string savingFolderPath)
        {
            try
            {
                if (tcpClient != null)
                {
                    socketReader = new DataReader(tcpClient.InputStream);
                    socketReader.InputStreamOptions = InputStreamOptions.Partial;

                    //Reading first field of incoming packet - name of the file
                    uint readBytesSize = await socketReader.LoadAsync((uint)bufSize);
                    byte[] downBuffer = new byte[readBytesSize];
                    socketReader.ReadBytes(downBuffer);
                    string FileName = Encoding.Unicode.GetString(downBuffer, 0, downBuffer.Length);

                    //Reading second field of incoming packet - size of the file
                    readBytesSize = await socketReader.LoadAsync((uint)bufSize);
                    downBuffer = new byte[readBytesSize];
                    socketReader.ReadBytes(downBuffer);
                    uint FileSize = Convert.ToUInt32(Encoding.Unicode.GetString(downBuffer, 0, downBuffer.Length));

                    //Reading file content
                    List<byte> bufferList = new List<byte>();
                    int readenBytes = 0;
                    while (readenBytes < FileSize)
                    {
                        readBytesSize = await socketReader.LoadAsync(FileSize);
                        downBuffer = new byte[readBytesSize];
                        socketReader.ReadBytes(downBuffer);
                        bufferList.AddRange(downBuffer);
                        readenBytes += (int)readBytesSize;
                    }
                    FileStream fileWriter = new FileStream(savingFolderPath + @"\" + FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    fileWriter.Write(bufferList.ToArray(), 0, (int)FileSize);
                }
            }
            catch(Exception exMessage)
            {
                MessageBox.Show(exMessage.Message, "Service information", MessageBoxButton.OK);
            }
        }

        void IStreamHandler.SendData(string filePath)
        {
            try
            {
                socketWriter = new DataWriter(tcpClient.OutputStream);
                if (tcpClient != null)
                {
                    //Getting info about selected file
                    FileInfo fInfo = new FileInfo(filePath);
                    string fileName = fInfo.Name;
                    long fileSize = fInfo.Length;

                    //Sending first field of the packet - name of the file
                    byte[] bytedFileName = new byte[bufSize];
                    bytedFileName = Encoding.Unicode.GetBytes(fileName.ToCharArray());
                    OutputStreamWriter(bytedFileName);

                    //Sending second field of the packet - size of the file
                    byte[] bytedFileSize = new byte[bufSize];
                    bytedFileSize = Encoding.Unicode.GetBytes(fileSize.ToString().ToCharArray());
                    OutputStreamWriter(bytedFileSize);

                    //Sending file content
                    byte[] sendBuffer = new byte[fileSize];
                    FileStream fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    fileReader.Read(sendBuffer, 0, (int)fileSize);
                    fileReader.Close();
                    OutputStreamWriter(sendBuffer);
                    MessageBox.Show("File sent. Closing streams and connections...", "Service information", MessageBoxButton.OK);
                }
            }
            catch (Exception exMessage)
            {
                MessageBox.Show(exMessage.Message, "Service information", MessageBoxButton.OK);
            }
        }
    }
}

