using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern04
{
    public interface IBusinessObjct
    {
        int ID { get; }
        string Name { get; }
    }

    public abstract class BusinessObjectBase : IBusinessObjct
    {
        protected int id;
        protected string name;
        public int ID => id;

        public string Name => name;
        public BusinessObjectBase(int id,string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
