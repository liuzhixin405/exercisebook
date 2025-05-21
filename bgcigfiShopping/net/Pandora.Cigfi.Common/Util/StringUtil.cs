using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Common.Util
{
    public class StringUtil
    {
        /// <summary>
        /// 剔除逗号分隔的字符串中重复的标签
        /// </summary>
        /// <param name="items">是一个逗号分隔的字符串</param>
        /// <returns></returns>
        public static string GetDistinctItems(string items)
        {
            string[] itemArray = items.Split(',');
            HashSet<string> strHs = new HashSet<string>();
            StringBuilder strBuilder = new StringBuilder();
            foreach(string item in itemArray)
            {
                if (!strHs.Contains(item))
                {
                     strHs.Add(item);
                    if (strBuilder.Length > 0)
                    {
                        strBuilder.Append(",");
                    }
                    strBuilder.Append(item);
                }
            }
            return strBuilder.ToString();
        }
    }
}
