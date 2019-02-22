namespace Mimir.SQL
{
    /// <summary>
    /// Sql安全类
    /// </summary>
    class SqlSecurity
    {
        /// <summary>
        /// 转义敏感字符
        /// </summary>
        /// <param name="data">要转义的数据</param>
        /// <returns>转义后的数据</returns>
        public static string Parse(string data)
        {
            return data.Replace("'", "\'").Replace("\"", "\\\"");
        }
    }
}
