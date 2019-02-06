using Mimir.SQL;
using Mimir.Util;
using Newtonsoft.Json;
using RUL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.SessionServer.Session.Minecraft
{
    class HasJoined
    {
        public static ValueTuple<int, string, string> OnGet(Dictionary<string, string> arguments)
        {
            // Get /sessionserver/session/minecraft/hasJoined?username={username}&serverId={serverId}&ip={ip}
            Response response = new Response();
            
            DataSet dataSetSessions = SqlProxy.Query($"select * from `sessions` where `ServerID` = '{SqlSecurity.Parse(arguments["serverId"])}' and `ExpireTime` > {TimeWorker.GetTimeStamp()}");
            if (SqlProxy.IsEmpty(dataSetSessions))
            {
                return (204, "text/plain", "");
            }

            DataSet dataSetToken = SqlProxy.Query($"select * from `tokens` where `BindProfile` = '{SqlSecurity.Parse(arguments["username"])}';");
            if (SqlProxy.IsEmpty(dataSetToken))
            {
                return (204, "text/plain", "");
            }

            DataRow dataRowSession = null;
            foreach (DataRow dataRow in dataSetSessions.Tables[0].Rows)
            {
                foreach (DataRow dataRowToken in dataSetToken.Tables[0].Rows)
                {
                    if (dataRow["AccessToken"].ToString() == dataRowToken["AccessToken"].ToString())
                    {
                        dataRowSession = dataRow;
                    }
                }
                
            }
            if (dataRowSession == null)
            {
                return (204, "text/plain", "");
            }

            if (arguments.ContainsKey("ip"))
            {
                if (arguments["ip"] != dataRowSession["ClientIP"].ToString())
                {
                    return (204, "text/plain", "");
                }
            }

            DataSet dataSetProfile = SqlProxy.Query($"select * from `profiles` where `Name` = '{SqlSecurity.Parse(arguments["username"])}'");
            if (SqlProxy.IsEmpty(dataSetProfile))
            {
                return (204, "text/plain", "");
            }
            DataRow dataRowProfile = dataSetProfile.Tables[0].Rows[0];

            response.id = dataRowProfile["UnsignedUUID"].ToString();
            response.name = dataRowProfile["Name"].ToString();

            return (200, "text/plain", JsonConvert.SerializeObject(response));
        }

        struct Response
        {
            public string id;
            public string name;
            public Properties?[] properties;
        }
        struct Properties
        {
            public string name;
            public string value;
            public string signature;
        }
    }
}
