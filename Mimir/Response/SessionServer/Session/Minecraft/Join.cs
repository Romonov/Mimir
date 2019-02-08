using Mimir.SQL;
using Mimir.Response.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimir.Util;

namespace Mimir.Response.SessionServer.Session.Minecraft
{
    class Join
    {
        public static ValueTuple<int, string, string> OnPost(string postData, string clientIP)
        {
            // Post /sessionserver/session/minecraft/join
            Request request = JsonConvert.DeserializeObject<Request>(postData);

            DataSet dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}' and `Status` = 2;");
            DataRow dataRowToken;

            if (dataSetToken?.Tables[0]?.Rows?.Count >= 1)
            {
                dataRowToken = dataSetToken.Tables[0].Rows[0];
            }
            else
            {
                return InvalidToken.GetResponse();
            }

            DataSet dataSetProfile = SqlProxy.Query($"select * from `profiles` where `Name` = '{dataRowToken["BindProfile"].ToString()}' and `UnsignedUUID` = '{SqlSecurity.Parse(request.selectedProfile)}';");

            if (dataSetProfile?.Tables[0]?.Rows?.Count >= 1)
            {
                SqlProxy.Excute($"insert into `sessions` (`ServerID`, `AccessToken`, `ClientIP`, `ExpireTime`) values ('{SqlSecurity.Parse(request.serverId)}', '{SqlSecurity.Parse(request.accessToken)}', '{clientIP}', '{TimeWorker.GetTimeStamp(30000)}');");
            }
            else
            {
                return InvalidToken.GetResponse();
            }

            return (204, "text/plain", "");
        }

        struct Request
        {
            public string accessToken;
            public string selectedProfile;
            public string serverId;
        }
    }
}
