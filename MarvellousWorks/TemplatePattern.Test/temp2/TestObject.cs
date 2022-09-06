using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp2;

namespace TemplatePattern.Test.temp2
{
    /// <summary>
    /// 同时实现多个Template的具体类型
    /// </summary>
    public class DataBroker : ITransform, ISetter
    {
        #region ISetter Members
        private ISetter setter = new InternalSetter();
        public string Append(string data)
        {
            return setter.Append(data);
        }

        public bool CheckHeader(string data)
        {
            return setter.CheckHeader(data);
        }

        public bool CheckTailer(string data)
        {
            return setter.CheckTailer(data);
        }
        #endregion
        #region ITransform Members
        private ITransform transform = new InternalTransform();
        public bool Parse(string data)
        {
            return transform.Parse(data);
        }

        public string Replacce(string data)
        {
            return transform.Replacce(data);
        }

        public string Transform(string data)
        {
            return transform.Transform(data);
        }
        #endregion

        class InternalTransform : TransformBase
        {
            public override bool Parse(string data)
            {
                return data.Contains("X");
            }

            public override string Replacce(string data)
            {
                return data.Replace("X", "Y");
            }
        }
        class InternalSetter : SetterBase
        {
            public override bool CheckHeader(string data)
            {
                return data.StartsWith("H:");
            }

            public override bool CheckTailer(string data)
            {
                return data.EndsWith(":T");
            }
        }
    }
}
