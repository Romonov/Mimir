using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Mimir
{
    class Response
    {
        public static string GetRoot()
        {
            RespRoot respRoot = new RespRoot();
            RespRootMeta respRootMeta = new RespRootMeta();

            respRootMeta.serverName = Program.ServerName;
            respRootMeta.implementationName = Program.Name;
            respRootMeta.implementationVersion = Program.Version;

            respRoot.Meta = respRootMeta;
            respRoot.skinDomains = new string[] {"*.pacrea.org"};
            respRoot.signaturePublickey = "";


            string contect = JsonConvert.SerializeObject(respRoot);

            return contect;

        }

        struct RespRootMeta
        {
            public string serverName;
            public string implementationName;
            public string implementationVersion;
        }
        struct RespRoot
        {
            public RespRootMeta Meta;
            public string[] skinDomains;
            public string signaturePublickey;
        }
    }
}
