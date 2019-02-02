using RUL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.SQL
{
    class Sqlite
    {
        public static bool IsConnected { set; get; }

        private static Logger log = new Logger("Sqlite");
        private static SQLiteConnection sqliteConnection = new SQLiteConnection($"data source={Program.SqlDbName}.db");

        public static SQLiteConnection GetSqlConnection()
        {
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
            sqliteConnection.Close();
            log.Info("Sqlite connection closed.");
        }

    }
}
