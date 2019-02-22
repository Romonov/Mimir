using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Data;
using System.Data.SQLite;

namespace Mimir.SQL
{
    /// <summary>
    /// Sql访问代理类
    /// </summary>
    class SqlProxy
    {
        private static Logger log = new Logger("SqlProxy");

        /// <summary>
        /// 打开Sql链接
        /// </summary>
        public static void Open()
        {
            log.Info("Connecting database...");

            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Sqlite.Open();
                    break;

                case SqlConnectionType.MySql:
                    MySql.Open();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 获得Sql链接
        /// </summary>
        /// <returns>Sql链接</returns>
        public object GetConnection()
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    return Sqlite.GetSqlConnection();

                case SqlConnectionType.MySql:
                    return MySql.GetSqlConnection();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Todo
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void Init()
        {
            log.Info("Initializing the database...");

        }

        /// <summary>
        /// 进行Sql查询
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <returns>返回结果数据集</returns>
        public static DataSet Query(string sql)
        {
            DataSet dataSet = new DataSet();

            try
            {
                switch (Program.SqlType)
                {
                    case SqlConnectionType.Sqlite:
                        if (Sqlite.GetSqlConnection().State == ConnectionState.Broken)
                        {
                            Open();
                        }
                        using (SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sql, Sqlite.GetSqlConnection()))
                        {
                            sqliteDataAdapter.Fill(dataSet);
                        }
                        return dataSet;

                    case SqlConnectionType.MySql:
                        if (MySql.GetSqlConnection().State == ConnectionState.Broken)
                        {
                            Open();
                        }
                        using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, MySql.GetSqlConnection()))
                        {
                            mySqlDataAdapter.Fill(dataSet);
                        }
                        return dataSet;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 执行Sql语句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <returns>影响的行数</returns>
        public static int Excute(string sql)
        {
            try
            {
                switch (Program.SqlType)
                {
                    case SqlConnectionType.Sqlite:
                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(sql, Sqlite.GetSqlConnection()))
                        {
                            return sqliteCommand.ExecuteNonQuery();
                        }

                    case SqlConnectionType.MySql:
                        using (MySqlCommand mySqlCommand = new MySqlCommand(sql, MySql.GetSqlConnection()))
                        {
                            return mySqlCommand.ExecuteNonQuery();
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 关闭Sql链接
        /// </summary>
        public static void Close()
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    if (Sqlite.IsConnected)
                    {
                        Sqlite.Close();
                    }
                    break;

                case SqlConnectionType.MySql:
                    if (MySql.IsConnected)
                    {
                        MySql.Close();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 判断数据集是否为空
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool IsEmpty(DataSet ds)
        {
            if ((ds == null) || (ds.Tables.Count == 0) || (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 0))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Sql链接类型
    /// </summary>
    public enum SqlConnectionType
    {
        Sqlite,
        MySql
    }
}
