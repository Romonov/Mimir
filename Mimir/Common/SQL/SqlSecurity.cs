using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mimir.Common.SQL
{
    class SqlSecurity
    {
        public static bool Check(string Data)
        {
            Regex pattern1 = new Regex(@"[-|;|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
            Regex pattern2 = new Regex(@"(\%27)|(\')|(\-\-)");
            Regex pattern3 = new Regex(@"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))");
            Regex pattern4 = new Regex(@"\s+exec(\s|\+)+(s|x)p\w+");

            if (pattern1.IsMatch(Data) || pattern2.IsMatch(Data) || pattern3.IsMatch(Data) || pattern4.IsMatch(Data))
            {
                return false;
            }

            return true;
        }
    }
}
