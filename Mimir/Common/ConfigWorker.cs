using RUL;
using System;
using System.Collections.Generic;
using System.Net;

namespace Mimir.Common
{
    class ConfigWorker
    {
        private static Logger log = new Logger("Config");

        public static bool Init(string configPath)
        {
            try
            {
                INI.Write(configPath, "General", "Name", Program.ServerName);
                INI.Write(configPath, "General", "Port", Program.Port.ToString());
                INI.Write(configPath, "General", "MaxConnection", Program.MaxConnection.ToString());
                //INI.Write(ConfigPath, "General", "SSL", Program.IsSslEnabled.ToString());
                INI.Write(configPath, "General", "Debug", Program.IsDebug.ToString());

                INI.Write(configPath, "SQL", "Type", Program.SQLType.ToString());
                INI.Write(configPath, "SQL", "IP", Program.SQLIP);
                INI.Write(configPath, "SQL", "Username", Program.SQLUsername);
                INI.Write(configPath, "SQL", "Password", Program.SQLPassword);

                //INI.Write(ConfigPath, "SSL", "IsCustomSSL", Program.IsCustomCert.ToString());
                //INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
                //INI.Write(ConfigPath, "SSL", "Password", Program.SslCertPassword);

                INI.Write(configPath, "SkinDomains", "Count", "1");
                INI.Write(configPath, "SkinDomains", "1", "*.romonov.com");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 存配置文件方法
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>成功与否</returns>
        public static bool Save(string configPath)
        {
            try
            {
                INI.Write(configPath, "General", "Name", Program.ServerName);
                INI.Write(configPath, "General", "Port", Program.Port.ToString());
                INI.Write(configPath, "General", "MaxConnection", Program.MaxConnection.ToString());
                //INI.Write(ConfigPath, "General", "SSL", Program.IsSslEnabled.ToString());
                INI.Write(configPath, "General", "Debug", Program.IsDebug.ToString());

                INI.Write(configPath, "SQL", "Type", Program.SQLType.ToString());
                INI.Write(configPath, "SQL", "IP", Program.SQLIP);
                INI.Write(configPath, "SQL", "Username", Program.SQLUsername);
                INI.Write(configPath, "SQL", "Password", Program.SQLPassword);

                //INI.Write(ConfigPath, "SSL", "IsCustomSSL", Program.IsCustomCert.ToString());
                //INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
                //INI.Write(ConfigPath, "SSL", "Password", Program.SslCertPassword);

                INI.Write(configPath, "SkinDomains", "Count", Program.SkinDomainsCount.ToString());

                for (int i = 1; i <= Program.SkinDomains.Length + 1; i++)
                {
                    INI.Write(configPath, "SkinDomains", i.ToString(), Program.SkinDomains[i - 1]);
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读配置文件方法
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>成功与否</returns>
        public static bool Read(string configPath)
        {
            try
            {
                Program.ServerName = INI.Read(configPath, "General", "Name");
                Program.Port = int.Parse(INI.Read(configPath, "General", "Port"));
                Program.MaxConnection = int.Parse(INI.Read(configPath, "General", "MaxConnection"));
                //Program.IsSslEnabled = BoolParse(INI.Read(ConfigPath, "General", "SSL"));
                Program.IsDebug = BoolParse(INI.Read(configPath, "General", "Debug"));

                Program.SQLType = GetSQLType(configPath);
                Program.SQLIP = INI.Read(configPath, "SQL", "IP");
                Program.SQLUsername = INI.Read(configPath, "SQL", "Username");
                Program.SQLPassword = INI.Read(configPath, "SQL", "Password");

                //Program.IsCustomCert = BoolParse(INI.Read(ConfigPath, "SSL", "IsCustomSSL"));
                //Program.SslCertName = INI.Read(ConfigPath, "SSL", "Cert");
                //Program.SslCertPassword = INI.Read(ConfigPath, "SSL", "Password");

                Program.SkinDomainsCount = int.Parse(INI.Read(configPath, "SkinDomains", "Count"));
                List<string> list = new List<string>();
                for (int i = 1; i <= int.Parse(INI.Read(configPath, "SkinDomains", "Count")); i++)
                {
                    list.Add(INI.Read(configPath, "SkinDomains", i.ToString()));
                }
                Program.SkinDomains = list.ToArray();
            }
            catch(Exception e)
            {
                log.Error(e.Message);
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
                    log.Error("Bad sql server type, using MySql");
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
