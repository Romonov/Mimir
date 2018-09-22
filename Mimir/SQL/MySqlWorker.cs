using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir.SQL
{
    class MySqlWorker
    {
        public static bool isConneted = false;

        public static MySqlConnection mySqlConnection = new MySqlConnection($"server={Program.SQLIP};userid={Program.SQLUsername};password={Program.SQLPassword};database={Program.SQLDatabase};SslMode=none;");

        static readonly ThreadStart threadStart = new ThreadStart(KeepAlive);
        static Thread thread = new Thread(threadStart);

        public static MySqlConnection GetSqlConnection()
        {
            return mySqlConnection;
        }

        public static void Open()
        {
            try
            {
                mySqlConnection.Open();
                isConneted = true;

                Logger.Info("MySQL connected!");

                thread.Start();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void Close()
        {
            thread.Abort();
            mySqlConnection.Close();
        }

        static void KeepAlive()
        {
            while (true)
            {
                Thread.Sleep(8 * 60 * 60 * 10000 - 10000);

                MySqlCommand cmd = new MySqlCommand($"Select * from notice", mySqlConnection);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    Logger.Info("MySQL connection has refreshed.");
                }

                reader.Close();
            }
        }
    }
}
