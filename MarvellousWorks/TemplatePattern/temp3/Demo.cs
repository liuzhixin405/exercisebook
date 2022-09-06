using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern.temp3
{
    /// <summary>
    /// 事件参数 相当与抽象算法的数据结构约束
    /// </summary>
    public class CounterEventArgs : EventArgs
    {
        private int value;
        public CounterEventArgs(int value) => this.value = value;  
        public int Value => value;
    }
    /// <summary>
    /// 这里事件相当于抽象操作的入口规格，而抽象算法为.NET的事件响应机制
    /// </summary>
    public class Counter
    {
        private int value = 0;
        public event EventHandler<CounterEventArgs> Changed;

        public void Add()
        {
            value++;
            Changed(this, new CounterEventArgs(value));
        }
    }
}
