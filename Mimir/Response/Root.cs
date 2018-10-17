using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response
{
    class Root
    {
        public static Tuple<int, string> OnGet()
        {
            GetRootResponse response = new GetRootResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = Program.Name;
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = Program.SkinDomains;
            response.signaturePublickey = $"-----BEGIN PUBLIC KEY-----\n{Program.SkinPublicKey}\n-----END PUBLIC KEY-----\n";
            
            return new Tuple<int, string>(200, JsonConvert.SerializeObject(response));
        }
    }

    struct GetRootResponse
    {
        public GetRootMeta meta;
        public string[] skinDomains;
        public string signaturePublickey;
    }

    struct GetRootMeta
    {
        public string serverName;
        public string implementationName;
        public string implementationVersion;
    }
}
