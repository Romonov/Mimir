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
                sslStream.AuthenticateAsServer(Program.serverCertificate);

                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;

                byte[] recv_buffer = new byte[1024 * 1024 * 10];
                StringBuilder messageData = new StringBuilder();
                int real_recv = 0;

                if (Program.UseSsl)
                {
                    real_recv = sslStream.Read(recv_buffer, 0, recv_buffer.Length);
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(recv_buffer, 0, real_recv)];
                    decoder.GetChars(recv_buffer, 0, real_recv, chars, 0);
                    messageData.Append(chars);
                }
                else
                {
                    string recv_request = Encoding.Default.GetString(recv_buffer, 0, real_recv);
                }

                if (messageData.ToString() != "")
                {
                    Logger.Info($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                    Processor.Process(messageData.ToString(), new_client, sslStream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("") != -1)
                {
                    break;
                }
            }
            while (bytes != 0);

            return messageData.ToString();
        }

    }
}
