using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern.temp2
{
    public interface ITransform
    {
        string Transform(string data);
        bool Parse(string data);
        string Replacce(string data);
    }
    public interface ISetter
    {
        string Append(string data);
        bool CheckHeader(string data);
        bool CheckTailer(string data);
    }

    public abstract class TransformBase : ITransform
    {
        public abstract bool Parse(string data);
        public abstract string Replacce(string data);
        public virtual string Transform(string data)
        {
            if (Parse(data))
                data = Replacce(data);
            return data;
        }
    }

    public abstract class SetterBase : ISetter
    {
        public virtual string Append(string data)
        {
            if (!CheckHeader(data)) data = "H:" + data;
            if (!CheckTailer(data)) data = data + ":T";
            return data;
        }
        public abstract bool CheckHeader(string data);
        public abstract bool CheckTailer(string data);
        
    }
}
