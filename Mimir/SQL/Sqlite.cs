using RUL;
using System;
using System.Data;
using System.Data.SQLite;

namespace Mimir.SQL
{
    /// <summary>
    /// Sqlite操作类
    /// </summary>
    class Sqlite
    {
        public static bool IsConnected { set; get; }

        private static Logger log = new Logger("Sqlite");
        private static SQLiteConnection sqliteConnection = new SQLiteConnection($"data source={Program.SqlDbName}.db");

        /// <summary>
        /// 获得Sqlite链接
        /// </summary>
        /// <returns>Sqlite链接</returns>
        public static SQLiteConnection GetSqlConnection()
        {
            if (sqliteConnection == null)
            {
                IsConnected = false;
                Open();
            }
            else if (sqliteConnection.State == ConnectionState.Closed)
            {
                log.Info("Connection closed, reconnecting.");
                IsConnected = false;
                Open();
            }
            else if (sqliteConnection.State == ConnectionState.Broken)
            {
                Close();
                Open();
            }

            return sqliteConnection;
        }

        /// <summary>
        /// 打开Sqlite链接
        /// </summary>
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

        /// <summary>
        /// 关闭Sqlite链接
        /// </summary>
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
