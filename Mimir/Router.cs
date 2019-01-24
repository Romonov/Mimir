using Mimir.Response;
using RUL;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class Router
    {
        private static Logger log = new Logger("Router");

        public static void Route(string httpReq, Socket socket)
        {
            HttpReq req = HttpProtocol.Solve(httpReq);

            log.Info($"Got request {req.Method} {req.Url} from {socket.RemoteEndPoint}.");

            (int status, string type, string content) response = (403, "text/plain", "");

            try
            {
                if (req.Method == Method.Get)
                {
                    switch (req.Url)
                    {
                        case "/":
                            response = Root.OnGet();
                            break;
                        default:
                            break;
                    }
                }
                else if (req.Method == Method.Post)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Poster(response.status, response.type, response.content, socket);
        }

        private static void Poster(int status, string responseType, string response, Socket socket)
        {
            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Server", "Mimir");
            header.Add("Author", "Romonov");
            header.Add("Version", Program.Version);
            header.Add("X-Authlib-Injector-API-Location", "/");

            // 发送返回
            bcontect = Encoding.Default.GetBytes(response);
            responseHeader = HttpProtocol.Build(status, responseType, bcontect.Length, header);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            if (Program.IsDebug)
            {
                log.Debug($"Response header: {responseHeader}");
                log.Debug($"Response contect: {response}");
            }

            try
            {
                socket.Send(bresponse);
                socket.Send(bcontect);
            }
            catch (Exception e)
            {
                if (Program.IsDebug)
                {
                    log.Error(e);
                }
                else
                {
                    log.Error(e.Message);
                }
            }
            finally
            {
                socket.Close();
            }
        }
    }
}
