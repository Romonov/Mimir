using Mimir.SQL;
using Mimir.Util;
using Newtonsoft.Json;
using RUL.Encode;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir.Response.Common
{
    class GetProfile
    {
        public static string Get(string name, bool containProperties = false, bool isUnsigned = true)
        {
            return InternalGet(name, containProperties, isUnsigned);
        }

        public static string Get(Guid uuid, bool containProperties = false, bool isUnsigned = true)
        {
            return InternalGet(GetName.FromUuid(uuid), containProperties, isUnsigned);
        }

        private static string InternalGet(string name, bool containProperties, bool isUnsigned)
        {
            Response response = new Response();

            DataSet dataSetProfiles = SqlProxy.Query($"select * from `profiles` where `Name` = '{name}'");

            if (SqlProxy.IsEmpty(dataSetProfiles))
            {
                return "";
            }

            DataRow dataRowProfile = dataSetProfiles.Tables[0].Rows[0];

            string playerName = dataRowProfile["Name"].ToString();
            string playerUUID = dataRowProfile["UnsignedUUID"].ToString();

            if (containProperties)
            {
                string skinUrl = "";

                if (SqlProxy.Query($"select * from `profiles` where `Name` = '{playerName}'").Tables[0].Rows[0]["Skin"].ToString() == "")
                {
                    HttpWebRequest requestMojangUUID = (HttpWebRequest)WebRequest.Create($"https://api.mojang.com/users/profiles/minecraft/{playerName}");
                    HttpWebResponse responseMojangUUID = (HttpWebResponse)requestMojangUUID.GetResponse();
                    string mojangUUID = "";
                    using (Stream responseStreamMojangUUID = responseMojangUUID.GetResponseStream())
                    {
                        using (StreamReader responseStreamReaderMojangUUID = new StreamReader(responseStreamMojangUUID))
                        {
                            mojangUUID = JsonConvert.DeserializeObject<MojangUUID>(responseStreamReaderMojangUUID.ReadToEnd()).id;
                        }
                    }

                    HttpWebRequest requestSkin = (HttpWebRequest)WebRequest.Create($"https://sessionserver.mojang.com/session/minecraft/profile/{mojangUUID}");
                    HttpWebResponse responseSkin = (HttpWebResponse)requestSkin.GetResponse();

                    using (Stream responseStreamSkin = responseSkin.GetResponseStream())
                    {
                        using (StreamReader responseStreamReaderSkin = new StreamReader(responseStreamSkin))
                        {
                            Response skinResponse = JsonConvert.DeserializeObject<Response>(responseStreamReaderSkin.ReadToEnd());
                            Texture skinTexture = JsonConvert.DeserializeObject<Texture>(Base64.Decoder(skinResponse.properties[0].Value.value));
                            skinUrl = skinTexture.textures.SKIN.url;

                            SqlProxy.Excute($"update `profiles` set `Skin` = '{skinUrl}' where `Name` = '{playerName}'");
                        }
                    }
                }
                else
                {
                    skinUrl = SqlProxy.Query($"select * from `profiles` where `Name` = '{playerName}'").Tables[0].Rows[0]["Skin"].ToString();
                }

                Skin skin = new Skin();
                skin.url = skinUrl;

                Textures textures = new Textures();
                textures.SKIN = skin;

                Texture texture = new Texture();
                texture.timestamp = TimeWorker.GetJavaTimeStamp();
                texture.profileId = playerUUID;
                texture.profileName = playerName;
                texture.textures = textures;

                Properties propertie = new Properties();
                propertie.name = "textures";
                propertie.value = Base64.Encoder(JsonConvert.SerializeObject(texture));

                if (!isUnsigned)
                {
                    propertie.signature = RSAWorker.Sign(propertie.value);
                }

                response.properties = new Properties?[] { propertie };
            }

            response.id = playerUUID;
            response.name = playerName;

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
            public Skin SKIN;
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

        struct MojangUUID
        {
            public string id;
            public string name;
        }
    }
}
