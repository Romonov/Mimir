using Mimir.Response;
using Mimir.Response.AuthServer;
using Mimir.Response.Users;
using RUL;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class Router
    {
        /// <summary>
        /// 路由并处理请求
        /// </summary>
        /// <param name="HttpReq">请求字符串</param>
        /// <param name="Socket">Socket实例</param>
        /// <param name="EndPoint">远程终结点</param>
        public static void Route(string HttpReq, Socket Socket, IPEndPoint IPEndPoint)
        {
            HttpReq msg = HttpProtocol.Solve(HttpReq);

            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            Tuple<int, string> Response = new Tuple<int, string>(403, "");

            // 处理请求
            try
            {
                if (msg.Method == Method.Get)
                {
                    switch (msg.Url.ToLower())
                    {
                        case "/":
                            Response = Root.OnGet();
                            break;
                        default:
                            if(!File.Exists(Program.Path + msg.Url.ToLower()))
                            {

                            }
                            break;
                    }
                }
                else if (msg.Method == Method.Post)
                {
                    switch (msg.Url.ToLower())
                    {
                        #region Users
                        case "/users/register":
                            Response = Register.OnPost(msg.PostData);
                            break;
                        case "/users/login":
                            Response = Login.OnPost(msg.PostData);
                            break;
                        case "/users/logout":
                            Response = LogOut.OnPost(msg.PostData);
                            break;
                        #endregion

                        #region AuthServer
                        case "/authserver/authenticate":
                            Response = Authenticate.OnPost(msg.PostData);
                            break;
                        case "/authserver/refresh":
                            Response = Refresh.OnPost(msg.PostData);
                            break;
                        case "/authserver/validate":
                            Response = Validate.OnPost(msg.PostData);
                            break;
                        case "/authserver/invalidate":
                            Response = Invalidate.OnPost(msg.PostData);
                            break;
                        case "/authserver/signout":
                            Response = Signout.OnPost(msg.PostData);
                            break;
                        #endregion
                        default:

                            //GET /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
                            //if (Guid.TryParse(msg.Url.Split('/')[6], out Guid guid))
                            {

                            }
                            Response = new Tuple<int, string>(403, "");
                            break;
                    }
                }
                else
                {
                    Response = new Tuple<int, string>(403, "");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            // 发送返回
            bcontect = Encoding.Default.GetBytes(Response.Item2);
            responseHeader = HttpProtocol.Make(Response.Item1, "text", bcontect.Length);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            Logger.Debug($"Response header: {responseHeader}");
            Logger.Debug($"Response contect: {Response.Item2}");

            try
            {
                Socket.Send(bresponse);
                Socket.Send(bcontect);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            finally
            {
                Socket.Close();
            }
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
