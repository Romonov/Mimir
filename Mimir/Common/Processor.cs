using RUL;
using RUL.HTTP;
using Mimir.Response;
using Mimir.Response.AuthServer;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Mimir.Response.Mimir;
using Mimir.Response.Users;
using System;

namespace Mimir.Common
{
    public class Processor
    {
        /// <summary>
        /// 路由并处理请求
        /// </summary>
        /// <param name="req">请求字符串</param>
        /// <param name="socket">Socket实例</param>
        /// <param name="sslStream">SslStream实例</param>
        /// <param name="IsSSL">是否启用SSL</param>
        public static void Process(string req, Socket socket, SslStream sslStream, bool IsSSL)
        {
            HttpMsg msg = HttpProtocol.Solve(req);

            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            ReturnContent content = new ReturnContent();

            content.Status = 200;
            content.Contect = "";

            try
            {
                if (msg.Method == Method.Get)
                {
                    switch (msg.Url)
                    {
                        case "/":
                            content = Root.OnGet();
                            break;
                        case "/mimir/notice":
                            content = Notice.OnGet();
                            break;
                        default:
                            content.Status = 403;
                            break;
                    }
                }
                else if (msg.Method == Method.Post)
                {
                    switch (msg.Url)
                    {
                        #region Users
                        case "/users/register":
                            content = Register.OnPost(msg.PostData);
                            break;
                        case "/users/login":
                            content = Login.OnPost(msg.PostData);
                            break;
                        case "/users/logout":
                            content = LogOut.OnPost(msg.PostData);
                            break;
                        #endregion

                        #region AuthServer
                        case "/authserver/authenticate":
                            content = Authenticate.OnPost(msg.PostData);
                            break;
                        case "/authserver/refresh":
                            content = Refresh.OnPost(msg.PostData);
                            break;
                        case "/authserver/validate":
                            content = Validate.OnPost(msg.PostData);
                            break;
                        case "/authserver/invalidate":
                            content = Invalidate.OnPost(msg.PostData);
                            break;
                        case "/authserver/signout":
                            content = Signout.OnPost(msg.PostData);
                            break;
                        #endregion
                        default:
                            /*
                            GET /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}

                            if (Guid.TryParse(msg.Url.Split('/')[5], out Guid guid))
                            {

                            }
                            */
                            content.Status = 403;
                            break;
                    }
                }
                else
                {
                    switch (msg.Url)
                    {
                        default:
                            if (Program.IsSslEnabled)
                            {
                                content.Status = 200;
                                content.Contect = "Please use HTTPS, and this server only accept Get and Post.";
                            }
                            else
                            {
                                content.Status = 403;
                            }
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }
            
            bcontect = Encoding.Default.GetBytes(content.Contect);
            responseHeader = HttpProtocol.Make(content.Status, "text", bcontect.Length);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            Logger.Debug($"Response header: {responseHeader}");
            Logger.Debug($"Response contect: {content.Contect}");

            try
            {
                if (IsSSL)
                {
                    sslStream.Write(bresponse);
                    sslStream.Write(bcontect);
                    sslStream.Flush();
                }
                else
                {
                    socket.Send(bresponse);
                    socket.Send(bcontect);
                }
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }
            finally
            {
                //if (msg.Connection != Connection.KeepAlive)
                {
                    sslStream.Close();
                    socket.Close();
                }
            }
            return;
        }
        
        /// <summary>
        /// 返回值的自定义类型
        /// </summary>
        public struct ReturnContent
        {
            public string Contect;
            public int Status;
        }

        /// <summary>
        /// 错误信息返回的自定义类型
        /// </summary>
        public struct ReturnError
        {
            public string error;
            public string errorMessage;
            public string cause;
        }
    }
}