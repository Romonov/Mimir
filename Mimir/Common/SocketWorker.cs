using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class SocketWorker
    {
        /// <summary>
        /// Socket实例
        /// </summary>
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// 初始化Socket
        /// </summary>
        /// <param name="port">端监听口</param>
        /// <param name="listen">最大监听数量</param>
        /// <returns>是否成功</returns>
        public bool Init(int port, int listen)
        {
            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                Logger.Info($"Socket created, bind to localhost:{port}.");
                socket.Listen(listen);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            socket.BeginAccept(new AsyncCallback(OnAccept), socket);
            Logger.Info("Socket is listing now.");
        }

        /// <summary>
        /// 监听时触发
        /// </summary>
        /// <param name="ar">异步方法</param>
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
                    //Logger.Debug($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                    Router.Route(messageData, new_client);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
