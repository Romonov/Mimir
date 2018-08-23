using RUL;
using RUL.HTTP;
using System.Net.Sockets;
using System.Text;

namespace Mimir.Common
{
    class Processor
    {
        public static void Process(string req, Socket socket)
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
                        contect = "";
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

            socket.Send(bresponse);
            socket.Send(bcontect);

            if (msg.Connection != Connection.KeepAlive)
            {
                socket.Close();
            }

            return;
        }
    }
}