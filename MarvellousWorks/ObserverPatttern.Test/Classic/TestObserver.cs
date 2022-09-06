using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObserverPattern.Classic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPatttern.Test.Classic
{
    [TestClass]
    public class TestObserver
    {
        /// <summary>
        /// 验证目标类型《subject》对观察者类型《observer》的1:N通知
        /// </summary>
        [TestMethod]
        public void TestMulticst()
        {
            SubjectBase<int> subject = new SubjectA<int>();
            Observer<int> observer1 = new Observer<int>();
            observer1.State = 10;
            Observer<int> observer2 = new Observer<int>();
            observer2.State = 20;
            subject += observer1;
            subject += observer2;
            //确认通知有效性
            subject.Update(1);
            Assert.AreEqual<int>(1, observer1.State);
            Assert.AreEqual<int>(1, observer2.State);

            subject -= observer1;
            subject.Update(4);
            Assert.AreEqual<int>(1, observer1.State);
            Assert.AreEqual<int>(4, observer2.State);
        }
        /// <summary>
        /// 验证通过一个观察者对象observer可以同时观察多个目标多项subject
        /// </summary>
        [TestMethod]
        public void TestMultiSubject()
        {
            SubjectBase<int> subjectA = new SubjectA<int>();
            SubjectBase<int> subjectB = new SubjectB<int>();

            Observer<int> observer = new Observer<int>();
            observer.State = 20;
            subjectA += observer;
            subjectB += observer;
            subjectA.Update(10);
            Assert.AreEqual(10, observer.State);
            subjectB.Update(8);
            Assert.AreEqual(8, observer.State);
        }
    }
}
