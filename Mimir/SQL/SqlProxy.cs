using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SQLite;

namespace Mimir.SQL
{
    class SqlProxy
    {
        public static void Open()
        {
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

        public static void Init()
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    throw new NotImplementedException();
                case SqlConnectionType.MySql:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DataSet Query(string sql)
        {
            DataSet dataSet = new DataSet();

            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    if (!Sqlite.IsConnected)
                    {
                        Open();
                    }
                    using (SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sql, Sqlite.GetSqlConnection()))
                    {
                        sqliteDataAdapter.Fill(dataSet);
                    }
                    return dataSet;

                case SqlConnectionType.MySql:
                    if (!MySql.IsConnected)
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

        public static int Excute(string sql)
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    if (!Sqlite.IsConnected)
                    {
                        Open();
                    }
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(sql, Sqlite.GetSqlConnection()))
                    {
                        return sqliteCommand.ExecuteNonQuery();
                    }

                case SqlConnectionType.MySql:
                    if (!MySql.IsConnected)
                    {
                        Open();
                    }
                    using (MySqlCommand mySqlCommand = new MySqlCommand(sql, MySql.GetSqlConnection()))
                    {
                        return mySqlCommand.ExecuteNonQuery();
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void Close()
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    Sqlite.Close();
                    break;

                case SqlConnectionType.MySql:
                    MySql.Close();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum SqlConnectionType
    {
        Sqlite,
        MySql
    }
}
