using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Data;

namespace Mimir.SQL
{
    class MySql
    {
        public static bool IsConnected { set; get; }

        private static Logger log = new Logger("MySQL");
        private static MySqlConnection mySqlConnection = new MySqlConnection($"server={Program.SqlIp};userid={Program.SqlUsername};password={Program.SqlPassword};database={Program.SqlDbName};SslMode=none;");
        
        public static MySqlConnection GetSqlConnection()
        {
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                log.Info("Reconnecting mysql server.");
                Open();
            }
            return mySqlConnection;
        }

        public static void Open()
        {
            try
            {
                mySqlConnection.Open();
                IsConnected = true;

                log.Info("MySQL connected!");
            }
            catch (Exception ex)
            {
                log.Warn(ex);
                throw;
            }
        }

        public static void Close()
        {
            if (mySqlConnection.State != ConnectionState.Closed)
            {
                mySqlConnection.Close();
            }
            IsConnected = false;
            log.Info("MySql connection closed.");
        }
    }
}
