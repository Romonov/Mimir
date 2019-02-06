using Mimir.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Common
{
    class GetName
    {
        public static string GetProfile(Guid uuid)
        {
            DataSet dataSetProfiles = SqlProxy.Query($"select * from `profiles` where `UnsignedUUID` = '{uuid.ToString()}'");

            if (SqlProxy.IsEmpty(dataSetProfiles))
            {
                return "";
            }

            return dataSetProfiles.Tables[0].Rows[0]["Name"].ToString();
        }
    }
}
