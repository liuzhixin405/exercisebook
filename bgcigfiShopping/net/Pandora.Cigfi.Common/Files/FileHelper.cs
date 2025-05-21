using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pandora.Cigfi.Common.Files
{
    public  class FileHelper
    {
       
        /// <summary>
        /// 根据文件名判断上传文件格式
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileSuffix(string filename)
        {
            var result = filename.Split(".");
            if (result.Length != 0)
                return result[result.Length-1];
            return "";
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="base64string">文件base64</param>
        /// <param name="storagePath">保存路径</param>
        /// <param name="width">图宽</param>
        /// <param name="height">图高</param>
        /// <param name="flag">是否自定义设置文件宽高</param>
        /// <returns></returns>
        public static async Task<ResultModel> UpLoadFilesAsync(IFormFileCollection files, string filename, string storagePath)
        { 

            var fullPath = string.Empty;
            ResultModel Result = new ResultModel();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    fullPath = storagePath + formFile.FileName;

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

           
            return Result;
        }

    }
    public class ResultModel
    {
       
        public bool IsSuc
        {
            get;
            set;
        }
        public string Msg
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
    }

}

