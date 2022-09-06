using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Observer
{
    /// <summary>
    /// 发布者
    /// </summary>
    public class EventDemo
    {
        public event Action eventInvoke;

        public void Invoke()
        {
            if (eventInvoke != null)
            {
                eventInvoke();
            }
        }
    }

    /// <summary>
    /// 事件注册
    /// </summary>
    public class EventTest
    {
        public void DoSomeThing()
        {
            EventDemo demo = new EventDemo();

            demo.eventInvoke += new Action(new Student().Study);
            demo.eventInvoke += new Action(new Teacher().Teach);
            demo.Invoke();

        }
    }

    /// <summary>
    /// 订阅者
    /// </summary>
    public class Student
    {
        public void Study()
        {
            Console.WriteLine("老师在做作业");
        }
    }
    /// <summary>
    /// 订阅者
    /// </summary>
    public class Teacher
    {
        public void Teach()
        {
            Console.WriteLine("老师在讲课");
        }
    }
}
