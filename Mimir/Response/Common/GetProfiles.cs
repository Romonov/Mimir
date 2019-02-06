using Mimir.SQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Common
{
    class GetProfile
    {
        public static string Get(string name, bool isUnsigned = true)
        {
            return InternalGet(name, isUnsigned);
        }

        public static string Get(Guid uuid, bool isUnsigned = true)
        {
            return InternalGet(GetName.GetProfile(uuid), isUnsigned);
        }

        private static string InternalGet(string name, bool isUnsigned)
        {
            Response response = new Response();

            DataSet dataSetProfiles = SqlProxy.Query($"select * from `profiles` where `Name` = '{name}'");

            if (SqlProxy.IsEmpty(dataSetProfiles))
            {
                return "";
            }

            DataRow dataRowProfile = dataSetProfiles.Tables[0].Rows[0];
            response.id = dataRowProfile["UnsignedUUID"].ToString();
            response.name = dataRowProfile["Name"].ToString();

            return JsonConvert.SerializeObject(response);
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
        struct Texture
        {
            public string timestamp;
            public string profileId;
            public string profileName;
            public Textures textures;
        }
        struct Textures
        {
            public Skin?[] skin;
        }
        struct Skin
        {
            public string url;
            public Metadata? metadata;
        }
        struct Metadata
        {
            public string model;
        }
    }
}
