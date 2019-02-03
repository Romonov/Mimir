using MySql.Data.MySqlClient;
using RUL;
using System;
using System.Data;
using System.Data.SQLite;

namespace Mimir.SQL
{
    class SqlProxy
    {
        private static Logger log = new Logger("SqlProxy");

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
            log.Info("Initializing the database...");

            Excute("DROP TABLE IF EXISTS `profiles`;");
            Excute("CREATE TABLE `profiles` (`ID` int(11) NOT NULL AUTO_INCREMENT, `UserID` text NOT NULL, `Name` text NOT NULL, `UnsignedUUID` text NOT NULL, `IsSelected` tinyint(1) NOT NULL DEFAULT '0', PRIMARY KEY (`ID`)) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;");

            Excute("DROP TABLE IF EXISTS `sessions`;");
            Excute("CREATE TABLE `sessions` (`ServerID` char(100) NOT NULL, `AccessToken` text NOT NULL, `ClientIP` text NOT NULL COMMENT, `ExpireTime` double NOT NULL COMMENT, PRIMARY KEY (`ServerID`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;");

            Excute("DROP TABLE IF EXISTS `tokens`;");
            Excute("CREATE TABLE `tokens` (`AccessToken` char(32) NOT NULL, `ClientToken` char(32) NOT NULL, `BindProfile` text, `CreateTime` double NOT NULL, `Status` tinyint(1) NOT NULL, `BindUser` text NOT NULL, PRIMARY KEY (`AccessToken`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;");

            Excute("DROP TABLE IF EXISTS `users`;");
            Excute("CREATE TABLE `users` (`ID` int(11) NOT NULL AUTO_INCREMENT, `UUID` char(32) NOT NULL, `Username` char(20) NOT NULL, `Password` text NOT NULL, `Email` char(30) NOT NULL, `Nickname` text NOT NULL, `PreferredLanguage` text NOT NULL, `LastLogin` double(13,0) NOT NULL DEFAULT '0', `CreateTime` double(13,0) NOT NULL, `IsLogged` tinyint(1) NOT NULL DEFAULT '0', `IsAdmin` tinyint(1) NOT NULL DEFAULT '0', `IsVerified` tinyint(1) NOT NULL DEFAULT '0', `TryTimes` int(11) NOT NULL DEFAULT '0', PRIMARY KEY (`ID`,`UUID`,`Username`,`Email`)) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;");
        }

        public static DataSet Query(string sql)
        {
            DataSet dataSet = new DataSet();

            try
            {
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
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public static int Excute(string sql)
        {
            try
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
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
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

        public static bool IsEmpty(DataSet ds)
        {
            if ((ds == null) || (ds.Tables.Count == 0) || (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 0))
            {
                return true;
            }
            return false;
        }
    }

    public enum SqlConnectionType
    {
        Sqlite,
        MySql
    }
}
