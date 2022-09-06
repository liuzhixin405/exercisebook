using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Expressions
{
    public class Serializable
    {
        public static TOut DeSerializ<TIn, TOut>(TIn tin)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(Newtonsoft.Json.JsonConvert.SerializeObject(tin));
        }
    }
}
