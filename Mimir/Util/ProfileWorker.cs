using Mimir.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// 角色工具类
    /// </summary>
    public class ProfileWorker
    {
        /// <summary>
        /// 从离线角色名获取Uuid
        /// </summary>
        /// <param name="name">离线角色名</param>
        /// <returns>无符号Uuid</returns>
        public static string GetUuidFromOfflineName(string name)
        {
            return UuidWorker.GetUuid(Encoding.UTF8.GetBytes($"OfflinePlayer:{name}"));
        }

        /// <summary>
        /// 从角色名获取Uuid
        /// </summary>
        /// <param name="name">角色名</param>
        /// <returns>无符号Uuid</returns>
        public static string GetUuidFromName(string name)
        {
            HttpWebRequest requestMojangUUID = (HttpWebRequest)WebRequest.Create($"https://api.mojang.com/users/profiles/minecraft/{name}");
            HttpWebResponse responseMojangUUID = (HttpWebResponse)requestMojangUUID.GetResponse();
            string Uuid = "";
            using (Stream responseStreamMojangUUID = responseMojangUUID.GetResponseStream())
            {
                using (StreamReader responseStreamReaderMojangUUID = new StreamReader(responseStreamMojangUUID))
                {
                    Uuid = JsonConvert.DeserializeObject<MojangUUID>(responseStreamReaderMojangUUID.ReadToEnd()).id;
                }
            }
            return Uuid;
        }

        /// <summary>
        /// 从角色Uuid获取角色名
        /// </summary>
        /// <param name="uuid">角色Uuid</param>
        /// <returns>角色名</returns>
        public static string GetNameFormUuid(MimirContext db, Guid uuid, bool isOnline = false)
        {
            if (isOnline)
            {
                var request = (HttpWebRequest)WebRequest.Create($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid.ToString("N")}");
                var response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return JsonConvert.DeserializeObject<Profile>(reader.ReadToEnd()).name;
                    }
                }
            }
            else
            {
                var profile = from p in db.Profiles where p.Uuid == uuid.ToString("N") select p;
                if (profile.Count() != 1)
                {
                    return null;
                }
                else
                {
                    return profile.First().Name;
                }
            }
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="db">数据库上下文对象</param>
        /// <param name="name">角色名</param>
        /// <param name="containProperties">是否包含属性</param>
        /// <param name="isUnsigned">是否不签名</param>
        /// <returns>角色信息</returns>
        public static Profile? GetProfile(MimirContext db, string name, bool containProperties = false, bool isUnsigned = true)
        {
            var profiles = from p in db.Profiles where p.Name == name select p;
            if (profiles.Count() != 1)
            {
                return null;
            }

            var result = new Profile();
            var profile = profiles.First();
            result.id = profile.Uuid;
            result.name = profile.Name;

            if (containProperties)
            {
                var textures = new Textures();
                var properties = new Properties();

                if (profile.SkinUrl != null && profile.SkinUrl != string.Empty)
                {
                    var metadata = new Metadata();
                    switch (profile.SkinModel)
                    {
                        case 1:
                            metadata.model = "slim";
                            break;
                        case 0:
                        default:
                            metadata.model = "default";
                            break;
                    }
                    textures.SKIN = new Skin();
                    textures.SKIN.metadata = metadata;
                    if (Program.IsHttps)
                    {
                        textures.SKIN.url = $"https://{Program.ServerDomain}/textures/{profile.SkinUrl}";
                    }
                    else
                    {
                        textures.SKIN.url = $"http://{Program.ServerDomain}/textures/{profile.SkinUrl}";
                    }

                    if (profile.CapeUrl != null && profile.CapeUrl != string.Empty)
                    {
                        var cape = new Skin();
                        if (Program.IsHttps)
                        {
                            cape.url = $"https://{Program.ServerDomain}/textures/{profile.CapeUrl}";
                        }
                        else
                        {
                            cape.url = $"https://{Program.ServerDomain}/textures/{profile.CapeUrl}";
                        }
                        textures.CAPE = cape;
                    }

                    var texture = new Texture();
                    texture.timestamp = TimeWorker.GetJavaTimeStamp();
                    texture.profileId = profile.Uuid;
                    texture.profileName = profile.Name;
                    texture.textures = textures;

                    properties.name = "textures";
                    var value = EncodeWorker.Base64Encoder(JsonConvert.SerializeObject(texture));
                    properties.value = value;

                    if (!isUnsigned)
                    {
                        properties.signature = SignatureWorker.Sign(value);
                    }
                    else
                    {
                        properties.signature = "";
                    }
                }
                result.properties = new Properties?[] { properties };
            }
            else
            {
                result.properties = new Properties?[] { new Properties() };
            }
            return result;
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="db">数据库上下文对象</param>
        /// <param name="uuid">角色UUID</param>
        /// <param name="containProperties">是否包含属性</param>
        /// <param name="isUnsigned">是否不签名</param>
        /// <returns>角色信息</returns>
        public static Profile? GetProfile(MimirContext db, Guid uuid, bool containProperties = false, bool isUnsigned = true)
        {
            var name = GetNameFormUuid(db, uuid);
            if (name == null)
            {
                return null;
            }
            return GetProfile(db, name, containProperties, isUnsigned);
        }


        /// <summary>
        /// 从Mojang获取Uuid的返回类型
        /// </summary>
        struct MojangUUID
        {
            public string id;
            public string name;
        }

        /// <summary>
        /// 角色信息
        /// </summary>
        public struct Profile
        {
            public string id;
            public string name;
            public Properties?[] properties;
        }
        public struct Properties
        {
            public string name;
            public string value;
            public string signature;
        }

        /// <summary>
        /// 皮肤信息
        /// </summary>
        public struct Texture
        {
            public string timestamp;
            public string profileId;
            public string profileName;
            public Textures textures;
        }
        public struct Textures
        {
            public Skin SKIN;
            public Skin? CAPE;
        }
        public struct Skin
        {
            public string url;
            public Metadata? metadata;
        }
        public struct Metadata
        {
            public string model;
        }
    }
}
