using Mimir;
using RUL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class ConfigWorker
    {
        public static bool Write(string ConfigPath)
        {
            try
            {
                INI.Write(ConfigPath, "General", "Name", Program.ServerName);
                INI.Write(ConfigPath, "General", "Port", Program.Port.ToString());
                INI.Write(ConfigPath, "General", "MaxConnection", Program.MaxConnection.ToString());
                INI.Write(ConfigPath, "General", "SSL", Program.IsSslEnabled.ToString());

                INI.Write(ConfigPath, "SQL", "Type", Program.SQLType.ToString());
                INI.Write(ConfigPath, "SQL", "IP", Program.SQLIP);
                INI.Write(ConfigPath, "SQL", "Username", Program.SQLUsername);
                INI.Write(ConfigPath, "SQL", "Password", Program.SQLPassword);

                INI.Write(ConfigPath, "SSL", "CertChain", Program.IsSslUseCertChain.ToString());
                INI.Write(ConfigPath, "SSL", "CA", Program.SslCAName);
                INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
                INI.Write(ConfigPath, "SSL", "Key", Program.SslKeyName);

                INI.Write(ConfigPath, "SkinDomains", "Count", Program.SkinDomains.Length.ToString());
                List<string> list = new List<string>();
                for (int i = 0; i < int.Parse(INI.Read(ConfigPath, "SkinDomains", "Count")); i++)
                {
                    INI.Write(ConfigPath, "SkinDomains", "Count", Program.SkinDomains.Length.ToString());
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

        public static bool Read(string ConfigPath)
        {
            try
            {
                Program.ServerName = INI.Read(ConfigPath, "General", "Name");
                Program.Port = int.Parse(INI.Read(ConfigPath, "General", "Port"));
                Program.MaxConnection = int.Parse(INI.Read(ConfigPath, "General", "MaxConnection"));
                Program.IsSslEnabled = BoolParse(INI.Read(ConfigPath, "General", "SSL"));

                Program.SQLType = GetSQLType(ConfigPath);
                Program.SQLIP = INI.Read(ConfigPath, "SQL", "IP");
                Program.SQLUsername = INI.Read(ConfigPath, "SQL", "Username");
                Program.SQLPassword = INI.Read(ConfigPath, "SQL", "Password");

                Program.IsSslUseCertChain = BoolParse(INI.Read(ConfigPath, "SSL", "CertChain"));
                Program.SslCAName = INI.Read(ConfigPath, "SSL", "CA");
                Program.SslCAContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslCAName}");
                Program.SslCertName = INI.Read(ConfigPath, "SSL", "Cert");
                Program.SslCertContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslCertName}");
                Program.SslKeyName = INI.Read(ConfigPath, "SSL", "Key");
                Program.SslKeyContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslKeyName}");

                Program.SkinDomainsCount = int.Parse(INI.Read(ConfigPath, "SkinDomains", "Count"));
                List<string> list = new List<string>();
                for (int i = 0; i < int.Parse(INI.Read(ConfigPath, "SkinDomains", "Count")); i++)
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

        static SQLType GetSQLType(string configPath)
        {
            switch(INI.Read(configPath, "SQL", "Type"))
            {
                case "MySql":
                    return SQLType.MySql;
                case "MsSql":
                    return SQLType.MsSql;
                case "MariaDB":
                    return SQLType.MariaDB;
                case "Sqlite":
                    return SQLType.Sqlite;
                default:
                    Logger.Error("Bad sql server type, using MySql");
                    INI.Write(configPath, "SQL", "Type", SQLType.MySql.ToString());
                    return SQLType.MySql;
            }
        }

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

        public enum SQLType
        {
            MySql, 
            MsSql, 
            MariaDB,
            Sqlite,
        }
    }
}
