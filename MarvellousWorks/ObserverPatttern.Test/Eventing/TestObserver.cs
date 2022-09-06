using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObserverPattern.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPatttern.Test.Eventing
{
    [TestClass]
    public class TestObserver
    {
        [TestMethod]
        public void Test()
        {
            User user = new User();
            user.NamedChanged += OnNameChanged;
            user.Name = "joe";
        }

        private void OnNameChanged(object sender,UserEventArgs args)
        {
            Assert.AreEqual<string>("joe", args.Name);
        }
    }
}
