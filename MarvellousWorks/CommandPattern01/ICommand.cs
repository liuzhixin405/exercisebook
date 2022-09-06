using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern01
{
    /// <summary>
    /// 抽象命令对象
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 提供给调用者的统一操作方法
        /// </summary>
        void Execute();
        /// <summary>
        /// 定义命令对象实际操作的对象
        /// </summary>
        Receiver Recceiver { set; }
    }

    public abstract class CommandBase : ICommand
    {
        protected Receiver recceiver;
        public Receiver Recceiver
        {
            set { recceiver = value; }
        }
        public abstract void Execute();
    }
}
