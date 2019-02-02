using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public object GetConnection()
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    return Sqlite.GetSqlConnection();
                case SqlConnectionType.MySql:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
            }
        }

        public static DataSet Query(string sql)
        {
            switch (Program.SqlType)
            {
                case SqlConnectionType.Sqlite:
                    if (!Sqlite.IsConnected)
                    {
                        Open();
                    }
                    DataSet dataSet = new DataSet();
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(sql, Sqlite.GetSqlConnection()))
                    {
                        using (SQLiteDataAdapter sqliteDataAdapter = new SQLiteDataAdapter(sqliteCommand))
                        {
                            sqliteDataAdapter.Fill(dataSet);
                        }
                    }
                    return dataSet;
                case SqlConnectionType.MySql:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
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
                    break;
                default:
                    break;
            }
        }
    }

    public enum SqlConnectionType
    {
        Sqlite,
        MySql
    }
}
