using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPserverClassLibrary
{
    public class TCPClient
    {

        TcpClient m_client;
        IPAddress m_ip;

        EndPoint m_thisIP;
        public EndPoint ThisIP
        {
            get { return m_thisIP; }
        }

        EndPoint m_IPEndPoint;
        public EndPoint IPEndPoint
        {
            get { return m_IPEndPoint; }
        }
       
        byte[] ReadData;
        NetworkStream m_stream;

        private DateTime m_DateCreate;
        public DateTime DateCreate
        {
            get { return m_DateCreate; }
        }


        public TCPClient(IPAddress ip, int port)
        {
            m_ip = ip;
            ReadData = new byte[4096];
            Connect(ip, port);
            m_DateCreate = DateTime.Now;
            Console.WriteLine("TCP connect is ectablishment "+ip.ToString()+":"+port.ToString());
        }

        private void Connect(IPAddress ip, int port)
        {
            m_client = null;
            m_stream = null;
            try
            {
                m_connectionIsLost = 0;
                m_client = new TcpClient();
                m_client.Connect(ip, port);
                m_stream = m_client.GetStream();
                m_IPEndPoint = m_client.Client.LocalEndPoint;
                m_thisIP = m_client.Client.RemoteEndPoint;
                m_stream.BeginRead(ReadData, 0, ReadData.Length, ReceiverCallback, null);
                Thread lookingForServerThread = new Thread(LookingForServer);
        //        lookingForServerThread.Start();
            }
            catch
            {
                if (m_client != null)
                    m_client = null;
                System.Windows.Forms.MessageBox.Show("не удалось установить соединение");
            }
        }

        public Action<TCPClient> DisconectClient_action;
        public void Disconect()
        {
            if (m_client != null)
            {
                //if (LineReceived_action != null)
                  //  LineReceived_action(m_ip, "Connection lost");
                if (DisconectClient_action != null)
                    DisconectClient_action(this);
                if (m_client.Connected)
                {
                    m_client.Client.Shutdown(SocketShutdown.Both);
                    m_client.Close();
                }
            }
        }

        public void DataWrite(byte[] msg)
        {
            try
            {
                byte[] buffer = new byte[4096];
                //buffer = Encoding.UTF8.GetBytes(msg);
                buffer = msg;
                m_stream.Write(buffer, 0, buffer.Length);
            }
            catch { }

        }

        public Action<byte[]> LineReceived_action;
        private void ReceiverCallback(IAsyncResult ar)
        {
            try
            {
                m_connectionIsLost = 0;
                int ByteRead = m_client.GetStream().EndRead(ar);
                if (ByteRead < 1)
                {
                    Disconect();
                }
                if (Encoding.UTF8.GetString(ReadData, 0, ByteRead) != "Yes, i am here")
                {
                    //string msg = System.Text.Encoding.UTF8.GetString(ReadData, 0, ByteRead);
                    if (LineReceived_action != null)
                        LineReceived_action(ReadData);
                }
                m_client.GetStream().BeginRead(ReadData, 0, ReadData.Length, ReceiverCallback, null);
                for (int i = 0; i < ReadData.Length; i++)
                    ReadData[i] = 0;                
            }
            catch { }
        }

        private int m_connectionIsLost;
        private void LookingForServer()
        {
            while (m_client.Connected)
            {
                if (m_connectionIsLost > 5)
                {
                    DataWrite(Encoding.UTF8.GetBytes("Are you here?"));
                }
                if (m_connectionIsLost > 10)
                {
                    Disconect();
                    break;
                }
                m_connectionIsLost += 1;
                Thread.Sleep(1000);
            }    
        }

    }
}
