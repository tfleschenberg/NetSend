using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetSend
{
    public static class AsyncUDPClient
    {
        public static event EventHandler<string> onReceived;

        private static bool Closing;
        private static IPEndPoint localEP;
        private static IPEndPoint remoteEP;
        private static UdpClient udpClient;

        static AsyncUDPClient()
        {
            Closing = false;
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
        }

        public static void Bind(int port)
        {
            try
            {
                remoteEP = new IPEndPoint(IPAddress.Broadcast, port);
                localEP = new IPEndPoint(IPAddress.Any, port);

                udpClient.Client.Bind(localEP);
                
                Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Close()
        {
            Closing = true;
            udpClient.Close();
        }

        private static void Receive()
        {
            try
            {
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReceiveCallback(IAsyncResult asyncResult)
        {
            if (Closing) return;

            try
            {
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
                onReceived?.Invoke(null, Encoding.GetEncoding(437).GetString(udpClient.EndReceive(asyncResult, ref localEP)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Send(String data)
        {
            byte[] byteData = Encoding.GetEncoding(437).GetBytes(data);

            udpClient.SendAsync(byteData, byteData.Length, remoteEP);
        }
    }
}
