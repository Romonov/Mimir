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
    class INIWorker
    {
        public static string ConfigPath = Directory.GetCurrentDirectory() + @"\config.ini";

        public static void Write()
        {
            INI.Write(ConfigPath, "General", "Port", Program.Port.ToString());
            INI.Write(ConfigPath, "General", "MaxConnection", Program.MaxConnection.ToString());
            INI.Write(ConfigPath, "General", "SSL", Program.UseSsl.ToString());

            INI.Write(ConfigPath, "SQL", "IP", Program.SQLIP);
            INI.Write(ConfigPath, "SQL", "Username", Program.SQLUsername);
            INI.Write(ConfigPath, "SQL", "Password", Program.SQLPassword);

            INI.Write(ConfigPath, "SSL", "CertChain", Program.SslUseCABundle.ToString());
            INI.Write(ConfigPath, "SSL", "CA", Program.SslCAName);
            INI.Write(ConfigPath, "SSL", "Cert", Program.SslCertName);
            INI.Write(ConfigPath, "SSL", "Key", Program.SslKeyName);
        }

        public static void Read()
        {
            Program.Port = int.Parse(INI.Read(ConfigPath, "General", "Port"));
            Program.MaxConnection = int.Parse(INI.Read(ConfigPath, "General", "MaxConnection"));
            Program.UseSsl = BoolParse(INI.Read(ConfigPath, "General", "MaxConnection"));

            Program.SQLIP = INI.Read(ConfigPath, "SQL", "IP");
            Program.SQLUsername = INI.Read(ConfigPath, "SQL", "Username");
            Program.SQLPassword = INI.Read(ConfigPath, "SQL", "Password");

            Program.SslUseCABundle = BoolParse(INI.Read(ConfigPath, "SSL", "CertChain"));
            Program.SslCAName = INI.Read(ConfigPath, "SSL", "CA");
            Program.SslCAContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslCAName}");
            Program.SslCertName = INI.Read(ConfigPath, "SSL", "Cert");
            Program.SslCertContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslCertName}");
            Program.SslKeyName = INI.Read(ConfigPath, "SSL", "Key");
            Program.SslKeyContent = File.ReadAllText($@"{Program.Path}\Cert\{Program.SslKeyName}");
        }

        static bool BoolParse(string str)
        {
            if(str.ToLower() == "true" || str.ToLower() == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
