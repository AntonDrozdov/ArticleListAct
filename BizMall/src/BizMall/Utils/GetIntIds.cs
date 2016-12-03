using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BizMall.Utils
{
    public static class GetIntIds
    {
        public static List<int> ConvertIdsToInt(string input_str_ids) {
            string[] str_ids = input_str_ids.Split('_');
            List<int> ids = new List<int>();
            foreach (var id in str_ids)
            {
                if (id.Length != 0)
                    ids.Add(Convert.ToInt32(id));
            }
            return ids;
        }
    }
}
