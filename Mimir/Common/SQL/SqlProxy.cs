using Mimir.Common;
using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir.Common.SQL
{
    class SqlProxy
    {
        private static Logger log = new Logger("SQL");

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

        public static DataSet Query(string Command)
        {
            DataSet dataSet = new DataSet();

            if (Program.SQLType == ConfigWorker.SQLType.MySql)
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(Command, MySqlWorker.GetSqlConnection());
                try
                {
                    mySqlDataAdapter.Fill(dataSet);
                }
                catch (Exception e)
                {
                    log.Warn(e.Message);
                }
            }

            return dataSet;
        }

        public static bool Save(DataSet dataSet)
        {

            return false;
        }

        public static int Excuter(string Command)
        {
            int rows = 0;

            if (Program.SQLType == ConfigWorker.SQLType.MySql)
            {
                MySqlCommand mySqlCommand = new MySqlCommand(Command, MySqlWorker.GetSqlConnection());
                try
                {
                    rows = mySqlCommand.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Warn(e.Message);
                }
                finally
                {
                    mySqlCommand.Dispose();
                }
            }

            return rows;
        }

        public static bool IsEmpty(DataSet ds)
        {
            if((ds == null) || (ds.Tables.Count == 0) || (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 0))
            {
                return true;
            }
            return false;
        }
    }
}
