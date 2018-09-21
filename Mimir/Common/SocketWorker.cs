using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class SocketWorker
    {
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Socket GetSocket()
        {
            return socket;
        }

        public bool Init(int port, int listen)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Logger.Info($"Socket created, bind to localhost:{port}.");
            socket.Listen(listen);
            return true;
        }

        public void Start()
        {
            socket.BeginAccept(new AsyncCallback(OnAccept), socket);
            Logger.Info("Socket is listing now.");
        }

        public void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket _socket = ar.AsyncState as Socket;
                Socket new_client = _socket.EndAccept(ar);
                _socket.BeginAccept(new AsyncCallback(OnAccept), _socket);
                
                SslStream sslStream = new SslStream(new NetworkStream(new_client, true), false);
;
                byte[] recv_buffer = new byte[1024 * 1024 * 10];
                string messageData = "";
                int real_recv = 0;

                bool IsSSL = Program.IsSslEnabled;

                try
                {
                    sslStream.AuthenticateAsServer(Program.serverCertificate);

                    //sslStream.ReadTimeout = 5000;
                    //sslStream.WriteTimeout = 5000;

                    real_recv = sslStream.Read(recv_buffer, 0, recv_buffer.Length);

                    messageData = "";
                    messageData = Encoding.Default.GetString(recv_buffer, 0, real_recv);
                }
                catch (Exception e)
                {
                    Logger.Warn(e.Message);

                    real_recv = new_client.Receive(recv_buffer);

                    IsSSL = false;

                    messageData = "";
                    messageData = Encoding.Default.GetString(recv_buffer, 0, real_recv);
                }
                finally
                {
                    if (messageData.ToString() != "")
                    {
                        Logger.Debug($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                        Processor.Process(messageData, new_client, sslStream, IsSSL);
                    }
                }

            }

            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
