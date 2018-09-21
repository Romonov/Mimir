using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response
{
    class Root
    {
        public static ReturnContent OnGet()
        {
            ReturnContent returnContect = new ReturnContent();

            GetRootResponse response = new GetRootResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = Program.Name;
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = Program.SkinDomains;
            response.signaturePublickey = Program.SkinPublicKey;

            returnContect.Contect = JsonConvert.SerializeObject(response);
            returnContect.Status = 200;

            return returnContect;
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
