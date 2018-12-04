using System.Text.RegularExpressions;

namespace Mimir.Common.SQL
{
    class SqlSecurity
    {
        public static bool Check(string data)
        {
            Regex pattern1 = new Regex(@"[-|;|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
            Regex pattern2 = new Regex(@"(\%27)|(\')|(\-\-)");
            Regex pattern3 = new Regex(@"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))");
            Regex pattern4 = new Regex(@"\s+exec(\s|\+)+(s|x)p\w+");

            if (pattern1.IsMatch(data) || pattern2.IsMatch(data) || pattern3.IsMatch(data) || pattern4.IsMatch(data))
            {
                return false;
            }

            return true;
        }

        public static string Parse(string data)
        {
            return data.Replace("'", "\'").Replace("\"", "\\\"");
        }
    }
}
