using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Batch
{
    public abstract class DecisionBase
    {
        protected IBatchFactory _factory;
        protected int _quantity;

        public DecisionBase(IBatchFactory factory,int quantity)
        {
            this._factory = factory;
            this._quantity = quantity;
        }
        public virtual IBatchFactory Factory => _factory;
        public virtual int Quantity => _quantity;
    }

    public abstract class DirectorBase
    {
        protected IList<DecisionBase> decisions = new List<DecisionBase>();
        /// <summary>
        /// 实际项目中，组好每个director需要添加的decision也定义在配置文件中
        /// 这样增加新的decision项都在后台完成，而不需要assembler显式调用该方法补充
        /// </summary>
        /// <param name="decision"></param>
        protected virtual void Insert(DecisionBase decision)
        {
            if (decision == null || decision.Factory == null || decision.Quantity < 0) throw new ArgumentNullException("decision");
            decisions.Add(decision);
        }
        /// <summary>
        /// 便于客户程序使用增加的迭代器
        /// </summary>
        public virtual IEnumerable<DecisionBase> Decisions => decisions;

    }
}
