using Newtonsoft.Json;
using System;

namespace Mimir.Response
{
    class Root
    {
        public static ValueTuple<int, string, string> OnGet()
        {
            GetRootResponse response = new GetRootResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = "Mimir";
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = new string[] { ".mojang.com", ".minecraft.net"};
            response.signaturePublickey = $"-----BEGIN PUBLIC KEY-----\n{Program.SkinPublicKey}\n-----END PUBLIC KEY-----\n";

            return (200, "text/plain", JsonConvert.SerializeObject(response));
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
