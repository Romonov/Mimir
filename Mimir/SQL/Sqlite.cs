using RUL;
using System;
using System.Data;
using System.Data.SQLite;

namespace Mimir.SQL
{
    class Sqlite
    {
        public static bool IsConnected { set; get; }

        private static Logger log = new Logger("Sqlite");
        private static SQLiteConnection sqliteConnection = new SQLiteConnection($"data source={Program.SqlDbName}.db");

        public static SQLiteConnection GetSqlConnection()
        {
            if (sqliteConnection.State == ConnectionState.Closed)
            {
                log.Info("Reconnecting sqlite database.");
                Open();
            }
            return sqliteConnection;
        }

        public static void Open()
        {
            try
            {
                sqliteConnection.Open();
                IsConnected = true;

                log.Info("Sqlite connected!");
            }
            catch (Exception ex)
            {
                log.Warn(ex);
                throw;
            }
        }

        public static void Close()
        {
            if (sqliteConnection.State != ConnectionState.Closed)
            {
                sqliteConnection.Close();
            }
            IsConnected = false;
            log.Info("Sqlite connection closed.");
        }
    }
}
