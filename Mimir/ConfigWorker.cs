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

            log.Info("Configs loaded.");
        }

        public static void Save(string path)
        {
            log.Info("Saving configs...");

            Write(path, "General", "ServerName", Program.ServerName);
            Write(path, "General", "IsDebug", Program.IsDebug.ToString());
            Write(path, "General", "Port", Program.Port.ToString());
            Write(path, "General", "MaxConnection", Program.MaxConnection.ToString());

            log.Info("Configs saved.");
        }

        private static string Read(string path, string section, string key, string defaultValue, bool isPassword = false)
        {
            string value = defaultValue;
            try
            {
                value = INI.Read(path, section, key);
                if (value == "")
                {
                    throw new NullReferenceException();
                }

                if (!isPassword)
                {
                    log.Info($"Key {key} is set to value {value}.");
                }
            }
            catch (Exception)
            {
                log.Warn($"Key {key} is missing, will use default value.");
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
