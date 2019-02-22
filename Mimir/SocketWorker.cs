using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    /// <summary>
    /// Socket管理类
    /// </summary>
    class SocketWorker
    {
        private readonly Logger log = new Logger("Socket");

        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        /// <summary>
        /// 实例化SocketWorker
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <param name="listen">最大等待队列长度</param>
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

        /// <summary>
        /// 启动Socket监听
        /// </summary>
        public void Start()
        {
            socket.BeginAccept(new AsyncCallback(OnAccept), socket);
            log.Info("Socket is listing now.");
        }

        /// <summary>
        /// 在监听到有请求时触发方法
        /// </summary>
        /// <param name="ar">异步回调</param>
        public void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket _socket = ar.AsyncState as Socket;
                Socket new_client = _socket.EndAccept(ar);
                _socket.BeginAccept(new AsyncCallback(OnAccept), _socket);

                SslStream sslStream = null;

                byte[] recv_buffer = new byte[1024 * 1024 * 10];
                string messageData = "";
                int real_recv = 0;

                if (Program.SslIsEnable)
                {
                    sslStream = new SslStream(new NetworkStream(new_client, true), false);
                    sslStream.AuthenticateAsServer(Program.SslCert, false, SslProtocols.Tls11, false);
                    sslStream.ReadTimeout = 5000;
                    sslStream.WriteTimeout = 5000;
                    real_recv = sslStream.Read(recv_buffer, 0, recv_buffer.Length);
                }
                else
                {
                    real_recv = new_client.Receive(recv_buffer);
                }

                messageData = "";
                messageData = Encoding.Default.GetString(recv_buffer, 0, real_recv);

                if (messageData.ToString() != "")
                {
                    if (Program.IsDebug)
                    {
                        log.Debug($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                    }

                    Router.Route(messageData, new_client, sslStream);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
