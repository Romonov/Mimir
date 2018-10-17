using RUL;
using System;
using System.Collections.Generic;

namespace Mimir.Common
{
    class ConfigWorker
    {
        public static bool Init(string ConfigPath)
        {
            try
            {
                INI.Write(ConfigPath, "General", "Name", Program.ServerName);
                INI.Write(ConfigPath, "General", "Port", Program.Port.ToString());
                INI.Write(ConfigPath, "General", "MaxConnection", Program.MaxConnection.ToString());
                //INI.Write(ConfigPath, "General", "SSL", Program.IsSslEnabled.ToString());

                INI.Write(ConfigPath, "SQL", "Type", Program.SQLType.ToString());
                INI.Write(ConfigPath, "SQL", "IP", Program.SQLIP);
                INI.Write(ConfigPath, "SQL", "Username", Program.SQLUsername);
                INI.Write(ConfigPath, "SQL", "Password", Program.SQLPassword);

                //INI.Write(ConfigPath, "SSL", "IsCustomSSL", Program.IsCustomCert.ToString());
                //INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
                //INI.Write(ConfigPath, "SSL", "Password", Program.SslCertPassword);

                INI.Write(ConfigPath, "SkinDomains", "Count", "1");
                INI.Write(ConfigPath, "SkinDomains", "1", ".romonov.com");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 存配置文件方法
        /// </summary>
        /// <param name="ConfigPath">配置文件路径</param>
        /// <returns>成功与否</returns>
        public static bool Save(string ConfigPath)
        {
            try
            {
                INI.Write(ConfigPath, "General", "Name", Program.ServerName);
                INI.Write(ConfigPath, "General", "Port", Program.Port.ToString());
                INI.Write(ConfigPath, "General", "MaxConnection", Program.MaxConnection.ToString());
                //INI.Write(ConfigPath, "General", "SSL", Program.IsSslEnabled.ToString());

                INI.Write(ConfigPath, "SQL", "Type", Program.SQLType.ToString());
                INI.Write(ConfigPath, "SQL", "IP", Program.SQLIP);
                INI.Write(ConfigPath, "SQL", "Username", Program.SQLUsername);
                INI.Write(ConfigPath, "SQL", "Password", Program.SQLPassword);

                //INI.Write(ConfigPath, "SSL", "IsCustomSSL", Program.IsCustomCert.ToString());
                //INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
                //INI.Write(ConfigPath, "SSL", "Password", Program.SslCertPassword);

                INI.Write(ConfigPath, "SkinDomains", "Count", Program.SkinDomainsCount.ToString());

                for (int i = 1; i <= Program.SkinDomains.Length + 1; i++)
                {
                    INI.Write(ConfigPath, "SkinDomains", i.ToString(), Program.SkinDomains[i]);
                }
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读配置文件方法
        /// </summary>
        /// <param name="ConfigPath">配置文件路径</param>
        /// <returns>成功与否</returns>
        public static bool Read(string ConfigPath)
        {
            try
            {
                Program.ServerName = INI.Read(ConfigPath, "General", "Name");
                Program.Port = int.Parse(INI.Read(ConfigPath, "General", "Port"));
                Program.MaxConnection = int.Parse(INI.Read(ConfigPath, "General", "MaxConnection"));
                //Program.IsSslEnabled = BoolParse(INI.Read(ConfigPath, "General", "SSL"));

                Program.SQLType = GetSQLType(ConfigPath);
                Program.SQLIP = INI.Read(ConfigPath, "SQL", "IP");
                Program.SQLUsername = INI.Read(ConfigPath, "SQL", "Username");
                Program.SQLPassword = INI.Read(ConfigPath, "SQL", "Password");

                //Program.IsCustomCert = BoolParse(INI.Read(ConfigPath, "SSL", "IsCustomSSL"));
                //Program.SslCertName = INI.Read(ConfigPath, "SSL", "Cert");
                //Program.SslCertPassword = INI.Read(ConfigPath, "SSL", "Password");

                Program.SkinDomainsCount = int.Parse(INI.Read(ConfigPath, "SkinDomains", "Count"));
                List<string> list = new List<string>();
                for (int i = 1; i <= int.Parse(INI.Read(ConfigPath, "SkinDomains", "Count")); i++)
                {
                    list.Add(INI.Read(ConfigPath, "SkinDomains", i.ToString()));
                }
                Program.SkinDomains = list.ToArray();
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 辨别SQL服务器设置方法
        /// </summary>
        /// <param name="ConfigPath">配置文件路径</param>
        /// <returns>返回SQL服务器类型</returns>
        static SQLType GetSQLType(string ConfigPath)
        {
            switch(INI.Read(ConfigPath, "SQL", "Type").ToLower())
            {
                case "mysql":
                    return SQLType.MySql;
                case "mssql":
                    return SQLType.MsSql;
                case "mariadb":
                    return SQLType.MariaDB;
                case "sqlite":
                    return SQLType.Sqlite;
                default:
                    Logger.Error("Bad sql server type, using MySql");
                    INI.Write(ConfigPath, "SQL", "Type", SQLType.MySql.ToString());
                    return SQLType.MySql;
            }
        }

        /// <summary>
        /// 把特定字符串转换为对应的布尔值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>布尔值</returns>
        static bool BoolParse(string str)
        {
            if(str.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// SQL服务器类型
        /// </summary>
        public enum SQLType
        {
            MySql, 
            MsSql, 
            MariaDB,
            Sqlite,
        }
    }
}
