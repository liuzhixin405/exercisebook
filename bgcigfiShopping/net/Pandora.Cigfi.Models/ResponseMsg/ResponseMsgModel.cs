/*
 * 作者：pc/DESKTOP-QGGMQC1
 * 时间：2018-09-13 10:47:53
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using Newtonsoft.Json;

namespace Pandora.Cigfi.Models.ResponseMessage
{

    /// <summary>
    /// 消息对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseMessage<T>
    {
        /// <summary>
        /// 消息状态
        /// </summary>
        public int Code { get; set; } = 500;
        /// <summary>
        /// 消息状态描述
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 消息数据
        /// </summary>
        private T _data;
        public T Data
        {
            get
            {
                if (_data == null)
                {
                    var type = typeof(T);
                    if (type == typeof(string))
                    {
                        _data = (T)(object)string.Empty;
                    }
                    else
                    {
                        try
                        {
                            _data = (T)System.Activator.CreateInstance(type);
                        }
                        catch
                        {
                            _data = default(T);
                        }
                    }
                }
                return _data;
            }
            set { _data = value; }
        }

        public static ResponseMessage<T> Parse(string json)
        {
            return JsonConvert.DeserializeObject<ResponseMessage<T>>(json);
        }
    }
}