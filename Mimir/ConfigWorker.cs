using Mimir.SQL;
using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class ConfigWorker
    {
        private static Logger log = new Logger("Config");

        public static void Load(string path)
        {
            log.Info("Loading configs...");

            Program.ServerName = Read(path, "General", "ServerName", Program.ServerName);
            bool.TryParse(Read(path, "General", "IsDebug", Program.IsDebug.ToString()), out Program.IsDebug);
            int.TryParse(Read(path, "General", "Port", Program.Port.ToString()), out Program.Port);
            int.TryParse(Read(path, "General", "MaxConnection", Program.MaxConnection.ToString()), out Program.MaxConnection);
            Enum.TryParse(Read(path, "SQL", "Type", Program.SqlType.ToString(), true), out Program.SqlType);

            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Program.SqlDbName = Read(path, "SQL", "DatabaseName", Program.SqlDbName);
                    break;
                case SqlConnectionType.MySql:
                    Program.SqlDbName = Read(path, "SQL", "DatabaseName", Program.SqlDbName);
                    Program.SqlIp = Read(path, "SQL", "IP", Program.SqlIp);
                    Program.SqlUsername = Read(path, "SQL", "Username", Program.SqlUsername);
                    Program.SqlPassword = Read(path, "SQL", "Password", Program.SqlPassword, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            log.Info("Configs loaded.");
        }

        public static void Save(string path)
        {
            log.Info("Saving configs...");

            Write(path, "General", "ServerName", Program.ServerName);
            Write(path, "General", "IsDebug", Program.IsDebug.ToString());
            Write(path, "General", "Port", Program.Port.ToString());
            Write(path, "General", "MaxConnection", Program.MaxConnection.ToString());
            Write(path, "SQL", "Type", Program.SqlType.ToString());
            
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Write(path, "SQL", "DatabaseName", Program.SqlDbName);
                    break;
                case SqlConnectionType.MySql:
                    Write(path, "SQL", "DatabaseName", Program.SqlDbName); ;
                    Write(path, "SQL", "IP", Program.SqlIp);
                    Write(path, "SQL", "Username", Program.SqlUsername);
                    Write(path, "SQL", "Password", Program.SqlPassword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            log.Info("Configs saved.");
        }

        private static string Read(string path, string section, string key, string defaultValue, bool isHide = false)
        {
            string value = defaultValue;
            try
            {
                value = INI.Read(path, section, key);
                if (value == "")
                {
                    throw new NullReferenceException();
                }

                if (!isHide)
                {
                    log.Info($"Key \"{section}.{key}\" is set to value \"{value}\".");
                }
                else
                {
                    log.Info($"Key \"{section}.{key}\" is loaded.");
                }
            }
            catch (Exception)
            {
                log.Warn($"Key \"{section}.{key}\" is missing, will use default value.");
                Write(path, section, key, defaultValue);
            }
            return value;
        }

        private static void Write(string path, string section, string key, string value)
        {
            try
            {
                INI.Write(path, section, key, value);
            }
            catch (Exception ex)
            {
                log.Warn($"Can't write value {value} to key {key}.");
                log.Error(ex);
            }
        }
    }
}
