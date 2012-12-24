using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace TCPserverClassLibrary
{
    public class TCPserver
    {
        TcpListener Listener;
        Thread ListenerThread;
        ArrayList m_ClientList;

        IPAddress m_ip;
        public IPAddress Ip
        {
            get { return m_ip; }
        }

        int m_port;
        public int Port
        {
            get { return m_port; }
        }

        private DateTime m_DateCreate;
        public DateTime DateCreate
        {
            get { return m_DateCreate; }
        }
       
        public Action<string, bool> ClientListIsChanged_action;
        public TCPserver(IPAddress IP, int PORT)
        {
            //try
            //{
                m_ip = IP;
                m_port = PORT;
                m_DateCreate = DateTime.Now;
                m_ClientList = new ArrayList();
                Listener = new TcpListener(m_ip, m_port);
                Listener.Start();
                ListenerThread = new Thread(new ThreadStart(DoListen));
                ListenerThread.Start();
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.ToString());
            //}
           
        }

        private void DoListen()
        {   
            while (ListenerThread != null)
            {
                try
                {
                    TcpClient client = this.Listener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    m_ClientList.Add(client);
                    if (ClientListIsChanged_action != null)
                        ClientListIsChanged_action(client.Client.RemoteEndPoint.ToString(), true);
                    clientThread.Start(client);
                }
                catch { CloseServer(); }
            }
        }

        public Action<byte[]> ServerReciveMessage_action;
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            int bytesRead;
            while (true)
            {
                bytesRead = 0;
                try
                {
                    byte[] message = new byte[4096];
                //    byte[] i_am_here_msg = Encoding.UTF8.GetBytes("Yes, i am here");
                    bytesRead = clientStream.Read(message, 0, message.Length);
                    byte[] tmp = new byte[bytesRead];
                    for (int i = 0; i < tmp.Length; i++)
                        tmp[i] = message[i];
                    //string recive_msg = Encoding.UTF8.GetString(message, 0, bytesRead);
                    //if (recive_msg == "Are you here?")
                    //    clientStream.Write(i_am_here_msg, 0, i_am_here_msg.Length);
                    //else
                    //{
                        if (ServerReciveMessage_action != null)
                            ServerReciveMessage_action(tmp);
                        //foreach (TcpClient oneClient in m_ClientList)
                        //{
                        //    NetworkStream oneStream = oneClient.GetStream();
                        //    oneStream.Write(message, 0, bytesRead);
                        //    oneStream.Flush();
                        //}
                  //  }
                }
                catch
                {
                    m_ClientList.Remove(tcpClient);
                    if (ClientListIsChanged_action != null)
                        ClientListIsChanged_action(tcpClient.Client.RemoteEndPoint.ToString(), false);
                    break;
                }

                if (bytesRead == 0)
                {
                    m_ClientList.Remove(tcpClient);
                    if (ClientListIsChanged_action != null)
                        ClientListIsChanged_action(tcpClient.Client.RemoteEndPoint.ToString(), false);
                    break;
                }
            }
            tcpClient.Close();
        }

        public void SendMessage(byte[] message)
        {
            int bytesRead = message.Length;
            foreach (TcpClient oneClient in m_ClientList)
            {
                NetworkStream oneStream = oneClient.GetStream();
                oneStream.Write(message, 0, bytesRead);
                oneStream.Flush();
            }
        }

        public void CloseServer()
        {
            ListenerThread = null;
            Listener.Stop();
            try
            {
                foreach (TcpClient client in m_ClientList)
                    client.Client.Disconnect(false);
            }
            catch { }
            m_ClientList.Clear();
        }

    }
}
