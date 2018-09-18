﻿using RUL;
using RUL.HTTP;
using Mimir.Response;
using Mimir.Response.AuthServer;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Mimir.Response.Mimir;

namespace Mimir.Common
{
    class Processor
    {
        public static void Process(string req, Socket socket, SslStream sslStream)
        {
            HttpMsg msg = HttpProtocol.Solve(req);

            string contect = "";
            byte[] bcontect;
            string response = "";
            byte[] bresponse;

            int status = 200;
            if (msg.Method == Method.Get)
            {
                switch (msg.Url)
                {
                    case "/":
                        status = 200;
                        contect = Root.OnGet();
                        break;
                    case "/mimir/notice":
                        status = 200;
                        contect = Notice.OnGet();
                        break;
                    default:
                        status = 404;
                        break;
                }
            }
            else if (msg.Method == Method.Post)
            {
                switch (msg.Url)
                {
                    #region AuthServer
                    case "/authserver/authenticate":
                        status = 200;
                        contect = Authenticate.OnPost(msg.PostData);
                        break;
                    case "/authserver/refresh":
                        status = 200;
                        contect = Refresh.OnPost(msg.PostData);
                        break;
                    case "/authserver/validate":
                        status = 204;
                        contect = Validate.OnPost(msg.PostData);
                        break;
                    case "/authserver/invalidate":
                        status = 204;
                        contect = Invalidate.OnPost(msg.PostData);
                        break;
                    case "/authserver/signout":
                        status = 204;
                        contect = Signout.OnPost(msg.PostData);
                        break;
                    #endregion
                    default:
                        status = 403;
                        break;
                }
            }
            else
            {
                switch (msg.Url)
                {
                    default:
                        status = 403;
                        break;
                }
            }

            bcontect = Encoding.Default.GetBytes(contect);
            response = HttpProtocol.Make(status, "text", bcontect.Length);
            bresponse = Encoding.Default.GetBytes(response);

            Logger.Info($"Response header: {response}");
            Logger.Info($"Response contect: {contect}");

            //if (Program.UseSsl)
            {
                sslStream.Write(bresponse);
                sslStream.Write(bcontect);
                sslStream.Flush();
            }
            //else
            {
            //    socket.Send(bresponse);
            //    socket.Send(bcontect);
            }

            if (msg.Connection != Connection.KeepAlive)
            {
                sslStream.Close();
                socket.Close();
            }

            return;
        }
    }
}