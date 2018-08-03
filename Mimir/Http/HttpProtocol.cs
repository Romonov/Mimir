using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Http
{
    class HttpProtocol
    {
        struct Request
        {
            public Method ReqType;
            public Protocol ReqProtocol;
            public string Url;
            public IPAddress From;
            public Dictionary<string, string> Get;
            public string UA;
            public string[] AcceptType;
            public string[] AcceptEncoding;
            public string[] AcceptLanguage;
            public string Cookies;
            public string Post;
        }

        struct Reponse
        {
            
        }

        enum Method
        {
            Get = 0,
            Post = 1,
            Head = 2,
            Put = 3,
            Delete = 4,
            Options = 5,
            Trace = 6,
            Conntect = 7
        }

        enum Protocol
        {
            HTTP10,
            HTTP11,
            HTTPS
        }
    }
}
