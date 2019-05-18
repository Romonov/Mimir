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
        /// 从Mojang获取Uuid的返回类型
        /// </summary>
        struct MojangUUID
        {
            public string id;
            public string name;
        }
    }
}
