using RUL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class InitWorker
    {
        public static bool Init()
        {
            try
            {
                if (!File.Exists(Program.Path + @"/config.ini"))
                {
                    Logger.Warn("Config file is missing, will create now.");


                    INI.Write(Program.Path, @"/config.ini", "General", "ServerName", Program.ServerName);
                    INI.Write(Program.Path, @"/config.ini", "General", "Port", Program.Port.ToString());
                    INI.Write(Program.Path, @"/config.ini", "General", "MaxConnection", Program.MaxConnection.ToString());
                }

                Program.ServerName = INI.Read(Program.Path, @"/config.ini", "General", "ServerName");
                Program.Port = int.Parse(INI.Read(Program.Path, @"/config.ini", "General", "Port"));
                Program.MaxConnection = int.Parse(INI.Read(Program.Path, @"/config.ini", "General", "MaxConnection"));

                Main.Init(Program.Port, Program.MaxConnection);
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }
    }
}
