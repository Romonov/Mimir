using RUL;
using RUL.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class Processor
    {
        public static void Process(string req, Socket socket)
        {
            Request msg = HttpProtocol.Solve(req);

            string contect = "";
            byte[] bcontect;
            string response = "";
            byte[] bresponse;

            int status = 200;

            switch (msg.Url)
            {
                case "/":
                    status = 200;
                    if(msg.ReqType == Method.Post)
                    {
                        contect = Response.GetRoot();
                    }

                    break;
                default:
                    status = 404;
                    return;
            }

            bcontect = Encoding.Default.GetBytes(contect);
            response = HttpProtocol.Make(status, "text", bcontect.Length);
            bresponse = Encoding.Default.GetBytes(response);

            Logger.Info($"Response header: {response}");
            Logger.Info($"Response contect: {contect}");

            socket.Send(bresponse);
            socket.Send(bcontect);

            return;
        }        
    }
}
