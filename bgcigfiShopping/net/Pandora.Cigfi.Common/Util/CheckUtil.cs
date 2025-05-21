using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Common.Util
{
    public class CheckUtil
    {
        /// <summary>
        /// 检查输入框的文本是否为空或长度溢出
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="maxLength"></param>
        /// <param name="itemName"></param> 
        /// <returns></returns>
        public static bool CheckTextIsEmptyOrOverflow(string inputText, int maxLength, string itemName,ref string msg)
        {
             inputText = inputText ?? "";
            msg = "";
            inputText = inputText.Trim();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                  msg = itemName+"不能为空";
                return false;
            }

            if (inputText.Trim().Length > maxLength)
            {
                  msg =   $"{itemName}的字符长度不能超过{maxLength}"   ;
                return false;
            }

        
            return true;

        }


    }

   }