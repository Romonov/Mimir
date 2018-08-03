using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class Main
    {
        public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static bool Init(int port, int listen)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Logger.Info($"Socket created, bind to localhost:{port}, Max connection is {listen}.");
            socket.Listen(listen);
            return true;
        }

        public static void Start()
        {
            socket.BeginAccept(new AsyncCallback(OnAccept), socket);
            Logger.Info("Socket is listing now.");
        }

        static void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket _socket = ar.AsyncState as Socket;
                Socket new_client = _socket.EndAccept(ar);
                _socket.BeginAccept(new AsyncCallback(OnAccept), _socket);

                byte[] recv_buffer = new byte[1024 * 1024 * 100];
                int real_recv = new_client.Receive(recv_buffer);
                string recv_request = Encoding.ASCII.GetString(recv_buffer, 0, real_recv);

                if (recv_request != "")
                {
                    Logger.Info($"Recived request from {new_client.RemoteEndPoint}\n{recv_request}");
                    Processor.Process(recv_request, new_client);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
