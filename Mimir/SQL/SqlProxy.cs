using Mimir.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir.SQL
{
    class SqlProxy
    {
        public static void Open()
        {
            if(Program.SQLType == ConfigWorker.SQLType.MySql)
            {
                MySqlWorker.Open();
            }
        }

        public static void Close()
        {
            if (Program.SQLType == ConfigWorker.SQLType.MySql)
            {
                MySqlWorker.Close();
            }
        }

        public static object GetSqlConnection()
        {
            if (Program.SQLType == ConfigWorker.SQLType.MySql)
            {
                return MySqlWorker.GetSqlConnection();
            }

            return null;
        }
    }
}
