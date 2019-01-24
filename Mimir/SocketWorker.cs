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
    class SocketWorker
    {
        private readonly Logger log = new Logger("Socket");

        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public SocketWorker(int port, int listen)
        {
            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                log.Info($"Socket created, bind to *:{port}.");
                socket.Listen(listen);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Start()
        {
            socket.BeginAccept(new AsyncCallback(OnAccept), socket);
            log.Info("Socket is listing now.");
        }

        public void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket _socket = ar.AsyncState as Socket;
                Socket new_client = _socket.EndAccept(ar);
                _socket.BeginAccept(new AsyncCallback(OnAccept), _socket);

                byte[] recv_buffer = new byte[1024 * 1024 * 10];
                string messageData = "";
                int real_recv = 0;

                real_recv = new_client.Receive(recv_buffer);

                messageData = "";
                messageData = Encoding.Default.GetString(recv_buffer, 0, real_recv);

                if (messageData.ToString() != "")
                {
                    if (Program.IsDebug)
                    {
                        log.Debug($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                    }

                    Router.Route(messageData, new_client);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }
    }
}
