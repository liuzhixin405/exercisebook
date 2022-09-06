using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverThree
{
    internal class TestObserver
    {
        public void TestMulticst()
        {
            SubjectBase<int> subject = new Subject<int>();
            Observer<int> observer1 = new Observer<int>();
            observer1.State = 10;
            Observer<int> observer2 = new Observer<int>();
            observer2.State = 20;
            subject += observer1;
            subject += observer2;
            subject.Update(1);
            Console.WriteLine($"observer1.State={observer1.State}  observer2.State={observer2.State}");
            subject -= observer1;
            subject.Update(100);
            Console.WriteLine($"update state = 100, observer1.State={observer1.State}  observer2.State={observer2.State}");
        }

     
        public void TestMultiSubject()
        {
            SubjectBase<string> subject1 = new Subject<string>();
            SubjectBase<string> subject2 = new Subject<string>();
            Observer<string> observer1 = new Observer<string>();
            observer1.State = "运动";
            Console.WriteLine($"observer1.State={observer1.State}");
            subject1 += observer1;
            subject2 += observer1;
            subject1.Update("看电影");
            Console.WriteLine($"observer1.State={observer1.State}");
            subject2.Update("喝茶");
            Console.WriteLine($"observer1.State={observer1.State}");

            subject1 -= observer1;
            subject2 -= observer1;
            observer1.State = "休息";
            subject1 -= observer1;
            subject2 -= observer1;
            Console.WriteLine($"observer1.State={observer1.State}");
        }
    }
}
