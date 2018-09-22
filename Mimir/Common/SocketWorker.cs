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

namespace Mimir.Common
{
    class SocketWorker
    {
        /// <summary>
        /// Socket实例
        /// </summary>
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// 获取Socket实例
        /// </summary>
        /// <returns>Socket实例</returns>
        public Socket GetSocket()
        {
            return socket;
        }

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
            catch(Exception e)
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
                
                SslStream sslStream = new SslStream(new NetworkStream(new_client, true), false);
;
                byte[] recv_buffer = new byte[1024 * 1024 * 10];
                string messageData = "";
                int real_recv = 0;

                try
                {
                    if (Program.IsSslEnabled)
                    {
                        sslStream.AuthenticateAsServer(Program.ServerCertificate, false, SslProtocols.Tls, true);

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
                }
                catch (Exception e)
                {
                    throw;
                    Logger.Error(e.Message);
                }
                finally
                {
                    if (messageData.ToString() != "")
                    {
                        Logger.Debug($"Recived request from {new_client.RemoteEndPoint}\n{messageData.ToString()}");
                        Processor.Process(messageData, new_client, sslStream, Program.IsSslEnabled);
                    }
                }

            }

            catch (Exception e)
            {
                Logger.Error(e.Message); throw;
            }
        }
    }
}
