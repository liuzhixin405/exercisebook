using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Common
{
    public class MySign
    {
        #region 验证签名
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool CheckSign(SortedDictionary<string, string> requestData, string signature)
        {
            //判断
            if (!requestData.ContainsKey("timeStamp")) return false;
            if (!requestData.ContainsKey("random")) return false;
            if (requestData.ContainsKey("signature"))
            {
                requestData.Remove("signature");
            }
            string signdata = CreateSignString(requestData); 
            string mysign = CommonUtils.MD5(signdata + CommonUtils.SIGNSALT); 
            return mysign.ToUpper() == signature.ToUpper();
        }
        #endregion

        #region 根据字典拼接
        public static string CreateSignString(SortedDictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Value);
            }
            return prestr.ToString();
        }
        #endregion
    }
}
