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
        public static string OnGet()
        {
            string Content = "";
            GetRootResponse response = new GetRootResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = Program.Name;
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = Program.SkinDomains;
            response.signaturePublickey = Program.SkinPublicKey;
            
            Content = JsonConvert.SerializeObject(response);
            return Content;
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
