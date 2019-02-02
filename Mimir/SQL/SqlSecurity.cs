namespace Mimir.SQL
{
    class SqlSecurity
    {
        public static string Parse(string data)
        {
            return data.Replace("'", "\'").Replace("\"", "\\\"");
        }
    }
}
