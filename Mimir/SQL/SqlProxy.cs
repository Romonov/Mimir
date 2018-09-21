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

        public static DataSet Querier(string Command)
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
                    Logger.Warn(e.Message);
                }
            }

            return dataSet;
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
                    Logger.Warn(e.Message);
                }
                finally
                {
                    mySqlCommand.Dispose();
                }
            }

            return rows;
        }
    }
}
