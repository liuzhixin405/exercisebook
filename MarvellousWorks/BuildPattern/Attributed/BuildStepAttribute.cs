using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildPattern.Attributed
{
    /// <summary>
    /// 指导每个具体类型BuildPart国恒目标方法和执行情况的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class BuildStepAttribute : Attribute, IComparable
    {
        private int sequence;
        private int times;
        private MethodInfo handler;
        /// <summary>
        /// 确保每个BuildStepAttribute可以根据sequence比较执行次序
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(BuildStepAttribute)) throw new ArgumentException("obj");
            return this.sequence - ((BuildStepAttribute)obj).sequence;
        }
        public BuildStepAttribute(int sequence,int times)
        {
            this.sequence = sequence;
            this.times = times;
        }
        public BuildStepAttribute(int sequence) : this(sequence, 1) { }
        /// <summary>
        /// 该Attribute需要执行的目标方法
        /// </summary>
        public MethodInfo Handler { get { return handler; } set { this.handler = value; } }
        /// <summary>
        /// 标注该Attribute的方法，在执行过程中的次序
        /// </summary>
        public int Sequence => sequence;
        /// <summary>
        /// 标注该Attribute的方法，在执行过程中的次数
        /// </summary>
        public int Times => times;

    }

    
}
