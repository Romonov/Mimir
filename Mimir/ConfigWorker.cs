using IniParser;
using IniParser.Model;
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
        private static readonly FileIniDataParser parser = new FileIniDataParser();
        private static IniData ini = parser.ReadFile("config.ini");

        public static void Load(string path)
        {
            log.Info("Loading configs...");

            Program.ServerName = Read("General", "ServerName", Program.ServerName);
            bool.TryParse(Read("General", "IsDebug", Program.IsDebug.ToString()), out Program.IsDebug);
            int.TryParse(Read("General", "Port", Program.Port.ToString()), out Program.Port);
            int.TryParse(Read("General", "MaxConnection", Program.MaxConnection.ToString()), out Program.MaxConnection);
            Enum.TryParse(Read("SQL", "Type", Program.SqlType.ToString(), true), out Program.SqlType);

            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Program.SqlDbName = Read("SQL", "DatabaseName", Program.SqlDbName);
                    break;
                case SqlConnectionType.MySql:
                    Program.SqlDbName = Read("SQL", "DatabaseName", Program.SqlDbName);
                    Program.SqlIp = Read("SQL", "IP", Program.SqlIp);
                    Program.SqlUsername = Read("SQL", "Username", Program.SqlUsername);
                    Program.SqlPassword = Read("SQL", "Password", Program.SqlPassword, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            log.Info("Configs loaded.");
        }

        public static void Save(string path)
        {
            log.Info("Saving configs...");

            Write("General", "ServerName", Program.ServerName);
            Write("General", "IsDebug", Program.IsDebug.ToString());
            Write("General", "Port", Program.Port.ToString());
            Write("General", "MaxConnection", Program.MaxConnection.ToString());
            Write("SQL", "Type", Program.SqlType.ToString());
            
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Write("SQL", "DatabaseName", Program.SqlDbName);
                    break;
                case SqlConnectionType.MySql:
                    Write("SQL", "DatabaseName", Program.SqlDbName); ;
                    Write("SQL", "IP", Program.SqlIp);
                    Write("SQL", "Username", Program.SqlUsername);
                    Write("SQL", "Password", Program.SqlPassword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            log.Info("Configs saved.");
        }

        private static string Read(string section, string key, string defaultValue, bool isHide = false)
        {
            string value = defaultValue;
            try
            {
                value = ini[section][key];
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
                log.Warn($"Key \"{section}.{key}\" can't load, will use default value.");
                Write(section, key, defaultValue);
            }
            return value;
        }

        private static void Write(string section, string key, string value)
        {
            try
            {
                ini[section][key] = value;
                parser.WriteFile("config.ini", ini);
            }
            catch (Exception ex)
            {
                log.Warn($"Can't write value {value} to key {key}.");
                log.Error(ex);
            }
        }
    }
}
