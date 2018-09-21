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
        public static void Process(string req, Socket socket, SslStream sslStream, bool IsSSL)
        {
            HttpMsg msg = HttpProtocol.Solve(req);

            string contect = "";
            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            int status = 200;

            ReturnContent response = new ReturnContent();

            if (msg.Method == Method.Get)
            {
                switch (msg.Url)
                {
                    case "/":
                        response = Root.OnGet();
                        break;
                    case "/mimir/notice":
                        response = Notice.OnGet();
                        break;
                    default:
                        response.Status = 403;
                        break;
                }
            }
            else if (msg.Method == Method.Post)
            {
                switch (msg.Url)
                {
                    #region Users
                    case "/users/register":
                        response = Register.OnPost(msg.PostData);
                        break;
                    case "/users/login":
                        response = Login.OnPost(msg.PostData);
                        break;
                    case "/users/logout":
                        response = LogOut.OnPost(msg.PostData);
                        break;
                    #endregion

                    #region AuthServer
                    case "/authserver/authenticate":
                        response = Authenticate.OnPost(msg.PostData);
                        break;
                    case "/authserver/refresh":
                        response = Refresh.OnPost(msg.PostData);
                        break;
                    case "/authserver/validate":
                        response = Validate.OnPost(msg.PostData);
                        break;
                    case "/authserver/invalidate":
                        response = Invalidate.OnPost(msg.PostData);
                        break;
                    case "/authserver/signout":
                        response = Signout.OnPost(msg.PostData);
                        break;
                    #endregion
                    default:
                        /*
                        GET /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}

                        if (Guid.TryParse(msg.Url.Split('/')[5], out Guid guid))
                        {

                        }
                        */
                        response.Status = 403;
                        break;
                }
            }
            else
            {
                switch (msg.Url)
                {
                    default:
                        response.Status = 403;
                        break;
                }
            }

            bcontect = Encoding.Default.GetBytes(response.Contect);
            responseHeader = HttpProtocol.Make(response.Status, "text", bcontect.Length);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            Logger.Info($"Response header: {responseHeader}");
            Logger.Info($"Response contect: {contect}");

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
                if (msg.Connection != Connection.KeepAlive)
                {
                    sslStream.Close();
                    socket.Close();
                }
            }
            return;
        }

        public struct ReturnContent
        {
            public string Contect;
            public int Status;
        }
    }
}