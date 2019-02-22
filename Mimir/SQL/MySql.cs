using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Data;

namespace Mimir.SQL
{
    /// <summary>
    /// MySql操作类
    /// </summary>
    class MySql
    {
        public static bool IsConnected { set; get; }

        private static Logger log = new Logger("MySQL");
        private static MySqlConnection mySqlConnection = new MySqlConnection($"server={Program.SqlIp};userid={Program.SqlUsername};password={Program.SqlPassword};database={Program.SqlDbName};SslMode=none;");

        /// <summary>
        /// 获得MySql链接
        /// </summary>
        /// <returns>MySql链接</returns>
        public static MySqlConnection GetSqlConnection()
        {
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                log.Info("Reconnecting mysql server.");
                Open();
            }
            return mySqlConnection;
        }

        /// <summary>
        /// 打开MySql链接
        /// </summary>
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

        /// <summary>
        /// 关闭MySql链接
        /// </summary>
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
